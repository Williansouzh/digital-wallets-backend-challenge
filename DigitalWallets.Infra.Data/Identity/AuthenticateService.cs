using System.IO;
using System.Security.Authentication;
using System.Security.Claims;
using DigitalWallets.Domain.Account;
using DigitalWallets.Infra.Data.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace DigitalWallets.Infra.Data.Identity;

public class AuthenticateService : IAuthenticate
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly RoleManager<IdentityRole<Guid>> _roleManager;
    private readonly IHttpContextAccessor _httpContextAccessor;
    ILogger<AuthenticateService> _logger;
    public AuthenticateService(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        RoleManager<IdentityRole<Guid>> roleManager,

        ILogger<AuthenticateService> logger,
        IHttpContextAccessor httpContextAccessor)
    {
        _roleManager = roleManager;
        _userManager = userManager;
        _signInManager = signInManager;
        _logger = logger;
        _httpContextAccessor = httpContextAccessor;
    }
    public async Task<bool> RegisterUser(string email, string password, string role, string name, string lastName, string phone)
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

        return true;
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


    public async Task<bool> UpdateRefreshToken(string email, string newRefreshToken)
    {
        return await StoreRefreshToken(email, newRefreshToken);
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
