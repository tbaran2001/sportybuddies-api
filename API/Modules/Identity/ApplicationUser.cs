using API.Modules.Profiles.Enums;
using Microsoft.AspNetCore.Identity;

namespace API.Modules.Identity;

public class ApplicationUser : IdentityUser<Guid>
{
    public string Name { get; set; }
    public DateOnly DateOfBirth { get; set; }
    public Gender Gender { get; set; }
}