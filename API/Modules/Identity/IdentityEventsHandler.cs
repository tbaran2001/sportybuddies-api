using API.Data;
using API.Modules.Profiles.Models;

namespace API.Modules.Identity;

public class IdentityEventsHandler(ApplicationDbContext context)
{
    public async Task OnUserCreatedAsync(ApplicationUser user)
    {
        var userEntity = Profile.Create(user.Id, user.Name, user.DateOfBirth, user.Gender);
        await context.Profiles.AddAsync(userEntity);
        await context.SaveChangesAsync();
    }
}