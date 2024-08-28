using SpraywallAppWeb.Models;

namespace SpraywallAppWeb.Helpers;
public static class WallHelper
{

    // Convert python's output to a set of 'boxes',
    // which are bounding regions (4 vertices, rectangular) which represent 
    // the area a hold might be.
    public static List<Box> ConvertToBoxes(string stringOutput)
    {
        var boxes = new List<Box>();

        // clean python's trash
        string cleanedOutput = stringOutput.Replace("[", "").Replace("]", "").Trim();
        string[] coordinates = cleanedOutput.Split(new[] { ",", "\n", "\r" }, StringSplitOptions.RemoveEmptyEntries);

        for (int i = 0; i < coordinates.Length; i += 4)
        {
            var box = new Box
            {
                X1 = float.Parse(coordinates[i]),
                Y1 = float.Parse(coordinates[i + 1]),
                X2 = float.Parse(coordinates[i + 2]),
                Y2 = float.Parse(coordinates[i + 3])
            };
            boxes.Add(box);
        }
        return boxes;
    }
}
