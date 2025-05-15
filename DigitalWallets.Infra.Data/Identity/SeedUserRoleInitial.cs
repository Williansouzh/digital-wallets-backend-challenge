using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DigitalWallets.Domain.Account;
using Microsoft.AspNetCore.Identity;

namespace DigitalWallets.Infra.Data.Identity;

public class SeedUserRoleInitial : ISeedUserRoleInitial
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole<Guid>> _roleManager;

    public SeedUserRoleInitial(
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole<Guid>> roleManager)
    {
        _roleManager = roleManager;
        _userManager = userManager;
    }
    public async Task SeedUsersAsync()
    {
        if (await _userManager.FindByEmailAsync("adopt@localhost") == null)
        {
            var user = new ApplicationUser
            {
                UserName = "adopt@localhost",
                Email = "adopt@localhost",
                EmailConfirmed = true,
            };
            var resul = await _userManager.CreateAsync(user, "FB1mF@ln*");
            if (resul.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, "User");
            }
        }
        if (await _userManager.FindByEmailAsync("ong@localhost") == null)
        {
            var user = new ApplicationUser
            {
                UserName = "ong@localhost",
                Email = "ong@localhost",
                EmailConfirmed = true,
            };
            var resul = await _userManager.CreateAsync(user, "FB1mF@ln*");
            if (resul.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, "Admin");
            }
        }
    }

    public async Task SeedRolesAsync()
    {
        if (!await _roleManager.RoleExistsAsync("User"))
        {
            await _roleManager.CreateAsync(new IdentityRole<Guid>("User"));
        }
        if (!await _roleManager.RoleExistsAsync("Admin"))
        {
            await _roleManager.CreateAsync(new IdentityRole<Guid>("Admin"));
        }
    }

}
