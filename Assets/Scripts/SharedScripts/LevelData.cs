using System.Collections.Generic;

[System.Serializable]
public class LevelData
{
    public string levelName;
    public List<int> levelSize;
    public List<int> playerStart;
    public int playerOrientation;
    public List<int> goal;
    public AvailableInstructions availableInstructions;
    public List<int> mapAndItems;
}

[System.Serializable]
public class AvailableInstructions
{
    public int condition;
    public int loop;
    public int turnRight;
    public int turnLeft;
    public int jump;
    public int move;
    public int action;
}

