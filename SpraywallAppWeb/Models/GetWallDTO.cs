namespace SpraywallAppWeb.Models;

// Data transfer object (DTO) for the getwall endpoint,
// which facilitates the transfer of wall name, id, image, and identified holds.
public class GetWallDTO
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string ImagePath { get; set; }
    public string IdentifiedHoldsJsonPath { get; set; }
}