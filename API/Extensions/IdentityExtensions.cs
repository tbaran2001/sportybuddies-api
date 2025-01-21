using API.Common.Web;
using API.Data;
using API.Modules.Identity;
using Microsoft.AspNetCore.Identity;

namespace API.Extensions;

public static class IdentityExtensions
{
    public static WebApplicationBuilder AddIdentity(this WebApplicationBuilder builder)
    {
        builder.Services.ConfigureApplicationCookie(options =>
        {
            options.Cookie.HttpOnly = false;
            options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
            options.Cookie.SameSite = SameSiteMode.None;
        });

        builder.Services.AddAuthorizationBuilder();

        builder.Services.AddIdentityApiEndpoints<ApplicationUser>(options =>
            {
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequiredLength = 3;
            })
            .AddRoles<IdentityRole<Guid>>()
            .AddEntityFrameworkStores<ApplicationDbContext>();

        builder.Services.AddScoped<UserManager<ApplicationUser>, CustomUserManager>();
        builder.Services.AddScoped<IdentityEventsHandler>();
        builder.Services.AddScoped<ICurrentUserProvider, CurrentUserProvider>();

        builder.Services.AddHttpContextAccessor();

        return builder;
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