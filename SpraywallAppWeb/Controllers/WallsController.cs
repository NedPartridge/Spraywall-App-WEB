using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SpraywallAppWeb.Data;
using SpraywallAppWeb.Helpers;
using SpraywallAppWeb.Models;
using System.Diagnostics;
using System.Text.Json;

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

    HttpClient _httpClient = new(); 

    // Create a wall, and add it to the database
    // 
    // Request consists of a wall name, and an image of the spraywall.
    //
    // Contacts a python api (built & hosted by yours truly) to analyse the uploaded image files
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

            // Construct the full URL with the fileName as a query parameter
            // Hardcoding is ok - only the one device, on api, yada yada - see solution requirements
            string baseUrl = "http://localhost:8000";
            string requestUrl = $"{baseUrl}/identify-holds?fileName={Uri.EscapeDataString(fileName)}";

            //// Contact the python api and analyse the image
            HttpResponseMessage rawResults = await _httpClient.GetAsync(requestUrl);
            List<Box> boxList = WallHelper.ConvertToBoxes(await rawResults.Content.ReadAsStringAsync());
            string jsonResults = JsonSerializer.Serialize(boxList);

            // Create a unique file name for the JSON file, save
            var jsonFileName = Guid.NewGuid().ToString() + ".json";
            var jsonFilePath = Path.Combine(_environment.WebRootPath, "json", jsonFileName);
            await System.IO.File.WriteAllTextAsync(jsonFilePath, jsonResults);

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
            return Ok();
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