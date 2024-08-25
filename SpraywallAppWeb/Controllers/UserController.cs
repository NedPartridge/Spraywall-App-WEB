using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SpraywallAppWeb.Data;
using SpraywallAppWeb.Helpers;
using SpraywallAppWeb.Models;
using SpraywallAppWeb.Services;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;

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
        // If email is not valid, reject before committing to further processing
        if (!UserHelper.IsValidEmail(userToSignup.Email))
            return BadRequest("Email is invalid");

        // Create an instance of the usercontext, which is destroyed on exiting the user block
        // This pattern prevents conflicts between endpoints using the same context
        using(UserContext context = await DbContextFactory.CreateDbContextAsync())
        {
            // Duplicate addresses are not allowed
            if(context.Users.Select(x => x.Email).Contains(userToSignup.Email))
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
}


internal static class UserHelper
{
    // Validate an email address
    // TODO: Confirm email exists (external service?)
    internal static bool IsValidEmail(string email)
    {
        var trimmedEmail = email.Trim();

        if (trimmedEmail.EndsWith("."))
        {
            return false;
        }
        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == trimmedEmail;
        }
        catch
        {
            return false;
        }
    }
}