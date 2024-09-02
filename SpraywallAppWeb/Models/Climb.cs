namespace SpraywallAppWeb.Models;
public class Climb
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string SetterName { get; set; }
    public int Grade { get; set; } // assigned by setter
    // json data representing the holds in a climb
    public string JsonHolds { get; set; }

    // The users that have logged the climb - through a join table to permit attempt tracking.
    public ICollection<UserClimb> UserClimbs { get; set; } = new List<UserClimb>();

    // The wall the climb is on
    public int WallID { get; set; } 
    public Wall Wall { get; set; }

    // Whether the wall has been identified as problematic
    public bool Flagged { get; set; }
}
