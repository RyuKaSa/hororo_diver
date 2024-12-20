using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine.Tilemaps;
using UnityEngine;

public class ImportBWMap : MonoBehaviour
{
    public string filePath = "Assets/Visual/Maps/processed_3_lvl.png";
    public TileBase ruleTiles;

    public int yMin;
    public int yMax;

    private Texture2D mapTexture;
    private Tilemap tilemap;

    private int offset;

    // Start is called before the first frame update
    void Start()
    {
        tilemap = GetComponent<Tilemap>();
        Debug.Log("Start OK");
        Debug.Log(tilemap);

        mapTexture = new Texture2D(1000, 1000, TextureFormat.BGRA32,false);
        byte[] fileData;
        if (File.Exists(filePath)) 	{
            fileData = File.ReadAllBytes(filePath);
            mapTexture = new Texture2D(2, 2);
            mapTexture.LoadImage(fileData); //..this will auto-resize the texture dimensions.
        }

        offset = mapTexture.width/5;


        // Texture2D tex = LoadTexture(filePath);
        Debug.Log(mapTexture);
        Debug.Log(mapTexture.width);
        Debug.Log(mapTexture.height);

        Color[] colors = mapTexture.GetPixels(0);

        Debug.Log(colors);

        Vector3Int currentCell = tilemap.WorldToCell(transform.position);
        Vector3Int[] positions = new Vector3Int[mapTexture.width * mapTexture.height];
        TileBase[] tileArray = new TileBase[mapTexture.width * mapTexture.height];
        for(int y=yMin; y < yMax; y++) {
            Debug.Log("Line " + y);
            for(int x=offset; x <mapTexture.width-offset; x++) {
                currentCell.x = x;
                currentCell.y = y;
                // Debug.Log("" + x + " " + y);
                positions[x+y*mapTexture.width] = new Vector3Int(x-(mapTexture.width/2), y-yMin, 0);
                if(colors[x+y*mapTexture.width].r > 0.5) {
                    // tileArray[x+y*mapTexture.width] = wallTile;
                    tileArray[x+y*mapTexture.width] = ruleTiles;
                }
                else {
                    tileArray[x+y*mapTexture.width] = null;
                }
            }
        }

        tilemap.SetTiles(positions, tileArray);

    }

    // Update is called once per frame
    void Update()
    {
        // Vector3Int currentCell = tilemap.WorldToCell(transform.position);
        // currentCell.x += 1;
        // tilemap.SetTile(currentCell, ruleTiles);
    }
}
