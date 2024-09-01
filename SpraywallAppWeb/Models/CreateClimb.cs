namespace SpraywallAppWeb.Models;
public class CreateClimb
{
    public string? Name { get; set; }
    public string? SetterName { get; set; } // static string - changing user's name wont effect. This is fine.
    public int? Grade { get; set; } // assigned by setter
    public int Attempts { get; set; } 
    
    // json data representing the holds in a climb
    public string? JsonHolds { get; set; }
}
