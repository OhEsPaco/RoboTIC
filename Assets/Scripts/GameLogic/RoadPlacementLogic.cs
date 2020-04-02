using System;
using System.Collections.Generic;
using UnityEngine;
using static ButtonConstants;
using static CardConstants;

public class RoadPlacementLogic : MonoBehaviour
{
    [SerializeField] private GameObject roadStartMarker;
    [SerializeField] private GameObject selectedOutputMarker;
    [SerializeField] private GameObject navmeshExtension;
    private List<Buttons> buttonInputBuffer;
    public int initialCapacityOfTheInputBuffer = 20;
    private Dictionary<Buttons, Action> buttonActionsDictionary;


    private enum Which { Yes, No, None };

    internal void Start()
    {

        //Llenar el diccionario de funciones
        buttonActionsDictionary = new Dictionary<Buttons, Action>();
        buttonActionsDictionary.Add(Buttons.Action, DoAction);
        buttonActionsDictionary.Add(Buttons.Condition, DoCondition);
        buttonActionsDictionary.Add(Buttons.Jump, DoJump);
        buttonActionsDictionary.Add(Buttons.Loop, DoLoop);
        buttonActionsDictionary.Add(Buttons.Move, DoMove);
        buttonActionsDictionary.Add(Buttons.Play, DoPlay);
        buttonActionsDictionary.Add(Buttons.Restart, DoRestart);
        buttonActionsDictionary.Add(Buttons.TurnLeft, DoTurnLeft);
        buttonActionsDictionary.Add(Buttons.TurnRight, DoTurnRight);
        buttonActionsDictionary.Add(Buttons.Undo, DoUndo);
        buttonInputBuffer = new List<Buttons>(initialCapacityOfTheInputBuffer);


        selectedOutputMarker.transform.position = roadStartMarker.transform.position;
    }

    private void DoUndo()
    {
    }

    internal void Update()
    {
        ExecuteNextAvailableInput();
    }

    private void ExecuteNextAvailableInput()
    {
        if (buttonInputBuffer.Count > 0)
        {
            Buttons buttonPressed = buttonInputBuffer[0];
            buttonInputBuffer.RemoveAt(0);
            buttonActionsDictionary[buttonPressed]();
        }
    }

    public void AddInputFromButton(Buttons buttonIndex)
    {
        buttonInputBuffer.Add(buttonIndex);
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
    }

    private void DoPlay()
    {
        //LevelManager.instance.RoadMovementLogic.StartMovement(firstInput, lastOutput, player, conditionDictionary, loopsDictionary);
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


}