using System;
using System.Collections.Generic;
using UnityEngine;
using static LevelButtons;

/*public class LevelData
{
    public string levelName;
    public List<int> levelSize;
    public List<int> playerPos;
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
*/

public class EditorLogic : MonoBehaviour
{
    private LevelData newLevel = new LevelData();
    private MapEditorRoadButton[] roadButtons;

    // Start is called before the first frame update
    private void Start()
    {
        newLevel.levelName = Guid.NewGuid().ToString();
        newLevel.levelSize = new List<int>();
        newLevel.playerPos = new List<int>();
        newLevel.playerOrientation = 0;
        newLevel.goal = new List<int>();
        newLevel.availableInstructions = new AvailableInstructions();
        newLevel.mapAndItems = new List<int>();

        newLevel.availableInstructions.condition = 0;
        newLevel.availableInstructions.loop = 0;
        newLevel.availableInstructions.turnRight = 0;
        newLevel.availableInstructions.turnLeft = 0;
        newLevel.availableInstructions.jump = 0;
        newLevel.availableInstructions.move = 0;
        newLevel.availableInstructions.action = 0;

        roadButtons = FindObjectsOfType<MapEditorRoadButton>();
        foreach (MapEditorRoadButton m in roadButtons)
        {
            m.Subscribe = ProcessMapEditorRoadButton;
            Debug.Log("subscribed");
        }
    }

    public void ProcessMapEditorRoadButton(Buttons type, int numberOfInstructions)
    {
        switch (type)
        {
            case Buttons.Action:
                newLevel.availableInstructions.action = numberOfInstructions;
                break;

            case Buttons.Condition:
                newLevel.availableInstructions.condition = numberOfInstructions;
                break;

            case Buttons.Jump:
                newLevel.availableInstructions.jump = numberOfInstructions;
                break;

            case Buttons.Loop:
                newLevel.availableInstructions.loop = numberOfInstructions;
                break;

            case Buttons.Move:
                newLevel.availableInstructions.move = numberOfInstructions;
                break;

            case Buttons.Play:
                break;

            case Buttons.Restart:
                break;

            case Buttons.TurnLeft:
                newLevel.availableInstructions.turnLeft = numberOfInstructions;
                break;

            case Buttons.TurnRight:
                newLevel.availableInstructions.turnRight = numberOfInstructions;
                break;

            case Buttons.Undo:
                break;

            case Buttons.MapMenu:
                break;

            case Buttons.Undefined:
                break;
        }
    }

    // Update is called once per frame
    private void Update()
    {
    }
}