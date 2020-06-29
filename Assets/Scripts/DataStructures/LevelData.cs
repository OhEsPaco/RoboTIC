using System.Collections.Generic;

[System.Serializable]
public class LevelData
{
    public string levelName;
    public List<int> levelSize;
    public List<int> playerPos;
    public int playerOrientation;
    public List<int> goal;
    public AvailableInstructions availableInstructions;
    public List<int> mapAndItems;

    public LevelData Clone()
    {
        LevelData clone = new LevelData
        {
            levelName = this.levelName,
            playerOrientation = this.playerOrientation,
            levelSize = CloneListInt(this.levelSize),
            playerPos = CloneListInt(this.playerPos),
            goal = CloneListInt(this.goal),
            mapAndItems = CloneListInt(this.mapAndItems)
        };

        AvailableInstructions av = new AvailableInstructions
        {
            condition = this.availableInstructions.condition,
            loop = this.availableInstructions.loop,
            turnRight = this.availableInstructions.turnRight,
            turnLeft = this.availableInstructions.turnLeft,
            jump = this.availableInstructions.jump,
            move = this.availableInstructions.move,
            action = this.availableInstructions.action
        };

        clone.availableInstructions = av;

        return clone;
    }

    private List<int> CloneListInt(List<int> original)
    {
        List<int> clone = new List<int>();
        foreach (int i in original)
        {
            clone.Add(i);
        }

        return clone;
    }
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