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


    // Seed the database, if it's empty
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>()
            .ToTable("User");

        modelBuilder.Entity<User>()
            .HasData(
                new User
                {
                    Id = 1,
                    Name = "Jeff",
                    Email = "jeff.theman@coolguy.com",
                    Password = "Password0!",
                }
            );
    }
}