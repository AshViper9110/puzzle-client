using System.Collections.Generic;

[System.Serializable]
public class GroundTilePos
{
    public int x;
    public int y;
}

[System.Serializable]
public class GroundMapData
{
    public List<Cell> cells;
}

[System.Serializable]
public class Cell
{
    public int x;
    public int y;
    public string type;
}
