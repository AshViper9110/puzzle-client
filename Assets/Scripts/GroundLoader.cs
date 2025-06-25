using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GroundLoader : MonoBehaviour
{
    public Tilemap tilemap;
    public Tile groundTile;
    public GameObject boxPrefab;
    public GameObject goalPrefab;

    void Start()
    {
        tilemap.ClearAllTiles();
        GenerateGroundFrameCentered();
        LoadGroundMap();
    }

    void LoadGroundMap()
    {
        int jsonFileID = GameMane.CurrentStageID;  // GameMane‚©‚çID‚ðŽæ“¾
        string path = "Json/stage" + jsonFileID;
        TextAsset jsonText = Resources.Load<TextAsset>(path);
        if (jsonText == null)
        {
            Debug.LogError("JSONƒtƒ@ƒCƒ‹‚ªŒ©‚Â‚©‚è‚Ü‚¹‚ñ: " + path);
            return;
        }

        GroundMapData mapData = JsonUtility.FromJson<GroundMapData>(jsonText.text);

        foreach (GroundTilePos pos in mapData.ground)
        {
            Vector3Int cellPos = new Vector3Int(pos.x, pos.y, 0);
            tilemap.SetTile(cellPos, groundTile);
        }

        if (mapData.boxes != null && boxPrefab != null)
        {
            foreach (GroundTilePos box in mapData.boxes)
            {
                Vector3Int cellPos = new Vector3Int(box.x, box.y, 0);
                Vector3 worldPos = tilemap.CellToWorld(cellPos) + tilemap.tileAnchor;
                Instantiate(boxPrefab, worldPos, Quaternion.identity);
            }
        }

        if (mapData.goal != null && goalPrefab != null)
        {
            foreach (GroundTilePos goal in mapData.goal)
            {
                Vector3Int cellPos = new Vector3Int(goal.x, goal.y, 0);
                Vector3 worldPos = tilemap.CellToWorld(cellPos) + tilemap.tileAnchor;
                Instantiate(goalPrefab, worldPos, Quaternion.identity);
            }
        }
    }

    void GenerateGroundFrameCentered()
    {
        int width = 17;
        int height = 11;

        int xMin = (-width / 2) - 1;
        int xMax = width / 2;
        int yMin = -height / 2;
        int yMax = height / 2;

        tilemap.ClearAllTiles();

        for (int x = xMin; x <= xMax; x++)
        {
            for (int y = yMin; y <= yMax; y++)
            {
                if (x == xMin || x == xMax || y == yMin || y == yMax)
                {
                    tilemap.SetTile(new Vector3Int(x, y, 0), groundTile);
                }
            }
        }
    }
}
