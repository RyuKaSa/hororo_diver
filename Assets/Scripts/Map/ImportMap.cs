using System.IO;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

[System.Serializable]
public struct ColorTileMapping
{
    [Tooltip("The color to match in the image.")]
    public Color color;

    [Tooltip("The RuleTile associated with this color.")]
    public RuleTile tile;
}

public class ImportMap : MonoBehaviour
{
    [Header("Map Configuration")]
    [Tooltip("Path to the map image file.")]
    public string filePath1;
    string filePath;

    [Tooltip("Left Margin")]
    public int leftMargin = 0;
    [Tooltip("Right Margin")]
    public int rightMargin = 0;

    [Header("Color and Tile Mappings")]
    [Tooltip("Define up to 6 colors and their corresponding RuleTiles.")]
    public List<ColorTileMapping> colorTileMappings = new List<ColorTileMapping>(6);

    private Texture2D mapTexture;
    private Tilemap tilemap;

    void Start()
    {
        filePath = Path.Combine(Application.streamingAssetsPath, filePath1).Replace("\\", "/");


        // Get Tilemap component
        tilemap = GetComponent<Tilemap>();
        if (tilemap == null)
        {
            Debug.LogError("No Tilemap component found on this GameObject!");
            return;
        }

        Debug.Log("Tilemap Component Found!");

        // Load the Texture
        if (!LoadTexture())
        {
            Debug.LogError("Failed to load the map texture. Check the file path.");
            return;
        }

        Debug.Log($"Texture Loaded. Width: {mapTexture.width}, Height: {mapTexture.height}");

        Debug.Log($"Trying to load from: {filePath}");


        // Process the map into tiles
        GenerateTilemap();
    }

    /// <summary>
    /// Load the image from the file path into a Texture2D.
    /// </summary>
    private bool LoadTexture()
    {
        if (!File.Exists(filePath))
        {
            Debug.LogError($"File not found at: {filePath}");
            return false;
        }

        byte[] fileData = File.ReadAllBytes(filePath);
        mapTexture = new Texture2D(2, 2);
        if (!mapTexture.LoadImage(fileData))
        {
            Debug.LogError("Failed to load image data into Texture2D.");
            return false;
        }

        return true;
    }

    /// <summary>
    /// Generate the tilemap based on image colors.
    /// </summary>
    private void GenerateTilemap()
    {
        Color[] colors = mapTexture.GetPixels();
        Vector3Int[] positions = new Vector3Int[mapTexture.width * mapTexture.height];
        TileBase[] tileArray = new TileBase[mapTexture.width * mapTexture.height];

        for (int y = 0; y < mapTexture.height; y++)
        {
            for (int x = leftMargin; x < mapTexture.width - rightMargin; x++)
            {
                int index = x + y * mapTexture.width;
                Vector3Int position = new Vector3Int(
                    x - (mapTexture.width / 2),
                    y - (mapTexture.height / 2),
                    0
                );

                positions[index] = position;

                Color pixelColor = colors[index];
                ColorTileMapping? mapping = GetTileMapping(pixelColor);

                if (mapping.HasValue)
                {
                    tileArray[index] = mapping.Value.tile;
                    tilemap.SetColor(position, mapping.Value.color); // Apply the color mapping
                }
                else
                {
                    tileArray[index] = null; // Unrecognized color leaves the cell empty
                    Debug.LogWarning($"Unrecognized Color at ({x}, {y}): {pixelColor}");
                }
            }
        }

        // Apply the tiles to the Tilemap
        tilemap.SetTiles(positions, tileArray);
        tilemap.RefreshAllTiles();

        Debug.Log("Tilemap successfully updated from the image with applied colors.");
    }

    /// <summary>
    /// Match a pixel color to a RuleTile and return its mapping.
    /// </summary>
    private ColorTileMapping? GetTileMapping(Color pixelColor)
    {
        foreach (var mapping in colorTileMappings)
        {
            if (IsColorMatch(pixelColor, mapping.color))
            {
                return mapping;
            }
        }

        return null; // No mapping found
    }

    /// <summary>
    /// Compare two colors with a tolerance to account for compression artifacts.
    /// </summary>
    private bool IsColorMatch(Color a, Color b, float tolerance = 0.03f)
    {
        return Mathf.Abs(a.r - b.r) < tolerance &&
               Mathf.Abs(a.g - b.g) < tolerance &&
               Mathf.Abs(a.b - b.b) < tolerance;
    }
}
