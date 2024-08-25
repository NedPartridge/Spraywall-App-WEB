using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SpraywallAppWeb.Data;
using SpraywallAppWeb.Models;
using System.Diagnostics;

namespace SpraywallAppWeb.Controllers;


// API controller to enable wall viewing and management
[ApiController]
[Authorize]
[Route("[controller]")]
public class WallsController : ControllerBase
{

    // Initialise the controller
    //
    // Dependecy injection (DI) is used to provide the controller with a database (db) context factory - 
    // an object which creates user contexts (interfaces for the db). This behaviour is defined in program.cs
    //
    // DI also provides the webhostenvironment, which stores application information (ie file locations)
    private readonly IWebHostEnvironment _environment;
    IDbContextFactory<UserContext> DbContextFactory;

    public WallsController(IWebHostEnvironment environment, IDbContextFactory<UserContext> dbContextFactory)
    {
        _environment = environment;
        DbContextFactory = dbContextFactory;
    }

    [HttpPost]
    public async Task<IActionResult> CreateWall([FromForm] CreateWallDto dto)
    {
        using (UserContext context = await DbContextFactory.CreateDbContextAsync())
        {
            if (dto.Image == null || dto.Image.Length == 0)
                return BadRequest("Image is required");

            // Create a unique file name for the image
            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(dto.Image.FileName);
            Debug.WriteLine("Path: " + _environment.WebRootPath);
            Debug.WriteLine("Filename: " + fileName);
            var filePath = Path.Combine(_environment.WebRootPath, "images", fileName);

            // Ensure the images directory exists
            Directory.CreateDirectory(Path.GetDirectoryName(filePath));

            // Save the image to the server
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await dto.Image.CopyToAsync(stream);
            }

            // Create a new Wall object
            var wall = new Wall
            {
                Name = dto.Name,
                ImagePath = $"/images/{fileName}" // Store the relative path
            };

            // Save the wall to the database
            context.Walls.Add(wall);
            await context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetWall), new { id = wall.Id }, wall);
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetWall(int id)
    {
        using (UserContext context = await DbContextFactory.CreateDbContextAsync())
        {
            var wall = await context.Walls.FindAsync(id);
            if (wall == null)
                return NotFound();

            return Ok(wall);
        }
    }
}
