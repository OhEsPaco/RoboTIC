using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelButtons : MonoBehaviour
{
    public ButtonCounterScript Action;
    public ButtonCounterScript Condition;
    public ButtonCounterScript Jump;
    public ButtonCounterScript Loop;
    public ButtonCounterScript Move;
    public ButtonCounterScript TurnLeft;
    public ButtonCounterScript TurnRight;


    private LevelManager manager;


    public int SetNumberOfAvailableInstructions(int button, int number)
    {
        switch (button)
        {
            case ButtonConstants.Action:
                return Action.SetNumber(number);
                break;

            case ButtonConstants.Condition:
                return Condition.SetNumber(number);
                break;
            case ButtonConstants.Jump:
                return Jump.SetNumber(number);
                break;
            case ButtonConstants.Loop:
                return Loop.SetNumber(number);
                break;
            case ButtonConstants.Move:
                return Move.SetNumber(number);
                break;
            case ButtonConstants.TurnLeft:
                return TurnLeft.SetNumber(number);
                break;
            case ButtonConstants.TurnRight:
                return TurnRight.SetNumber(number);
                break;
            default:
                return 0;

        }
    }

    /// <summary>
    /// The SetAvailableInstructions
    /// </summary>
    /// <param name="data">The data<see cref="CurrentLevelData"/></param>
    public void SetNumberOfAvailableInstructions(LevelData data)
    {
      
        SetNumberOfAvailableInstructions(ButtonConstants.Action, data.availableInstructions.action);
        SetNumberOfAvailableInstructions(ButtonConstants.Condition, data.availableInstructions.condition);
        SetNumberOfAvailableInstructions(ButtonConstants.Jump, data.availableInstructions.jump);
        SetNumberOfAvailableInstructions(ButtonConstants.Loop, data.availableInstructions.loop);
        SetNumberOfAvailableInstructions(ButtonConstants.Move, data.availableInstructions.move);
        SetNumberOfAvailableInstructions(ButtonConstants.TurnLeft, data.availableInstructions.turnLeft);
        SetNumberOfAvailableInstructions(ButtonConstants.TurnRight, data.availableInstructions.turnRight);
    }

    void Awake()
    {
        manager = LevelManager.instance;
      
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
