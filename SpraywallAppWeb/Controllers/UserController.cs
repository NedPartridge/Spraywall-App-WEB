using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SpraywallAppWeb.Data;
using SpraywallAppWeb.Helpers;
using SpraywallAppWeb.Models;
using SpraywallAppWeb.Services;
using System.Diagnostics;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text.Json;

namespace SpraywallAppWeb.Controllers;


// API controller to control data related to the user
[ApiController]
[Route("[controller]")]
public class UserController : ControllerBase
{
    IDbContextFactory<UserContext> DbContextFactory;
    private readonly AuthService AuthService;

    // Initialise the controller, with a logger (for writing test/debugging information)
    //
    // Dependecy injection (DI) is used to provide the controller with a database (db) context factory - 
    // an object which creates user contexts (interfaces for the db). This behaviour is defined in program.cs
    // 
    // DI is also used to provide the authservice, which creates/stores security tokens.
    private readonly ILogger<UserController> _logger;
    public UserController(ILogger<UserController> logger,
                        IDbContextFactory<UserContext> dbContextFactory,
                        AuthService authService)
    {
        _logger = logger;
        DbContextFactory = dbContextFactory;
        AuthService = authService;
    }


    // Attempt to create a new account for the provided user
    // If succesful, return a security token containing the user's ID, valid for (1) day.
    //
    // No need to existance/type check the provided user object in the method, because this
    // step occurs automatically when deserialialising the UserToSignup.
    //
    // No authentication required.
    [HttpPost]
    [Route("signup")]
    public async Task<IActionResult> SignUpUser(UserToSignup userToSignup)
    {
        // Existence check
        if (userToSignup.Email == null | userToSignup.Password == null | userToSignup.Name == null)
            return BadRequest("No null fields");
        // If email is not valid, reject before committing to further processing
        if (!UserHelper.IsValidEmail(userToSignup.Email))
            return BadRequest("Email is invalid");

        // Create an instance of the usercontext, which is destroyed on exiting the user block
        // This pattern prevents conflicts between endpoints using the same context
        using (UserContext context = await DbContextFactory.CreateDbContextAsync())
        {
            // Duplicate addresses are not allowed
            if (context.Users.Select(x => x.Email).Contains(userToSignup.Email))
                return BadRequest("Email in use");


            // Add the user to the database
            User user = new()
            {
                Name = userToSignup.Name,
                Email = userToSignup.Email,
                Password = userToSignup.Password
            };
            context.Users.Add(user);
            await context.SaveChangesAsync();


            // Generate and return a security token for the client
            string token = AuthService.GenerateToken(user);
            return Ok(token);
        }
    }

    [HttpPost]
    [Route("login")]
    public async Task<IActionResult> LogInUser(UserToLogin userToLogin)
    {
        // Create an instance of the usercontext, which is destroyed on exiting the user block
        // This pattern prevents conflicts between endpoints using the same context
        using (UserContext context = await DbContextFactory.CreateDbContextAsync())
        {
            // If the user exists, retrieve it. If not, reject the request.
            User? user = context.Users.Where(x => x.Email == userToLogin.Email).FirstOrDefault();
            if (user == null)
                return BadRequest("Invalid credentials");

            // stored user's encrypted password
            string storedEncryptedBase64 = user.Password;
            byte[] storedEncryptedBytes = Convert.FromBase64String(storedEncryptedBase64);

            // login attempt's encrypted password
            string submittedEncryptedBase64 = userToLogin.Password;
            byte[] submittedEncryptedBytes = Convert.FromBase64String(submittedEncryptedBase64);

            // Byte arrays should be erased when comparison is finished
            byte[] storedDecryptedPassword;
            byte[] submittedDecryptedPassword;

            // Confirm decrypted passwords match
            try
            {
                storedDecryptedPassword = PasswordEncryption.Decrypt(storedEncryptedBytes);
                submittedDecryptedPassword = PasswordEncryption.Decrypt(submittedEncryptedBytes);

                // If passwords don't match, return a bad request
                bool isValidLogin = storedDecryptedPassword.SequenceEqual(submittedDecryptedPassword);
                if (!isValidLogin)
                    return BadRequest("Invalid credentials");
            }
            catch (CryptographicException ex) { return BadRequest("Don't play games with me, Kodie."); }

            // Valid login :D
            // Generate and return a security token for the client
            string token = AuthService.GenerateToken(user);
            return Ok(token);
        }
    }


    // Return the public key for encrypting passwords
    // No authentication required
    [HttpGet]
    [Route("publickey")]
    public IActionResult RetrievePublicKey()
    {
        return Ok(PasswordEncryption.PublicKeyXML);
    }

    // Return all walls managed by a given user
    [Authorize]
    [HttpGet]
    [Route("getmanagedwalls")]
    public async Task<IActionResult> GetManagedWalls()
    {
        // Create a db context
        using (UserContext context = await DbContextFactory.CreateDbContextAsync())
        {
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

            // Retrieve the list of managed walls, selecting only the necessary fields
            var walls = context.Users
                .Where(u => u.Id == userId)
                .SelectMany(u => u.ManagedWalls)
                .Select(w => new { w.Id, w.Name })
                .ToList();

            return Ok(JsonSerializer.Serialize(walls));
        }
    }


    // Return all walls the user has saved (accessed)
    [Authorize]
    [HttpGet("getsavedwalls")]
    public async Task<IActionResult> GetSavedWalls()
    {
        // Create a db context
        using (UserContext context = await DbContextFactory.CreateDbContextAsync())
        {
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

            // Retrieve the list of managed walls, selecting only the necessary fields
            var walls = context.Users
                .Include(u => u.SavedWalls)
                .Where(u => u.Id == userId)
                .SelectMany(w => w.SavedWalls)
                .Select(w => new { w.Id, w.Name })
                .ToList();

            return Ok(JsonSerializer.Serialize(walls));
        }
    }


    // Add the given wall to the set of the requesting user's saved walls
    [Authorize]
    [HttpGet("savewall/{id}")]
    public async Task<IActionResult> SaveWall(int id)
    {
        // Create a db context
        using (UserContext context = await DbContextFactory.CreateDbContextAsync())
        {
            // Get the user, and wall, confirm not ass
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


            // Update the data in the relationship - adding to one adds to both (yay EF!)
            User user = context.Users.First(u => u.Id == userId);
            Wall wall = context.Walls.FirstOrDefault(w => w.Id == id);
            if (wall == null)
                return NotFound();
            // If it's not already there, add it
            // If it is, just return and let the illiterate bumpkin on the other end feel pleased.
            if (!wall.SavedUsers.Contains(user))
                wall.SavedUsers.Add(user);
            await context.SaveChangesAsync();

            return Ok();
        }
    }

    // Edit a user's personal settings
    [Authorize]
    [HttpPost("edituser")]
    public async Task<IActionResult> EditUser(UserToSignup userToEdit)
    {
        // Create a db context, to manage stored data
        using (UserContext context = await DbContextFactory.CreateDbContextAsync())
        {
            // Get the user, confirming it's a valid request
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


            User user = context.Users.First(x => x.Id == userId);

            // Only update data if fields are provided
            if (userToEdit.Name != null)
                user.Name = userToEdit.Name;
            if (userToEdit.Email != null)
            {
                if (!UserHelper.IsValidEmail(userToEdit.Email))
                    return BadRequest("Email is invalid");
                // Duplicate addresses are not allowed
                if (context.Users.Where(u => u.Id != userId).Select(x => x.Email).Contains(userToEdit.Email))
                    return BadRequest("Email in use");
                user.Email = userToEdit.Email;
            }
            if (userToEdit.Password != null)
                user.Password = userToEdit.Password;
            // Save any changes
            await context.SaveChangesAsync();
            return Ok();
        }
    }

    // Retrieve the logged in user (from token)
    [Authorize]
    [HttpGet("getuser")]
    public async Task<IActionResult> GetUser()
    {
        // Create a db context, to manage stored data
        using (UserContext context = await DbContextFactory.CreateDbContextAsync())
        {
            // Get the user, confirming it's a valid request
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
            
            // Prepare the user to send
            var user = context.Users.Select(x => new {x.Id, x.Email, x.Name}).First(x => x.Id == userId);
            return Ok(user);
        }
    }
}