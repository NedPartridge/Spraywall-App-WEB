using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SpraywallAppWeb.Data;
using SpraywallAppWeb.Models;
using SpraywallAppWeb.Services;

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

    // Create a wall, and add it to the database
    // 
    // Request consists of a wall name, and an image of the spraywall.
    [Authorize]
    [HttpPost]
    public async Task<IActionResult> CreateWall([FromForm] CreateWallDto dto)
    {
        // Create a db context
        using (UserContext context = await DbContextFactory.CreateDbContextAsync())
        {
            // Wall must have a unique name, and an image of the wall is required
            if (dto.Image == null || dto.Image.Length == 0)
                return BadRequest("Image is required");
            if (context.Walls.Select(x => x.Name).Contains(dto.Name))
                return BadRequest("Wall name is in use");

            // Create a unique file name for the image
            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(dto.Image.FileName);
            var filePath = Path.Combine(_environment.WebRootPath, "images", fileName);

            // Save the image to the server
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await dto.Image.CopyToAsync(stream);
            }

            // Greatest potential bottleneck in the application 
            // Running the identification takes up to a whopping 2gb of ram
            string identifiedHoldsJson = WallService.IdentifyHolds(filePath);

            // If shit hits the fan, let the mobile app sort itself out
            if (identifiedHoldsJson == "error")
                return BadRequest("Unknown error");

            // Reject requests with poor quality images
            // The algorithm is thought 'good enough' such that it won't miss anything,
            // Meaning that if the wall is an 'actual' wall, it will have identifiable holds.
            if (identifiedHoldsJson == null |
                identifiedHoldsJson == "" |
                identifiedHoldsJson == "[]" |
                identifiedHoldsJson == "[{}]")
                return BadRequest("No holds identified");


            // Create a unique file name for the JSON file
            var jsonFileName = Guid.NewGuid().ToString() + ".json";
            var jsonFilePath = Path.Combine(_environment.WebRootPath, "json", jsonFileName);

            // Save the JSON data to the file
            await System.IO.File.WriteAllTextAsync(jsonFilePath, identifiedHoldsJson);

            // Create a new Wall object, and save to the database
            var wall = new Wall
            {
                Name = dto.Name,
                ImagePath = $"/images/{fileName}",
                IdentifiedHoldsJsonPath = $"/json/{jsonFileName}"
            };
            context.Walls.Add(wall);
            await context.SaveChangesAsync();

            // Send the holds back to the user, so they can view
            return Ok(identifiedHoldsJson);
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
