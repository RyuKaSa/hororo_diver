using System.IO;
using UnityEngine;

public class GetEdgesBWMap : MonoBehaviour
{
    private Texture2D blackAndWhiteImage; // Assign your image in the inspector
    public string filePath = "Assets/Visual/Maps/processed_3_lvl.png";
    private float tileSize = 1f; // Size of each tile in Unity units
    static public int[,] grid;
    static public bool confirmEdge = false;

    private void Start()
    {
        
    }

    private void Update() {
        if (ImportBWMap.confirmImport && !confirmEdge) {
            blackAndWhiteImage = ImportBWMap.mapTexture;
            grid = new int[blackAndWhiteImage.width, blackAndWhiteImage.height];
            ExtractEdges();
            confirmEdge = true;
        }
    }

    void ExtractEdges()
    {
        int width = blackAndWhiteImage.width;
        int height = blackAndWhiteImage.height;
        Color[] pixels = blackAndWhiteImage.GetPixels();
        
        // Loop through each pixel to find edges
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                if (IsBlack(x, y, width, pixels))
                {
                    // Check neighbors for edges
                    if (!IsBlack(x - 1, y, width, pixels)) grid[x, y] = 1; // Left
                    else if (!IsBlack(x + 1, y, width, pixels)) grid[x, y] = 1; // Right
                    else if (!IsBlack(x, y - 1, width, pixels)) grid[x, y] = 1; // Bottom
                    else if (!IsBlack(x, y + 1, width, pixels)) grid[x, y] = 1; // Top
                }
            }
        }
    }

    bool IsBlack(int x, int y, int width, Color[] pixels)
    {
        // Check if the pixel is within bounds and black
        if (x >= 0 && x < width && y >= 0 && y < blackAndWhiteImage.height)
        {
            return pixels[y * width + x].grayscale < 0.5f; // Grayscale threshold
        }
        return false; // Out of bounds is considered white
    }

    void CheckAndDrawEdge(int x1, int y1, int x2, int y2, int width, int height, Color[] pixels)
    {
        if (!IsBlack(x2, y2, width, pixels)) // If the neighbor is not black, it's an edge
        {
            // Draw or store the edge (adjust for tile size)
            Vector3 start = new Vector3(x1 * tileSize - width/2, y1 * tileSize - height/2, 0);
            Vector3 end = new Vector3(x2 * tileSize - width/2, y2 * tileSize - height/2, 0);
            grid[x1, y1] = 1;
            Debug.DrawLine(start, end, Color.red, 100f); // Draw line for visualization
            return;
        }
        grid[x1, y1] = 0;
    }

    void PrintEdgeMap()
    {
        int width = grid.GetLength(0);
        int height = grid.GetLength(1);

        for (int y = 0; y < height; y++)
        {
            string row = "";
            for (int x = 0; x < width; x++)
            {

                row += grid[x, y] + " ";
            }
            Debug.Log(row);
        }
    }

    void OnDrawGizmos()
    {
        if (grid == null)
        {
            Debug.LogWarning("Grid is null. Ensure it is initialized.");
            return;
        }

        Gizmos.color = Color.red;

        Debug.Log("gizmos");

        for (var y = 0; y < grid.GetLength(1); ++y)
        {
            for (var x = 0; x < grid.GetLength(0); ++x)
            {
                if (grid[x, y] == 1)
                {
                    var position = new Vector3(x * tileSize - blackAndWhiteImage.width/2, y * tileSize - blackAndWhiteImage.height/2, 0);
                    Gizmos.DrawSphere(position, 0.1f);
                }
            }
        }
    }

}
