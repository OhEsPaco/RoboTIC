using System;
using System.Collections.Generic;
using UnityEngine;
using static ButtonConstants;

public class RoadPlacementLogic : MonoBehaviour
{
    [SerializeField] private Transform roadStartMarker;
    [SerializeField] private GameObject selectedOutputMarker;
    [SerializeField] private Transform roadParent;
    private List<Buttons> buttonInputBuffer;
    public int initialCapacityOfTheInputBuffer = 20;
    private Dictionary<Buttons, Action> buttonActionsDictionary;

    private RoadIO selectedIO = null;
    private RoadIO pivotIO = null;

    private const float MAX_ACCEPTABLE_DISTANCE = 0.3f;

    public RoadIO PivotIO { get => pivotIO; set => pivotIO = value; }
    public RoadIO SelectedIO { get => selectedIO; set => selectedIO = value; }

    internal void Start()
    {
        // selectedOutputMarker.transform.parent = roadParent;

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

        selectedOutputMarker.transform.position = roadStartMarker.position;
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
        SpawnVerticalButton("action");
    }

    private void DoCondition()
    {
    }

    private void DoJump()
    {
        SpawnVerticalButton("jump");
    }

    private void DoLoop()
    {
    }

    private bool SpawnRoad(in string id, out Road spawnedRoad)
    {
        spawnedRoad = null;
        if (this.selectedIO == null)
        {
            if (LevelManager.instance.RoadFactory.SpawnRoadByName(id, out spawnedRoad))
            {
                spawnedRoad.transform.parent = roadParent;
                RoadIO[] allRoadIO = spawnedRoad.GetAllIO();
               /* if (pivotIO == null)
                {
                    pivotIO = allRoadIO[0];
                }*/

                foreach (RoadIO rIO in allRoadIO)
                {
                    if (rIO is RoadInput && rIO.CanBeSelected)
                    {
                        this.selectedIO = rIO;
                        pivotIO = rIO;
                        break;
                    }
                }
            }
        }
        else
        {
            //Buscar los io en el mismo lado y conectarlos
            if (this.selectedIO.connectedTo == null)
            {
                List<RoadIO> ioToMatch = new List<RoadIO>();
                ioToMatch.Add(this.selectedIO);
                //SpawnAndConnectRoad(in Road roadToSpawn, in List<RoadIO> ioToMatch, in float errorMargin, out Road spawnedRoad)
                
                if (LevelManager.instance.RoadFactory.SpawnRoadByName(id, ioToMatch, out spawnedRoad))
                {
                    spawnedRoad.transform.parent = roadParent;
                    /*RoadIO[] allRoadIO = spawnedRoad.GetAllIO();
                    foreach (RoadIO rIO in allRoadIO)
                    {
                        if (rIO is RoadOutput && rIO.CanBeSelected)
                        {
                            //this.selectedIO = rIO;

                            break;
                        }
                    }*/
                }
            }
            else
            {
                List<RoadIO> ioToMatch = new List<RoadIO>();
                ioToMatch.Add(this.selectedIO);

                List<RoadIO> ioToMatch2 = new List<RoadIO>();
                ioToMatch2.Add(this.selectedIO.connectedTo);
                //SpawnAndConnectRoad(in Road roadToSpawn, in List<RoadIO> ioToMatch, in float errorMargin, out Road spawnedRoad)
                if (LevelManager.instance.RoadFactory.SpawnRoadByName(id, ioToMatch, ioToMatch2, out spawnedRoad))
                {
                    spawnedRoad.transform.parent = roadParent;
                   /* RoadIO[] allRoadIO = spawnedRoad.GetAllIO();
                    foreach (RoadIO rIO in allRoadIO)
                    {
                        if (rIO is RoadOutput && rIO.CanBeSelected)
                        {
                            this.selectedIO = rIO;

                            break;
                        }
                    }*/
                }
            }
        }

        if (spawnedRoad == null)
        {
            return false;
        }
        else
        {
            //Mejorar esto
            CorrectRoadsPosition(this.pivotIO, MAX_ACCEPTABLE_DISTANCE);

            if(this.selectedIO is RoadInput)
            {
                RoadIO f = FindFartestInput(this.selectedIO, spawnedRoad.GetAllIO());
                if (f != null)
                {
                    this.selectedIO = f;
                    this.pivotIO = f;
                    CorrectRoadsPosition(this.pivotIO, MAX_ACCEPTABLE_DISTANCE);
                }

            }
            this.selectedOutputMarker.transform.position = this.selectedIO.transform.position;
            return true;
        }
    }

    private RoadIO FindFartestInput(RoadIO pivot, RoadIO[] allIO)
    {

        RoadIO fartest = allIO[0];
        foreach(RoadIO rIO in allIO)
        {
            if(fartest is RoadOutput && rIO is RoadInput && rIO.CanBeSelected)
            {
                fartest = rIO;
            }
            else if(rIO is RoadInput && rIO.CanBeSelected)
            {
                if (Vector3.Distance(pivot.transform.position, fartest.transform.position) < Vector3.Distance(pivot.transform.position, rIO.transform.position))
                {
                    fartest = rIO;
                }
            }
        }

        if(fartest is RoadOutput || !fartest.CanBeSelected)
        {
            return null;
        }
        return fartest;
    }
    private void SpawnVerticalButton(string button)
    {
        Road spawnedRoad;
        if (SpawnRoad("NodeVerticalButton", out spawnedRoad))
        {
            string[] args = { "activate", button };
            spawnedRoad.ExecuteAction(args);
        }
    }

    private void DoMove()
    {
        SpawnVerticalButton("move");
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
        SpawnVerticalButton("turn_left");
    }

    private void DoTurnRight()
    {
        SpawnVerticalButton("turn_right");
    }

    

    private void CorrectRoadsPosition(RoadIO pivotIO, in float maxAcceptableDistance)
    {
        if (pivotIO != null)
        {
            pivotIO.MoveRoadTo(roadStartMarker.position);

            List<Road> processedRoads = new List<Road>();
            processedRoads.Add(pivotIO.GetParentRoad());

            Stack<RoadIO> ioToProc = new Stack<RoadIO>();

            RoadIO[] tmpe = pivotIO.GetParentRoad().GetAllIO();

            foreach (RoadIO rio in tmpe)
            {
                ioToProc.Push(rio);
            }

            while (ioToProc.Count > 0)
            {
                RoadIO toProc = ioToProc.Pop();
                RoadIO connectedTo = toProc.ConnectedTo;
                if (connectedTo != null)
                {
                    float distance = Vector3.Distance(toProc.transform.position, connectedTo.transform.position);
                    if (distance > maxAcceptableDistance)
                    {
                        connectedTo.MoveRoadTo(toProc.transform.position);
                    }

                    processedRoads.Add(connectedTo.GetParentRoad());

                    tmpe = connectedTo.GetParentRoad().GetAllIO();

                    foreach (RoadIO rio in tmpe)
                    {
                        if (rio.ConnectedTo != null)
                        {
                            if (!processedRoads.Contains(rio.ConnectedTo.GetParentRoad()))
                            {
                                ioToProc.Push(rio);
                            }
                        }
                    }
                }
            }
        }
    }
}