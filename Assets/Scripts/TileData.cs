[System.Serializable]
public class GroundTilePos
{
    public int x;
    public int y;
}

[System.Serializable]
public class GroundMapData
{
    public GroundTilePos[] ground;
    public GroundTilePos[] boxes;  // �� �Y�ꂸ�ɒǉ��I
    public GroundTilePos[] goal;  // �� �Y�ꂸ�ɒǉ��I
}
