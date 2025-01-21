using API.Common.Web;
using API.Data;
using API.Modules.Identity;
using Microsoft.AspNetCore.Identity;

namespace API.Extensions;

public static class IdentityExtensions
{
    public static IServiceCollection AddIdentity(this IServiceCollection services, IConfiguration configuration)
    {
        services.ConfigureApplicationCookie(options =>
        {
            options.Cookie.HttpOnly = false;
            options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
            options.Cookie.SameSite = SameSiteMode.None;
        });

        services.AddAuthorizationBuilder();

        services.AddIdentityApiEndpoints<ApplicationUser>(options =>
            {
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequiredLength = 3;
            })
            .AddRoles<IdentityRole<Guid>>()
            .AddEntityFrameworkStores<ApplicationDbContext>();

        services.AddScoped<UserManager<ApplicationUser>, CustomUserManager>();
        services.AddScoped<IdentityEventsHandler>();
        services.AddScoped<ICurrentUserProvider, CurrentUserProvider>();

        return services;
    }

    public static IEndpointRouteBuilder MapIdentityApi(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGroup("/api")
            .WithTags("Identity")
            .MapCustomIdentityApi<ApplicationUser>();

        endpoints.MapPost("/api/logout", async (SignInManager<ApplicationUser> signInManager) =>
            {
                await signInManager.SignOutAsync();
                return TypedResults.Ok();
            })
            .WithTags("Identity");

        return endpoints;
    }
}