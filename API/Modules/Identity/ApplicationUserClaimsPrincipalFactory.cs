using System.Security.Claims;
using API.Data.Repositories.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace API.Modules.Identity;

public class ApplicationUserClaimsPrincipalFactory(
    UserManager<ApplicationUser> userManager,
    RoleManager<IdentityRole<Guid>> roleManager,
    IOptions<IdentityOptions> options,
    IProfilesRepository iProfilesRepository)
    : UserClaimsPrincipalFactory<ApplicationUser, IdentityRole<Guid>>(userManager, roleManager, options)
{
    public override async Task<ClaimsPrincipal> CreateAsync(ApplicationUser user)
    {
        var claimsIdentity = await GenerateClaimsAsync(user);
        var dbUser = await iProfilesRepository.GetProfileByIdAsync(user.Id);

        if(dbUser?.Description != null)
            claimsIdentity.AddClaim(new Claim(AppClaimTypes.Description, dbUser.Description));

        return new ClaimsPrincipal(claimsIdentity);
    }
}