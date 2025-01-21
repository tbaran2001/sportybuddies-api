using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace API.Modules.Identity;

public class CustomUserManager : UserManager<ApplicationUser>
{
    private readonly IdentityEventsHandler _eventsHandler;

    public CustomUserManager(IUserStore<ApplicationUser> store, IOptions<IdentityOptions> optionsAccessor,
        IPasswordHasher<ApplicationUser> passwordHasher, IEnumerable<IUserValidator<ApplicationUser>> userValidators,
        IEnumerable<IPasswordValidator<ApplicationUser>> passwordValidators, ILookupNormalizer keyNormalizer,
        IdentityErrorDescriber errors, IServiceProvider services, ILogger<UserManager<ApplicationUser>> logger,
        IdentityEventsHandler eventsHandler) : base(store, optionsAccessor, passwordHasher, userValidators,
        passwordValidators, keyNormalizer, errors, services, logger)
    {
        _eventsHandler = eventsHandler;
    }

    public override async Task<IdentityResult> CreateAsync(ApplicationUser user, string password)
    {
        var result = await base.CreateAsync(user, password);

        if (result.Succeeded)
        {
            await _eventsHandler.OnUserCreatedAsync(user);
        }

        return result;
    }
}