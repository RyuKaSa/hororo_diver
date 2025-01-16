from PIL import Image
import numpy as np
from scipy.ndimage import convolve
import json

# Load the image
image_path = "Assets\Visual\Maps\processed.bmp"  # Replace with your image file path
image = Image.open(image_path)

# Convert the image to grayscale and binarize it (black/white threshold)
gray_image = image.convert("L")  # Convert to grayscale
binary_image = np.array(gray_image) < 128  # Threshold to get binary (black/white)

# Define Sobel kernels for edge detection
sobel_x = np.array([[-1, 0, 1],
                    [-2, 0, 2],
                    [-1, 0, 1]])
sobel_y = np.array([[-1, -2, -1],
                    [ 0,  0,  0],
                    [ 1,  2,  1]])

# Apply Sobel filters to detect edges
edge_x = convolve(binary_image.astype(np.int16), sobel_x)
edge_y = convolve(binary_image.astype(np.int16), sobel_y)
edges = np.hypot(edge_x, edge_y) > 0  # Combine edges from both directions

# Convert to binary (1 for edges, 0 otherwise)
edge_array = edges.astype(np.uint8)

# Save the edge array to a .txt file
output_txt_path = "Assets\Visual\Maps\extracted_edges.txt"  # Replace with your desired file path
np.savetxt(output_txt_path, edge_array, fmt='%d', delimiter='')

print(f"Edges saved to: {output_txt_path}")

# Convert the 2D edge array to a JSON file
output_json_path = "Assets\Visual\Maps\edges.json"  # Replace with your desired file path

# Create a dictionary to store the array
data = {"edges": edge_array.tolist()}  # Convert NumPy array to a regular list

# Save the dictionary as a JSON file
with open(output_json_path, "w") as json_file:
    json.dump(data, json_file)

print(f"Edges saved to JSON: {output_json_path}")