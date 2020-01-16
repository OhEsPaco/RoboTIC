using System;
using System.Collections.Generic;
using UnityEngine;
using static ButtonConstants;
using static CardConstants;
using static RoadConstants;

public class RoadLogic : MonoBehaviour
{
    [SerializeField] private GameObject mainCharacterGameObject;
    [SerializeField] private GameObject roadStartMarker;
    [SerializeField] private GameObject selectedOutputMarker;
    private List<Road> spawnedRoads = new List<Road>();

    private List<Buttons> buttonInputBuffer;
    private RoadOutput selectedOutput;

    public int initialCapacityOfTheInputBuffer = 20;

    private Dictionary<Buttons, Action> buttonActionsDictionary;

    private SelectedRoadAndOutput selectedOutputAndRoad;

    private struct SelectedRoadAndOutput
    {
        private int SelectedRoad;
        private int SelectedOutput;
    }

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
        buttonInputBuffer = new List<Buttons>(initialCapacityOfTheInputBuffer);
    }

    internal void Update()
    {
        if (Input.GetKeyDown(KeyCode.H)) // forward
        {
            if (selectedOutput != null)
            {
                RoadOutput nextOutput = GetNextRoadOutput();
                if (nextOutput != null)
                {
                    selectedOutput = nextOutput;
                    selectedOutputMarker.transform.position = new Vector3(selectedOutput.transform.position.x, selectedOutputMarker.transform.position.y, selectedOutput.transform.position.z);
                }
            }
        }

        ExecuteNextAvailableInput();
    }

    private RoadOutput GetNextRoadOutput()
    {
        return null;
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

    private void SpawnRoad(RoadType thisRoad)
    {
    }

    public void InformOfCardChanged(Road road, Cards newCard)
    {
        Debug.Log("Changed to card " + newCard.ToString() + " on road " + road.RoadType.ToString());
    }

    public void InformOfLoopRepsChanged(Road road, int newReps)
    {
        Debug.Log("Changed to number " + newReps + " on road " + road.RoadType.ToString());
    }
}