# Using YOLO, and the model taken from 
# https://github.com/mkurc1/climbingcrux_model?tab=readme-ov-file,
# identify holds on a given image of a spraywall

from fastapi import FastAPI, HTTPException, Query
from fastapi.responses import JSONResponse
from ultralytics import YOLO
import cv2
import re
import os

app = FastAPI()

# Load the YOLO model
model = YOLO(r"C:\Users\nedfp\OneDrive\Desktop\Ned\Programming\Software Development\Criterion 6\Spraywall-App-WEB\SpraywallAppWeb\Python\Model\best.pt")

# Regular expression for validating file names (alphanumeric + underscores + dashes)
# Unvalidated file names would allow directory traversal (security risk)
# Wall names from the desktop app are generated unique names, so should never
# return an error there.
FILE_NAME_REGEX = re.compile(r'^[\w\-.]+$')
IMAGE_DIR = r"C:\Users\nedfp\OneDrive\Desktop\Ned\Programming\Software Development\Criterion 6\Spraywall-App-WEB\SpraywallAppWeb\wwwroot\images"

# Endpoint to analyze an image using the YOLO model
#
# Takes the name of the image to analyse, assuming it's stored in the correct directory,
# and returns the set of bounding boxes which represent the holds.
@app.get("/identify-holds")
async def identify_holds(fileName: str):
    try:
    # Validate inputs
        if not FILE_NAME_REGEX.match(fileName):
            raise HTTPException(status_code=400, detail="Invalid file name")

        file_path = os.path.join(IMAGE_DIR, fileName)

        # Check if file exists
        if not os.path.exists(file_path):
            raise HTTPException(status_code=404, detail="File not found")
        
        
        # Ensure file path is within the allowed directory
        if not os.path.abspath(file_path).startswith(os.path.abspath(IMAGE_DIR)):
            raise HTTPException(status_code=403, detail="Access to the specified file is forbidden")

        # Read the image file
        img = cv2.imread(file_path)

        # Check if the image was loaded properly
        if img is None:
            raise HTTPException(status_code=500, detail="Error loading image")

    # Analyse image
        # Convert BGR to RGB
        img_rgb = cv2.cvtColor(img, cv2.COLOR_BGR2RGB)

        # Perform inference
        results = model(img_rgb)

        if isinstance(results, list) and len(results) > 0:
            result = results[0]  # Get the first result if there are multiple
            # Process and return bounding boxes
            boxes = result.boxes
            boxes_xyxy = boxes.xyxy.cpu().numpy().tolist()
            return JSONResponse(content=boxes_xyxy, status_code=200)
        else:
            raise HTTPException(status_code=500, detail="Unexpected results format")

    except Exception as e:
        raise HTTPException(status_code=500, detail=str(e))

if __name__ == "__main__":
    import uvicorn
    uvicorn.run(app, host="0.0.0.0", port=8000)
