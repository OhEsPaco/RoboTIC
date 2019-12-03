using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Logic : MonoBehaviour
{
    private LevelManager manager;
    private bool dirty;
    private LevelData levelData;
    // Start is called before the first frame update
    void Start()
    {
        manager = LevelManager.instance;
        manager.Logic = this;
        dirty = true;
    }

    // Update is called once per frame
    void Update()
    {
        
        if (dirty)
        {

            dirty = false;
            levelData = manager.JSonLoader.LoadData(GetLevelPath());
            Debug.Log(levelData.levelName);

            manager.MapRenderer.RenderMapAndItems(levelData);
            manager.MapRenderer.RenderScenery(levelData);
            manager.MapRenderer.RenderMainCharacter(levelData);

            SetAvailableInstructions(levelData);
           
        }
    }
    private void SetAvailableInstructions(LevelData data)
    {
        LevelButtons lButtonsScript = manager.LevelButtons;
        lButtonsScript.setNumber(ButtonConstants.Action, data.availableInstructions.action);
        lButtonsScript.setNumber(ButtonConstants.Condition, data.availableInstructions.condition);
        lButtonsScript.setNumber(ButtonConstants.Jump, data.availableInstructions.jump);
        lButtonsScript.setNumber(ButtonConstants.Loop, data.availableInstructions.loop);
        lButtonsScript.setNumber(ButtonConstants.Move, data.availableInstructions.move);
        lButtonsScript.setNumber(ButtonConstants.TurnLeft, data.availableInstructions.turnLeft);
        lButtonsScript.setNumber(ButtonConstants.TurnRight, data.availableInstructions.turnRight);
    }
    private string GetLevelPath()
    {
        return "Assets/StoryLevels/test.json";
    }
}
