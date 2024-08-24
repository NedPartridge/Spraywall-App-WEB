namespace SpraywallAppWeb.Models;

// A class which represents log in data - json data in log in requests is deserialised to
// an object of this type.
//
// Distinct from the 'User' class because not all of the 'User''s fields are required by the 
// login process.
public class UserToLogin
{
    // User's email
    public string Email { get; set; }

    // The hash of the user's password
    public string Password { get; set; }
}