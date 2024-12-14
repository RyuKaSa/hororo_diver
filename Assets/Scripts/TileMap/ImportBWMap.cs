using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine.Tilemaps;
using UnityEngine;

public class ImportBWMap : MonoBehaviour
{
    public string filePath = "Assets/Visual/Maps/processed.png";

    private Texture2D mapTexture;
    public Tile wallTile;
    private Tilemap tilemap;

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


        // Texture2D tex = LoadTexture(filePath);
        Debug.Log(mapTexture);
        Debug.Log(mapTexture.width);
        Debug.Log(mapTexture.height);

        Color[] colors = mapTexture.GetPixels(0);

        Debug.Log(colors);

        Vector3Int currentCell = tilemap.WorldToCell(transform.position);
        Vector3Int[] positions = new Vector3Int[mapTexture.width * mapTexture.height];
        TileBase[] tileArray = new TileBase[mapTexture.width * mapTexture.height];
        for(int y=0; y <mapTexture.height; y++) {
            for(int x=250; x <mapTexture.width-250; x++) {
                currentCell.x = x;
                currentCell.y = y;
                Debug.Log("" + x + " " + y);
                positions[x+y*mapTexture.width] = new Vector3Int(x, y, 0);
                if(colors[x+y*mapTexture.width].r == 1.0) {
                    tileArray[x+y*mapTexture.width] = wallTile;
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
        Vector3Int currentCell = tilemap.WorldToCell(transform.position);
        currentCell.x += 1;
        tilemap.SetTile(currentCell, wallTile);
    }
}
