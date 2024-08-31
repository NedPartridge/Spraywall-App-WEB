using Microsoft.EntityFrameworkCore;
using SpraywallAppWeb.Models;

namespace SpraywallAppWeb.Data;

// The database context
// Basically the object which controls access to the database (db)
// The db is sqlite, but entirely managed by the entity framework (EF)
// - meaning it's only c# code which manages the data.
public class UserContext : DbContext
{
    // Initialise context
    protected readonly IConfiguration Configuration;
    public UserContext(IConfiguration configuration)
    {
        Configuration = configuration;
    }
    // Get db conection string from appsettings.json
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite(Configuration.GetConnectionString("UserDB"));
    }



    // Sets of objects allow data to be manipulated through class 
    // structures, rather than raw sql queries.
    public DbSet<User> Users { get; set; }
    public DbSet<Wall> Walls { get; set; }


    // Define relationships between objects
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>()
            .ToTable("User");

        // Walls each have one manager, who may manage many walls
        modelBuilder.Entity<User>()
            .HasMany(e => e.ManagedWalls)
            .WithOne(e => e.Manager)
            .HasForeignKey(e => e.ManagerID)
            .IsRequired();

        // Users can be banned from many walls, which can ban many users
        modelBuilder.Entity<User>()
            .HasMany(e => e.BannedWalls)
            .WithMany(e => e.BannedUsers);

        // Users can save many walls, which can be saved by many users
        modelBuilder.Entity<User>()
            .HasMany(e => e.SavedWalls)
            .WithMany(e => e.SavedUsers);
    }
}