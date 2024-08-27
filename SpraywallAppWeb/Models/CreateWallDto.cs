namespace SpraywallAppWeb.Models;

// Data transfer object (DTO) for creating a wall
// Data is used to create a 'Wall'
public class CreateWallDto
{
    public string Name { get; set; }


    // Represents the uploaded image
    public IFormFile Image { get; set; }
}
