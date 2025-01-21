using API.Common.Models;
using API.Data;
using API.Data.Repositories.Implementations;
using API.Data.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace API.Extensions;

public static class DatabaseExtensions
{
    public static WebApplicationBuilder AddDatabase(this WebApplicationBuilder builder)
    {
        builder.Services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseSqlServer(builder.Configuration.GetConnectionString("Database"));
        });

        builder.Services.AddScoped<IUnitOfWork>(serviceProvider =>
            serviceProvider.GetRequiredService<ApplicationDbContext>());

        builder.Services.AddScoped<ISportsRepository, SportsRepository>();
        builder.Services.AddScoped<IProfilesRepository, ProfilesRepository>();
        builder.Services.AddScoped<IMatchesRepository, MatchesRepository>();
        builder.Services.AddScoped<IBuddiesRepository, BuddiesRepository>();
        builder.Services.AddScoped<IConversationsRepository, ConversationsRepository>();

        return builder;
    }
}