using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SpraywallAppWeb.Data;
using SpraywallAppWeb.Helpers;
using SpraywallAppWeb.Models;
using System.Diagnostics;
using System.IO;
using System.Security.Claims;
using System.Text.Json;
using System.Text.RegularExpressions;

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

    // Create a wall, and add it to the database: authorization required (inherits from controller)
    // 
    // Request consists of a wall name, and an image of the spraywall.
    //
    // Contacts a python api (built & hosted by yours truly) to analyse the uploaded image files
    [HttpPost]
    public async Task<IActionResult> CreateWall([FromForm] CreateWallDto dto)
    {
        // Create a db context
        using (UserContext context = await DbContextFactory.CreateDbContextAsync())
        {
            // Get the requesting user's id from the security token
            // Will only send bad req to desktop/mobile user if they've timed out, 
            // this is mostly to protect the api    
            int userId;
            string userIdString = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdString == null) // existence check 
            { 
                return BadRequest("Invalid credentials"); 
            }
            try 
            {
                // Type check
                userId = Convert.ToInt32(userIdString);
                // Range check - is it a real userid?
                if (!context.Users.Select(x => x.Id).Any()) 
                { 
                    return BadRequest("Invalid credentials"); 
                }
            }
            catch (Exception ex) { return BadRequest("Invalid credentials"); }

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

            // Assign the wall to be managed by the creating user.
            User user = context.Users.FirstOrDefault(u => u.Id == userId);
            user.ManagedWalls.Add(wall);

            await context.SaveChangesAsync();

            // Send the holds back to the user, so they can view
            return Ok();
        }
    }



    // For a given wall, overwrite the old name, image with new name, image
    //
    // The image is not lost, in compliance with data storage laws
    // A new image is added, and the reference changed.
    [HttpPost("updatewall/{id}")]
    public async Task<IActionResult> UpdateWall(int id, [FromForm] CreateWallDto wallDto)
    {
        using (UserContext context = await DbContextFactory.CreateDbContextAsync())
        {
            // Validate wall exists, and that user is authourised to access it (is the manager)
            int userId;
            string userIdString = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdString == null) // existence check 
            {
                return BadRequest("Invalid credentials");
            }
            try
            {
                // Type check - exits to catch block on a fail
                userId = Convert.ToInt32(userIdString);
                // Range check - is it a real userid?
                if (!context.Users.Select(x => x.Id).Any())
                {
                    return BadRequest("Invalid credentials");
                }
            }
            catch { return BadRequest("Invalid credentials"); }

            if (wallDto == null) // existence check
                return BadRequest();

            // Get the wall stored in the db
            Wall storedWall = context.Walls.FirstOrDefault(x => x.Id == id);

            // Ensure wall exists
            if(storedWall == null)
                return NotFound();
            // Confirm user has edit access
            if (storedWall.ManagerID != userId)
                return BadRequest("Invalid credentials");
            // Confirm wall name isn't ass: existence check
            if (wallDto.Name == null)
                return BadRequest("A name is required. Stoopid.");
            // Confirm wall image data isn't null/empty: range, existence check
            if (wallDto.Image == null || wallDto.Image.Length == 0)
                return BadRequest("Image is required");
            // Confirm wall name isn't in use: range check
            if (context.Walls.Select(x => x.Name).Contains(wallDto.Name))
                return BadRequest("Wall name is in use");


            // Success! :D
            // Update the wall

            // Create a unique file name for the image
            var newImageFileName = Guid.NewGuid().ToString() + Path.GetExtension(wallDto.Image.FileName);
            var newImageFilePath = Path.Combine(_environment.WebRootPath, "images", newImageFileName);
            storedWall.ImagePath = "/images/" + newImageFileName;
            // Save the image to the server
            using (var stream = new FileStream(newImageFilePath, FileMode.Create))
            {
                await wallDto.Image.CopyToAsync(stream);
            }
            await context.SaveChangesAsync();

            // Analyse and save a json representation of the hold positions
            // Get the file name back from path
            var match = Regex.Match(storedWall.IdentifiedHoldsJsonPath, @"/json/(?<filename>[^/]+)");
            string jsonFileName = match.Groups["filename"].Value;

            // Analyse and save a json representation of the hold positions
            // Get the file name back from path
            match = Regex.Match(storedWall.ImagePath, @"/images/(?<filename>[^/]+)");
            string imageFileName = match.Groups["filename"].Value;

            // Construct the full URL with the fileName as a query parameter
            // Hardcoding is ok - only the one device, on api, yada yada - see solution requirements
            string baseUrl = "http://localhost:8000";
            string requestUrl = $"{baseUrl}/identify-holds?fileName={Uri.EscapeDataString(imageFileName)}";

            //// Contact the python api and analyse the image
            HttpResponseMessage rawResults = await _httpClient.GetAsync(requestUrl);
            List<Box> boxList = WallHelper.ConvertToBoxes(await rawResults.Content.ReadAsStringAsync());
            string jsonResults = JsonSerializer.Serialize(boxList);

            // Write the new json data over the old
            var jsonFilePath = Path.Combine(_environment.WebRootPath, "json", jsonFileName);
            await System.IO.File.WriteAllTextAsync(jsonFilePath, jsonResults);

            // Update the wall object: json path stays the same, just the name here
            storedWall.Name = wallDto.Name;
            await context.SaveChangesAsync();

            // Return response: 
            return Ok();
        }
    }


    // Return data related to a wall, given it's ID
    // Authorisation required.
    [HttpGet("getwall/{id}")]
    public async Task<IActionResult> GetWall(int id)
    {
        using (UserContext context = await DbContextFactory.CreateDbContextAsync())
        {
            // Validate wall exists, and that user is authourised to access it (is the manager)
            int userId;
            string userIdString = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdString == null) // existence check 
            {
                return BadRequest("Invalid credentials");
            }
            try
            {
                // Type check - exits to catch block on a fail
                userId = Convert.ToInt32(userIdString);
                // Range check - is it a real userid?
                if (!context.Users.Select(x => x.Id).Any())
                {
                    return BadRequest("Invalid credentials");
                }
            } catch { return BadRequest("Invalid credentials"); }

            Wall wall = await context.Walls.FindAsync(id);
            if (wall == null) // exist check
                return NotFound();
            if(wall.ManagerID != userId)
                return BadRequest("Invalid credentials");

            // Set path variables, from db values
            string jsonFilePath = _environment.WebRootPath + wall.IdentifiedHoldsJsonPath;
            string imageFilePath = _environment.WebRootPath + wall.ImagePath;

            // Load the image file (the wall)
            FileStream imageFileStream = new FileStream(imageFilePath, FileMode.Open, FileAccess.Read);
            FileStreamResult imageFileResult = new FileStreamResult(imageFileStream, "image/jpeg") // or "image/png" based on file type
            {
                FileDownloadName = Path.GetFileName(imageFilePath)
            };

            // load the json file containing the holds
            var jsonFileContent = await System.IO.File.ReadAllTextAsync(jsonFilePath);

            // Image file cannot be sent along with json data;
            // To avoid splitting the request across multiple endpoints, encode and send.
            byte[] imageBytes = await System.IO.File.ReadAllBytesAsync(imageFilePath);
            string base64Image = Convert.ToBase64String(imageBytes);

            // Return response: 
            return Ok(new
            {
                wall.Id,
                wall.Name,
                Image = base64Image,
                JsonFile = jsonFileContent
            });
        }
    }


    // Return the name of the wall with the given ID.
    // No authorization required
    [AllowAnonymous]
    [HttpGet("getwallname/{id}")]
    public async Task<IActionResult> GetWallName(int id)
    {
        Debug.WriteLine("Id: " + id);

        using (UserContext context = await DbContextFactory.CreateDbContextAsync())
        {
            if (!context.Walls.Any(x => x.Id == id)) // Wall not found
                return NotFound();
            return Ok(context.Walls.FirstOrDefault(x => x.Id == x.Id).Name); // Wall found: return the name
        }
    }
}