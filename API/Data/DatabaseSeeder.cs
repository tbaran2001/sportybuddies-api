using API.Modules.Sports.Models;
using Microsoft.EntityFrameworkCore;

namespace API.Data;

public static class DatabaseSeeder
{
    public static void InitializeDatabaseAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var services = scope.ServiceProvider;


        var context = services.GetRequiredService<ApplicationDbContext>();
        context.Database.MigrateAsync().GetAwaiter().GetResult();

        SeedSportAsync(context);
    }

    private static void SeedSportAsync(ApplicationDbContext context)
    {
        if (context.Sports.Any())
            return;

        context.Sports.AddRange(GetInitialSports());
        context.SaveChanges();
    }

    private static IEnumerable<Sport> GetInitialSports() => new List<Sport>
    {
        Sport.Create("Football", "Football description"),
        Sport.Create("Basketball", "Basketball description"),
        Sport.Create("Tennis", "Tennis description"),
        Sport.Create("Volleyball", "Volleyball description")
    };
}