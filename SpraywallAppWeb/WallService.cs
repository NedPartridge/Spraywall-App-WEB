using Python.Runtime;
using SpraywallAppWeb.Models;
using System.Diagnostics;
using System.Text.Json;

namespace SpraywallAppWeb.Services;

// Utility class for identifying holds on a spraywall
// 
// Actual identification is done by a python script, because 
// python interfaces better than c# with the model (yolo).
// 
// Returns a json object representing the wall's hold configuration.
static class WallService
{
    static WallService()
    {
        // Set DLL location: required by .net > 3.0 fsr.
        // Acceptable that this is hardcoded, because the solution will 
        // only be deployed to the one device.
        Runtime.PythonDLL = @"C:\Users\nedfp\AppData\Local\Programs\Python\Python312\python312.dll";
    }


    public static string IdentifyHolds(string imagePath)
    {
        // Initialize the Python runtime
        PythonEngine.Initialize();

        try
        {
            // Ensure operation are within the Python GIL
            using (Py.GIL())
            {
                dynamic sys = Py.Import("sys");

                // Add hold identification library to path
                // Static path is acceptable, because the server is only deployed to the one device
                // If the scope changes, and environment variables file could be created.
                sys.path.append(@"C:\Users\nedfp\OneDrive\Desktop\Ned\Programming\Software Development\Criterion 6\Spraywall-App-WEB\SpraywallAppWeb\Python");

                // Import the hold identification script
                dynamic detect_holds = Py.Import("HoldRecognition");

                // Call the Python function - not json yet, python is weitd.
                string stringOutput = detect_holds.detect_holds(imagePath).ToString();

                // Convert the output to a list of Box objects - easier to serialise
                List<Box> boxes = ConvertToBoxes(stringOutput);
                string jsonOutput = JsonSerializer.Serialize(boxes, new JsonSerializerOptions { WriteIndented = true });
                return jsonOutput;
            }
        }
        // 'Deal' with errors
        catch (Exception ex)
        {
            return "error";
        }
    }

    // Convert python's output to a set of 'boxes',
    // which are bounding regions (4 vertices, rectangular) which represent 
    // the area a hold might be.
    public static List<Box> ConvertToBoxes(string stringOutput)
    {
        var boxes = new List<Box>();

        // clean python's trash
        string cleanedOutput = stringOutput.Replace("[", "").Replace("]", "").Trim();
        string[] coordinates = cleanedOutput.Split(new[] { " ", "\n", "\r" }, StringSplitOptions.RemoveEmptyEntries);

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