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
    public GroundTilePos[] boxes;  // Å© ñYÇÍÇ∏Ç…í«â¡ÅI
    public GroundTilePos[] goal;  // Å© ñYÇÍÇ∏Ç…í«â¡ÅI
}
