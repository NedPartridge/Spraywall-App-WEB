# Using YOLO, and the model taken from 
# https://github.com/mkurc1/climbingcrux_model?tab=readme-ov-file,
# identify holds on a given image of a spraywall


import sys
from ultralytics import YOLO
import cv2
from matplotlib import pyplot as plt

def detect_holds(img_path):
    # Load the model
    model = YOLO(r"C:\Users\nedfp\OneDrive\Desktop\Ned\Programming\Software Development\Criterion 6\Spraywall-App-WEB\SpraywallAppWeb\Python\Model\best.pt")

    # Load the image
    img = cv2.imread(img_path)
    img_rgb = cv2.cvtColor(img, cv2.COLOR_BGR2RGB)

    # Perform inference
    results = model(img_rgb)

    if isinstance(results, list) and len(results) > 0:
        result = results[0]  # Get the first result if there are multiple

        # Process and return bounding boxes
        boxes = result.boxes
        return boxes.xyxy.cpu().numpy()
            
    else:
        print("Unexpected results format:", results)

if __name__ == "__main__":
    if len(sys.argv) > 1:
        detect_holds(sys.argv[1])
    else:
        print("No image path provided.")
