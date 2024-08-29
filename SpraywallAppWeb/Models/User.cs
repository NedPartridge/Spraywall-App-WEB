namespace SpraywallAppWeb.Models;


// 'USER' class
// Holds all information about the user, and their relationship with other data structures
// The one user class is used for both the management and viewing applications -
// depending on which app is being used, the respective data will be returned.
public class User
{
    // A unique identifier to differentiate between users
    public int Id { get; set; }

    // Personal details
    public string Name { get; set; }
    public string Email { get; set; }

    // The user's hashed password - may be decrypted by the passwordencryption.decrypt method
    public string Password { get; set; }


    // Relationships: represent relation data between objects, for the EF
    // One user may manage many walls
    public ICollection<Wall> ManagedWalls { get; set; } = new List<Wall>();
}
