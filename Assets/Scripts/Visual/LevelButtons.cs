﻿using UnityEngine;

public class LevelButtons : MonoBehaviour
{
    [SerializeField] private EventAggregator eventAggregator;

    public enum Buttons
    {
        Action = 0,
        Condition = 1,
        Jump = 2,
        Loop = 3,
        Move = 4,
        Play = 5,
        Restart = 6,
        TurnLeft = 7,
        TurnRight = 8,
        Undo = 9,
        MapMenu = 10,
        Undefined = 999
    };

    public ButtonCounterScript Action;
    public ButtonCounterScript Condition;
    public ButtonCounterScript Jump;
    public ButtonCounterScript Loop;
    public ButtonCounterScript Move;
    public ButtonCounterScript TurnLeft;
    public ButtonCounterScript TurnRight;

    private RoadButton[] allRoadButtons;

    private void DisableAllButtons(MsgDisableAllButtons msg)
    {
        foreach (RoadButton r in allRoadButtons)
        {
            r.Disable();
        }
    }

    private void EnableButton(MsgEnableButton msg)
    {
        foreach (RoadButton r in allRoadButtons)
        {
            if (r.ButtonType == msg.button)
            {
                r.Enable();
            }
        }
    }

    private void Awake()
    {
        eventAggregator.Subscribe<MsgSetAvInstructions>(SetNumberOfAvailableInstructions);
        eventAggregator.Subscribe<MsgEnableAllButtons>(EnableAllButtons);
        eventAggregator.Subscribe<MsgDisableAllButtons>(DisableAllButtons);
        eventAggregator.Subscribe<MsgEnableButton>(EnableButton);
    }

    private void EnableAllButtons(MsgEnableAllButtons msg)
    {
        foreach (RoadButton r in allRoadButtons)
        {
            r.Enable();
        }
    }

    private void Start()
    {
        allRoadButtons = FindObjectsOfType<RoadButton>();
    }

    private int SetNumberOfAvailableInstructions(in Buttons button, int number)
    {
        switch (button)
        {
            case Buttons.Action:
                return Action.SetNumber(number);

            case Buttons.Condition:
                return Condition.SetNumber(number);

            case Buttons.Jump:
                return Jump.SetNumber(number);

            case Buttons.Loop:
                return Loop.SetNumber(number);

            case Buttons.Move:
                return Move.SetNumber(number);

            case Buttons.TurnLeft:
                return TurnLeft.SetNumber(number);

            case Buttons.TurnRight:
                return TurnRight.SetNumber(number);

            default:
                return 0;
        }
    }

    /// <summary>
    /// The SetAvailableInstructions
    /// </summary>
    /// <param name="data">The data<see cref="CurrentLevelData"/></param>
    private void SetNumberOfAvailableInstructions(MsgSetAvInstructions msg)
    {
        AvailableInstructions availableInstructions = msg.avInst;
        SetNumberOfAvailableInstructions(Buttons.Action, availableInstructions.action);
        SetNumberOfAvailableInstructions(Buttons.Condition, availableInstructions.condition);
        SetNumberOfAvailableInstructions(Buttons.Jump, availableInstructions.jump);
        SetNumberOfAvailableInstructions(Buttons.Loop, availableInstructions.loop);
        SetNumberOfAvailableInstructions(Buttons.Move, availableInstructions.move);
        SetNumberOfAvailableInstructions(Buttons.TurnLeft, availableInstructions.turnLeft);
        SetNumberOfAvailableInstructions(Buttons.TurnRight, availableInstructions.turnRight);
    }
}