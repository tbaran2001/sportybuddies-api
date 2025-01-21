using API.Common.Models;
using API.Modules.Identity;
using API.Modules.Matches.Models;
using API.Modules.Profiles.Models;
using API.Modules.Sports.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace API.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>(options), IUnitOfWork
{
    public DbSet<Profile> Profiles { get; set; }
    public DbSet<Sport> Sports { get; set; }
    public DbSet<Match> Matches { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

        base.OnModelCreating(modelBuilder);
    }

    public async Task CommitChangesAsync()
    {
        await base.SaveChangesAsync();
    }
}