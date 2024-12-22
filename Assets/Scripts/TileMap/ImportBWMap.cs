using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine.Tilemaps;
using UnityEngine;

public class ImportBWMap : MonoBehaviour
{
    public string filePath = "Assets/Visual/Maps/processed_3_lvl.png";
    public TileBase ruleTiles;

    private Texture2D mapTexture;
    private Tilemap tilemap;

    private int offset;

    void Start()
    {
        // Get the Tilemap component
        tilemap = GetComponent<Tilemap>();
        Debug.Log("Start OK");
        Debug.Log(tilemap);

        // Load the image into a Texture2D
        byte[] fileData;
        if (File.Exists(filePath))
        {
            fileData = File.ReadAllBytes(filePath);
            mapTexture = new Texture2D(2, 2);
            mapTexture.LoadImage(fileData); // This will auto-resize the texture dimensions.
        }

        Debug.Log(mapTexture);
        Debug.Log($"Texture Width: {mapTexture.width}, Height: {mapTexture.height}");

        // Read pixel data
        Color[] colors = mapTexture.GetPixels(0);
        Debug.Log($"Total Pixels: {colors.Length}");

        // Define array sizes based on texture size
        Vector3Int[] positions = new Vector3Int[mapTexture.width * mapTexture.height];
        TileBase[] tileArray = new TileBase[mapTexture.width * mapTexture.height];

        // Iterate through all pixels
        for (int y = 0; y < mapTexture.height; y++)
        {
            for (int x = 0; x < mapTexture.width; x++)
            {
                int index = x + y * mapTexture.width;
                positions[index] = new Vector3Int(x - (mapTexture.width / 2), y - (mapTexture.height / 2), 0);

                if (colors[index].r > 0.5f)
                {
                    tileArray[index] = ruleTiles;
                }
                else
                {
                    tileArray[index] = null;
                }
            }
        }

        // Apply the tiles to the Tilemap
        tilemap.SetTiles(positions, tileArray);
        Debug.Log("Tilemap successfully updated from the image.");
    }
}
