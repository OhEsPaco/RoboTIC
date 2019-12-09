using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Logic : MonoBehaviour
{
    private LevelManager manager;
    private LevelData levelData;
    public LevelData LevelData { get => levelData; }


    // Start is called before the first frame update
    private void Start()
    {
      
        levelData = manager.JSonLoader.LoadData(GetLevelPath());
        Debug.Log(levelData.levelName);
        LoadVisual();
    }
    void Awake()
    {
        manager = LevelManager.instance;
      
       
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    #region graphics
    private void LoadVisual()
    {
            manager.MapRenderer.RenderMapAndItems(levelData);
            manager.MapRenderer.RenderScenery(levelData);
            manager.MapRenderer.RenderMainCharacter(levelData);

            SetAvailableInstructions(levelData);

        
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
    #endregion

    #region buttons
    public void ButtonInput(int buttonIndex)
    {

        if(buttonIndex<0||buttonIndex>= ButtonConstants.ButtonNames.Length)
        {
            Debug.Log("Invalid button");
        }
        else
        {
            Debug.Log("Pressed " + ButtonConstants.ButtonNames[buttonIndex] + " button.");
            switch (buttonIndex)
            {
                case ButtonConstants.Action:
                    DoAction();
                    break;
                case ButtonConstants.Condition:
                    DoCondition();
                    break;
                case ButtonConstants.Jump:
                    DoJump();
                    break;
                case ButtonConstants.Loop:
                    DoLoop();
                    break;
                case ButtonConstants.Move:
                    DoMove();
                    break;
                case ButtonConstants.Play:
                    DoPlay();
                    break;
                case ButtonConstants.Restart:
                    DoRestart();
                    break;
                case ButtonConstants.TurnLeft:
                    DoTurnLeft();
                    break;
                case ButtonConstants.TurnRight:
                    DoTurnRight();
                    break;

            }
        }
       

    }

    public void NotifyEndOfAction()
    {

    }

    private void DoAction()
    {

    }

    private void DoCondition()
    {

    }

    private void DoJump()
    {

    }
    private void DoLoop()
    {

    }

    private void DoMove()
    {
        manager.ActionRenderer.DoMove();
    }

    private void DoPlay()
    {

    }
    private void DoRestart()
    {

    }

    private void DoTurnLeft()
    {

    }
    private void DoTurnRight()
    {
        
    }
    #endregion

    private string GetLevelPath()
    {
        return "Assets/StoryLevels/test.json";
    }
}
