namespace SpraywallAppWeb.Models;

// 'join' class between users and climbs - allows the attempt functionality.
public class UserClimb
{
    public int Id { get; set; }

    // User ref
    public int UserId { get; set; }
    public User User { get; set; }

    // attempt ref
    public int ClimbId { get; set; }
    public Climb Climb { get; set; }

    // Number of user's attempts to send
    public int NumberOfAttempts { get; set; }
}