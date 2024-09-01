namespace SpraywallAppWeb.Models;

// A class which represents sign up data - json data in signup requests is deserialised to
// an object of this type.
//
// Distinct from the 'User' class because not all of the 'User''s fields are provided by the 
// signup process.
public class UserToSignup
{
    // Personal details
    public string? Name { get; set; }
    public string? Email { get; set; }

    // A hashed password - the password is stored hashed, too, but may be
    // decrypted by the passwordencryption.decrypt method
    public string? Password { get; set; }
}