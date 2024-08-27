namespace SpraywallAppWeb.Models;

// Class to encapsulate bounding box information.
// A bounding box represents an area of pixels in which a hold might be present,
// as determined by the YOLO image recognition model.
public class Box
{
    public float X1 { get; set; }
    public float Y1 { get; set; }
    public float X2 { get; set; }
    public float Y2 { get; set; }
}
