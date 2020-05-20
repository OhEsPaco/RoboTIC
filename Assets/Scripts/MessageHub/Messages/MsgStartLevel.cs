public class MsgStartLevel
{
    public LevelData levelData;
    public LevelObject[] levelObjects;

    public MsgStartLevel(LevelData levelData, LevelObject[] levelObjects)
    {
        this.levelData = levelData;
        this.levelObjects = levelObjects;
    }
}
