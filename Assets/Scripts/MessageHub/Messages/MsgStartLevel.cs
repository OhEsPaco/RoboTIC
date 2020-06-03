using UnityEngine;

public class MsgStartLevel
{
    public LevelData levelData;
    public GameObject mapParent;

    public MsgStartLevel(LevelData levelData, GameObject mapParent)
    {
        this.levelData = levelData;
        this.mapParent = mapParent;
    }
}