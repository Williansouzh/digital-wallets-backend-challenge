using System.Security.Authentication;
using System.Security.Claims;
using DigitalWallets.Domain.Account;
using DigitalWallets.Domain.Entities;
using DigitalWallets.Domain.Interfaces.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System.Transactions;

namespace DigitalWallets.Infra.Data.Identity;

public class AuthenticateService : IAuthenticate
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly RoleManager<IdentityRole<Guid>> _roleManager;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<AuthenticateService> _logger;
    private readonly IWalletRepository _walletRepository;
    private readonly ITransactionRepository _transactionRepository;

    public AuthenticateService(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        RoleManager<IdentityRole<Guid>> roleManager,
        IWalletRepository walletRepository,
        ILogger<AuthenticateService> logger,
        ITransactionRepository transactionRepository,
        IHttpContextAccessor httpContextAccessor)
    {
        _roleManager = roleManager;
        _userManager = userManager;
        _signInManager = signInManager;
        _logger = logger;
        _httpContextAccessor = httpContextAccessor;
        _walletRepository = walletRepository;
        _transactionRepository = transactionRepository;
    }

    public async Task<bool> RegisterUser(string email, string password, string role, string name, string lastName, string phone)
    {
        using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

        try
        {
            var user = new ApplicationUser
            {
                UserName = email,
                Email = email,
                Name = name,
                LastName = lastName,
                Phone = phone
            };

            var result = await _userManager.CreateAsync(user, password);

            if (!result.Succeeded)
            {
                _logger.LogWarning("User registration failed: {Errors}", string.Join(", ", result.Errors.Select(e => e.Description)));
                return false;
            }

            var wallet = new Wallet(user.Id, 0);
            await _walletRepository.AddAsync(wallet);

            if (!await _roleManager.RoleExistsAsync(role))
            {
                var newRole = new IdentityRole<Guid>
                {
                    Id = Guid.NewGuid(),
                    Name = role,
                    NormalizedName = role.ToUpper()
                };
                await _roleManager.CreateAsync(newRole);
            }

            await _userManager.AddToRoleAsync(user, role);

            // Register initial transaction
            //var transaction = Domain.Entities.Transaction.Create(
            //    amount: 0.1m,
            //    description: "Wallet created",
            //    timestamp: DateTime.UtcNow,
            //    senderId: user.Id,
            //    recipientId: user.Id 
            //);

            //await _transactionRepository.AddAsync(transaction);


            scope.Complete();
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during user registration.");
            return false;
        }
    }

    public async Task<AuthUser> Authenticate(string email, string password)
    {
        var result = await _signInManager.PasswordSignInAsync(email, password, isPersistent: false, lockoutOnFailure: false);

        if (!result.Succeeded)
            throw new AuthenticationException("Invalid login attempt.");

        var user = await _userManager.FindByEmailAsync(email);
        if (user == null)
            throw new AuthenticationException("User not found.");

        return new AuthUser(
            user.Id,
            user.Name,
            user.LastName,
            user.Phone,
            user.Email
        );
    }

    public async Task Logout()
    {
        await _signInManager.SignOutAsync();
    }

    public async Task<bool> StoreRefreshToken(string email, string refreshToken)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null) return false;

        var existingClaims = await _userManager.GetClaimsAsync(user);
        var existingRefreshToken = existingClaims.FirstOrDefault(c => c.Type == "refreshToken");

        if (existingRefreshToken != null)
            await _userManager.RemoveClaimAsync(user, existingRefreshToken);

        var newClaim = new Claim("refreshToken", refreshToken);
        var result = await _userManager.AddClaimAsync(user, newClaim);

        return result.Succeeded;
    }

    public Task<bool> UpdateRefreshToken(string email, string newRefreshToken)
    {
        return StoreRefreshToken(email, newRefreshToken);
    }

    public async Task<bool> ValidateRefreshToken(string email, string refreshToken)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null) return false;

        var claims = await _userManager.GetClaimsAsync(user);
        var storedRefreshToken = claims.FirstOrDefault(c => c.Type == "refreshToken")?.Value;

        return storedRefreshToken == refreshToken;
    }

    public async Task<AuthUser?> GetCurrentUser()
    {
        var principal = _httpContextAccessor.HttpContext?.User;
        if (principal == null) return null;

        var user = await _userManager.GetUserAsync(principal);
        if (user == null) return null;

        var roles = await _userManager.GetRolesAsync(user);

        return new AuthUser(
            user.Id,
            user.Name,
            user.LastName,
            user.Phone,
            user.Email
        )
        {
            Roles = roles
        };
    }

    public async Task<bool> IsUserLoggedIn()
    {
        var user = await _userManager.GetUserAsync(_httpContextAccessor.HttpContext.User);
        return user != null;
    }
}
