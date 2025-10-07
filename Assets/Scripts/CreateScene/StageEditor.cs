using System.Collections.Generic;
using UnityEngine;
using System.IO;

[System.Serializable]
public class CellPos
{
    public int x;
    public int y;
}

[System.Serializable]
public class StageData
{
    public List<CellPos> ground = new List<CellPos>();
    public List<CellPos> boxes = new List<CellPos>();
    public List<CellPos> goal = new List<CellPos>();
    public List<CellPos> player = new List<CellPos>();
}

public enum CellType { Empty, Ground, Box, Goal, Player }

public class StageEditor : MonoBehaviour
{
    [Header("配置用Prefab")]
    public GameObject groundPrefab;
    public GameObject boxPrefab;
    public GameObject goalPrefab;
    public GameObject playerPrefab;

    [Header("グリッド設定")]
    public float cellSize = 1f; // スナップ間隔（1でマス目）

    private StageData stageData = new StageData();
    private CellType currentTool = CellType.Ground;

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // 左クリック
        {
            Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mouseWorld.z = 0;

            // グリッドスナップ
            int gx = Mathf.RoundToInt(mouseWorld.x / cellSize);
            int gy = Mathf.RoundToInt(mouseWorld.y / cellSize);

            PlaceObject(gx, gy);
        }
    }

    void PlaceObject(int x, int y)
    {
        CellPos pos = new CellPos { x = x, y = y };

        switch (currentTool)
        {
            case CellType.Ground:
                Instantiate(groundPrefab, new Vector3(x, y, 0), Quaternion.identity);
                stageData.ground.Add(pos);
                break;

            case CellType.Box:
                Instantiate(boxPrefab, new Vector3(x, y, 0), Quaternion.identity);
                stageData.boxes.Add(pos);
                break;

            case CellType.Goal:
                Instantiate(goalPrefab, new Vector3(x, y, 0), Quaternion.identity);
                stageData.goal.Add(pos);
                break;

            case CellType.Player:
                Instantiate(playerPrefab, new Vector3(x, y, 0), Quaternion.identity);
                stageData.player.Clear(); // プレイヤーは1人
                stageData.player.Add(pos);
                break;
        }
    }

    // UIボタンから呼ぶ
    public void SetTool(int toolId)
    {
        currentTool = (CellType)toolId;
        Debug.Log("ツール切替: " + currentTool);
    }

    public void SaveStage()
    {
        string json = JsonUtility.ToJson(stageData, true);
        File.WriteAllText(Application.persistentDataPath + "/stage.json", json);
        Debug.Log("保存しました: " + Application.persistentDataPath + "/stage.json");
    }
}
