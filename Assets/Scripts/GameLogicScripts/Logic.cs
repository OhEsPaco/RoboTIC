using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Logic : MonoBehaviour
{
    private LevelManager manager;
    private LevelData levelData;
    public LevelData LevelData { get => levelData; }

    private bool isActionRenderingDone = true;

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
            if (isActionRenderingDone)
            {
                Debug.Log("Pressed " + ButtonConstants.ButtonNames[buttonIndex] + " button.");
                switch (buttonIndex)
                {
                    case ButtonConstants.Action:
                        isActionRenderingDone = false;
                        DoAction();
                        break;
                    case ButtonConstants.Condition:
                        isActionRenderingDone = false;
                        DoCondition();
                        break;
                    case ButtonConstants.Jump:
                        isActionRenderingDone = false;
                        DoJump();
                        break;
                    case ButtonConstants.Loop:
                        isActionRenderingDone = false;
                        DoLoop();
                        break;
                    case ButtonConstants.Move:
                        isActionRenderingDone = false;
                        DoMove();
                        break;
                    case ButtonConstants.Play:
                        isActionRenderingDone = false;
                        DoPlay();
                        break;
                    case ButtonConstants.Restart:
                        isActionRenderingDone = false;
                        DoRestart();
                        break;
                    case ButtonConstants.TurnLeft:
                        isActionRenderingDone = false;
                        DoTurnLeft();
                        break;
                    case ButtonConstants.TurnRight:
                        isActionRenderingDone = false;
                        DoTurnRight();
                        break;

                }
            }
            else
            {
                Debug.Log("You can't do that now. Something is moving.");
            }
          
        }
       

    }

    public void NotifyEndOfAction()
    {
        Debug.Log("End Coroutine");
        isActionRenderingDone = true;
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
