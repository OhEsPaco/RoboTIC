using System;
using System.Collections.Generic;
using UnityEngine;
using static ButtonConstants;
using static CardConstants;
using static RoadConstants;

public class RoadPlacementLogic : MonoBehaviour
{
    [SerializeField] private GameObject roadStartMarker;
    [SerializeField] private GameObject selectedOutputMarker;
    [SerializeField] private GameObject navmeshExtension;
    private List<Buttons> buttonInputBuffer;
    private RoadIO selectedIO = null;
    private RoadIO pivotIO = null;
    public int initialCapacityOfTheInputBuffer = 20;

    private Dictionary<Buttons, Action> buttonActionsDictionary;
    private Dictionary<Road, Cards> conditionDictionary;
    private Dictionary<Road, int> loopsDictionary;

    private enum Which { Yes, No, None };

    internal void Start()
    {
        conditionDictionary = new Dictionary<Road, Cards>();
        loopsDictionary = new Dictionary<Road, int>();

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
        if (selectedIO != null)
        {
            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                selectedIO = Back(selectedIO);
                Debug.Log(selectedIO.IsInputOrOutput());
            }

            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                selectedIO = Forward(selectedIO);
                Debug.Log(selectedIO.IsInputOrOutput());
            }

            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                selectedIO = Left(selectedIO);
                Debug.Log(selectedIO.IsInputOrOutput());
            }

            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                selectedIO = Right(selectedIO);
                Debug.Log(selectedIO.IsInputOrOutput());
            }

            selectedOutputMarker.transform.position = selectedIO.transform.position;
        }
        else
        {
            selectedOutputMarker.transform.position = roadStartMarker.transform.position;
        }

        ExecuteNextAvailableInput();
    }

    // InsideOf(RoadIO index) posible afectado
    private RoadIO Back(RoadIO io)
    {
        switch (io.GetParentRoad().RoadType)
        {
            case RoadType.Vertical:
                if (io.IsInputOrOutput() == InputOutput.Output)
                {
                    return Back(io.GetParentRoad().Inputs()[0]);
                }
                else
                {
                    RoadIO connectedTo = io.ConnectedTo();
                    if (connectedTo != null)
                    {
                        return connectedTo;
                    }
                }

                break;

            case RoadType.IfIn:
                if (io.IsInputOrOutput() == InputOutput.Output)
                {
                    return Back(io.GetParentRoad().Inputs()[0]);
                }
                else
                {
                    RoadIO connectedTo = io.ConnectedTo();
                    if (connectedTo != null)
                    {
                        return connectedTo;
                    }
                }

                break;

            case RoadType.IfOut:
                if (io.IsInputOrOutput() == InputOutput.Output)
                {
                    return Back(io.GetParentRoad().Inputs()[0]);
                }
                else
                {
                    RoadIO connectedTo = io.ConnectedTo();
                    if (connectedTo != null)
                    {
                        return connectedTo;
                    }
                }

                break;

            case RoadType.LoopIn:
                if (io.IsInputOrOutput() == InputOutput.Output)
                {
                    return Back(io.GetParentRoad().ReturnByIOAndDirection(InputOutput.Input, PointingTo.Back)[0]);
                }
                else
                {
                    RoadIO connectedTo = io.ConnectedTo();
                    if (connectedTo != null)
                    {
                        return connectedTo;
                    }
                }
                break;

            case RoadType.LoopOut:
                if (io.IsInputOrOutput() == InputOutput.Output)
                {
                    return Back(io.GetParentRoad().ReturnByIOAndDirectionAndType(InputOutput.Input, PointingTo.Back, IOType.Yes)[0]);
                }
                else
                {
                    RoadIO connectedTo = io.ConnectedTo();
                    if (connectedTo != null)
                    {
                        return connectedTo;
                    }
                }

                break;
        }

        return io;
    }

    private RoadIO Forward(RoadIO io)
    {
        switch (io.GetParentRoad().RoadType)
        {
            case RoadType.Vertical:
                if (io.IsInputOrOutput() == InputOutput.Input)
                {
                    return io.GetParentRoad().Outputs()[0];
                }
                else
                {
                    RoadIO connectedTo = io.ConnectedTo();
                    if (connectedTo != null)
                    {
                        return Forward(connectedTo);
                    }
                }

                break;

            case RoadType.IfIn:
                if (io.IsInputOrOutput() == InputOutput.Input)
                {
                    return io.GetParentRoad().Outputs()[0];
                }
                else
                {
                    RoadIO connectedTo = io.ConnectedTo();
                    if (connectedTo != null)
                    {
                        return Forward(connectedTo);
                    }
                }

                break;

            case RoadType.IfOut:
                if (io.IsInputOrOutput() == InputOutput.Input)
                {
                    return io.GetParentRoad().Outputs()[0];
                }
                else
                {
                    RoadIO connectedTo = io.ConnectedTo();
                    if (connectedTo != null)
                    {
                        return Forward(connectedTo);
                    }
                }

                break;

            case RoadType.LoopIn:
                if (io.IsInputOrOutput() == InputOutput.Input)
                {
                    return io.GetParentRoad().ReturnByIOAndDirection(InputOutput.Output, PointingTo.Forward)[0];
                }
                else
                {
                    RoadIO connectedTo = io.ConnectedTo();
                    if (connectedTo != null)
                    {
                        return Forward(connectedTo);
                    }
                }

                break;

            case RoadType.LoopOut:
                if (io.IsInputOrOutput() == InputOutput.Input)
                {
                    return io.GetParentRoad().ReturnByIOAndDirection(InputOutput.Output, PointingTo.Forward)[0];
                }
                else
                {
                    RoadIO connectedTo = io.ConnectedTo();
                    if (connectedTo != null)
                    {
                        return Forward(connectedTo);
                    }
                }

                break;
        }
        return io;
    }

    private RoadIO Right(RoadIO io)
    {
        switch (io.GetParentRoad().RoadType)
        {
            case RoadType.Vertical:

                if (InsideOfIf(io))
                {
                    int movs = 0;
                    Road ifIn = io.GetParentRoad();
                    RoadIO thisIO = io;
                    do
                    {
                        thisIO = Back(thisIO);
                        ifIn = thisIO.GetParentRoad();
                        movs++;
                    } while (ifIn.RoadType != RoadType.IfIn);

                    thisIO = Right(thisIO);
                    for (int i = 0; i < movs; i++)
                    {
                        thisIO = Forward(thisIO);
                    }
                    return thisIO;
                }
                else
                {
                    return Forward(io);
                }
                break;

            case RoadType.IfIn:
                if (io.IoType == IOType.Yes)
                {
                    return io.GetParentRoad().ReturnByIOAndDirectionAndType(InputOutput.Output, PointingTo.Forward, IOType.No)[0];
                }
                else if (io.IoType == IOType.No)
                {
                    return io.GetParentRoad().ReturnByIOAndDirectionAndType(InputOutput.Output, PointingTo.Forward, IOType.Yes)[0];
                }
                else
                {
                    return Forward(io);
                }

                break;

            case RoadType.IfOut:
                if (io.IoType == IOType.Yes)
                {
                    return io.GetParentRoad().ReturnByIOAndDirectionAndType(InputOutput.Input, PointingTo.Back, IOType.No)[0];
                }
                else if (io.IoType == IOType.No)
                {
                    return io.GetParentRoad().ReturnByIOAndDirectionAndType(InputOutput.Input, PointingTo.Back, IOType.Yes)[0];
                }
                else
                {
                    return Forward(io);
                }
                break;

            case RoadType.LoopIn:
                return Forward(io);

                break;

            case RoadType.LoopOut:

                return Forward(io);
                break;
        }
        return io;
    }

    private RoadIO Left(RoadIO io)
    {
        switch (io.GetParentRoad().RoadType)
        {
            case RoadType.Vertical:
                if (InsideOfIf(io))
                {
                    int movs = 0;
                    Road ifIn = io.GetParentRoad();
                    RoadIO thisIO = io;
                    do
                    {
                        thisIO = Back(thisIO);
                        ifIn = thisIO.GetParentRoad();
                        movs++;
                    } while (ifIn.RoadType != RoadType.IfIn);

                    thisIO = Left(thisIO);
                    for (int i = 0; i < movs; i++)
                    {
                        thisIO = Forward(thisIO);
                    }
                    return thisIO;
                }
                else
                {
                    return Back(io);
                }

                break;

            case RoadType.IfIn:
                if (io.IoType == IOType.Yes)
                {
                    return io.GetParentRoad().ReturnByIOAndDirectionAndType(InputOutput.Output, PointingTo.Forward, IOType.No)[0];
                }
                else if (io.IoType == IOType.No)
                {
                    return io.GetParentRoad().ReturnByIOAndDirectionAndType(InputOutput.Output, PointingTo.Forward, IOType.Yes)[0];
                }
                else
                {
                    return Back(io);
                }

                break;

            case RoadType.IfOut:
                if (io.IoType == IOType.Yes)
                {
                    return io.GetParentRoad().ReturnByIOAndDirectionAndType(InputOutput.Input, PointingTo.Back, IOType.No)[0];
                }
                else if (io.IoType == IOType.No)
                {
                    return io.GetParentRoad().ReturnByIOAndDirectionAndType(InputOutput.Input, PointingTo.Back, IOType.Yes)[0];
                }
                else
                {
                    return Back(io);
                }
                break;

            case RoadType.LoopIn:
                return Back(io);

                break;

            case RoadType.LoopOut:

                return Back(io);
                break;
        }
        return io;
    }

    private bool InsideOfIf(RoadIO roadToProcess)
    {
        List<Road> insideOf = InsideOf(roadToProcess);
        foreach (Road r in insideOf)
        {
            if (r.RoadType == RoadType.IfIn)
            {
                return true;
            }
        }
        return false;
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
        SpawnAndPlaceAction(Buttons.Action);
    }

    private void DoCondition()
    {
        if (selectedIO == null)
        {
            Road ifIn = SpawnRoad(RoadType.IfIn);
            conditionDictionary.Add(ifIn, Cards.NoCard);
            ifIn.transform.position = roadStartMarker.transform.position;
            Road ifOut = SpawnRoad(RoadType.IfOut);

            selectedIO = ifIn.ReturnByIOAndDirection(InputOutput.Output, PointingTo.Forward)[0];
            pivotIO = ifIn.ReturnByIOAndDirection(InputOutput.Input, PointingTo.Back)[0];
            pivotIO.MoveRoadTo(roadStartMarker.transform.position);
            AttachRoadsByDirectionAndType(ifIn, PointingTo.Forward, ifOut);
        }
        else
        {
            List<Road> insideOf = InsideOf(selectedIO);
            if (insideOf.Count > 1)
            {
                return;
            }
            else if (insideOf.Count == 1)
            {
                if (insideOf[0].RoadType == RoadType.LoopIn)
                {
                    ElongateLoop(insideOf[0]);
                    ElongateLoop(insideOf[0]);
                }
                else
                {
                    return;
                }
            }

            Road ifIn = SpawnRoad(RoadType.IfIn);
            conditionDictionary.Add(ifIn, Cards.NoCard);
            Road ifOut = SpawnRoad(RoadType.IfOut);

            AttachRoadsByDirectionAndType(ifIn, PointingTo.Forward, ifOut);

            RoadIO selIoConection = selectedIO.ConnectedTo();

            RoadIO verticalInput = ifIn.ReturnByDirection(PointingTo.Back)[0];
            RoadIO verticalOutput = ifOut.ReturnByDirection(PointingTo.Forward)[0];

            if (selectedIO.IsInputOrOutput() == InputOutput.Input)
            {
                verticalOutput.ConnectTo(selectedIO);
                if (selIoConection != null)
                {
                    verticalInput.ConnectTo(selIoConection);
                }
            }
            else
            {
                verticalInput.ConnectTo(selectedIO);
                if (selIoConection != null)
                {
                    verticalOutput.ConnectTo(selIoConection);
                }
            }
            selectedIO = ifIn.ReturnByIOAndDirection(InputOutput.Output, PointingTo.Forward)[0];
        }
        CorrectPositions(0.3f);
    }

    private void DoJump()
    {
        SpawnAndPlaceAction(Buttons.Jump);
    }

    private void DoLoop()
    {
        if (selectedIO == null)
        {
            Road loopIn = SpawnRoad(RoadType.LoopIn);
            loopsDictionary.Add(loopIn, 0);
            selectedIO = loopIn.ReturnByIOAndDirectionAndType(InputOutput.Output, PointingTo.Forward, IOType.Yes)[0];
            pivotIO = loopIn.ReturnByIOAndDirection(InputOutput.Input, PointingTo.Back)[0];
            pivotIO.MoveRoadTo(roadStartMarker.transform.position);
            loopIn.transform.position = roadStartMarker.transform.position;

            Road loopOut = SpawnRoad(RoadType.LoopOut);

            AttachRoadsByDirectionAndType(loopIn, PointingTo.Forward, loopOut);
        }
        else
        {
            List<Road> insideOf = InsideOf(selectedIO);
            if (insideOf.Count > 0)
            {
                return;
            }

            Road loopIn = SpawnRoad(RoadType.LoopIn);
            loopsDictionary.Add(loopIn, 0);
            Road loopOut = SpawnRoad(RoadType.LoopOut);

            AttachRoadsByDirectionAndType(loopIn, PointingTo.Forward, loopOut);

            RoadIO selIoConection = selectedIO.ConnectedTo();

            RoadIO verticalInput = loopIn.ReturnByDirection(PointingTo.Back)[0];
            RoadIO verticalOutput = loopOut.ReturnByDirection(PointingTo.Forward)[0];

            if (selectedIO.IsInputOrOutput() == InputOutput.Input)
            {
                verticalOutput.ConnectTo(selectedIO);
                if (selIoConection != null)
                {
                    verticalInput.ConnectTo(selIoConection);
                }
            }
            else
            {
                verticalInput.ConnectTo(selectedIO);
                if (selIoConection != null)
                {
                    verticalOutput.ConnectTo(selIoConection);
                }
            }
            selectedIO = loopIn.ReturnByIOAndDirectionAndType(InputOutput.Output, PointingTo.Forward, IOType.Yes)[0];
        }
        CorrectPositions(0.3f);
    }

    private void CorrectPositions(in float maxAcceptableDistance)
    {
        if (pivotIO != null)
        {
            pivotIO.MoveRoadTo(roadStartMarker.transform.position);

            List<Road> processedRoads = new List<Road>();
            processedRoads.Add(pivotIO.GetParentRoad());

            Stack<RoadIO> ioToProc = new Stack<RoadIO>();

            RoadIO[] tmpe = pivotIO.GetParentRoad().InputsAndOutputs;

            foreach (RoadIO rio in tmpe)
            {
                ioToProc.Push(rio);
            }

            while (ioToProc.Count > 0)
            {
                RoadIO toProc = ioToProc.Pop();
                RoadIO connectedTo = toProc.ConnectedTo();
                if (connectedTo != null)
                {
                    float distance = Vector3.Distance(toProc.transform.position, connectedTo.transform.position);
                    if (distance > maxAcceptableDistance)
                    {
                        connectedTo.MoveRoadTo(toProc.transform.position);
                    }

                    processedRoads.Add(connectedTo.GetParentRoad());

                    tmpe = connectedTo.GetParentRoad().InputsAndOutputs;

                    foreach (RoadIO rio in tmpe)
                    {
                        if (rio.ConnectedTo() != null)
                        {
                            if (!processedRoads.Contains(rio.ConnectedTo().GetParentRoad()))
                            {
                                ioToProc.Push(rio);
                            }
                        }
                    }
                }
            }
        }
    }

    private Road ElongateIf(Road ifIn)
    {
        if (ifIn.RoadType == RoadType.IfIn)

        {
            /////PLACEHOLDER
            Road connector = null;
            /////

            Which rToEln = IfWhatToElongate(ifIn);
            Debug.Log(rToEln.ToString());
            RoadIO thisIo = null;

            switch (rToEln)
            {
                case Which.Yes:
                    connector = SpawnRoad(Buttons.Undefined);
                    thisIo = ifIn.ReturnByIOAndDirectionAndType(InputOutput.Output, PointingTo.Forward, IOType.Yes)[0];
                    break;

                case Which.No:
                    connector = SpawnRoad(Buttons.Undefined);
                    thisIo = ifIn.ReturnByIOAndDirectionAndType(InputOutput.Output, PointingTo.Forward, IOType.No)[0];
                    break;

                case Which.None:
                    return null;
            }

            RoadIO currentlyConnectedTo = thisIo.ConnectedTo();

            connector.ReturnByIO(InputOutput.Input)[0].ConnectTo(thisIo);
            connector.ReturnByIO(InputOutput.Output)[0].ConnectTo(currentlyConnectedTo);

            return connector;
        }
        return null;
    }

    private Which IfWhatToElongate(Road ifIn)
    {
        RoadIO ifInOutputNo = ifIn.ReturnByIOAndDirectionAndType(InputOutput.Output, PointingTo.Forward, IOType.No)[0];
        RoadIO ifInOutputYes = ifIn.ReturnByIOAndDirectionAndType(InputOutput.Output, PointingTo.Forward, IOType.Yes)[0];

        do
        {
            Road roadNo = ifInOutputNo.GetParentRoad();
            Road roadYes = ifInOutputYes.GetParentRoad();

            Debug.Log(roadNo.RoadType);
            Debug.Log(roadYes.RoadType);
            Debug.Log("");
            if (roadNo.RoadType == RoadType.IfOut && roadYes.RoadType == RoadType.IfOut)
            {
                return Which.None;
            }
            else if (roadNo.RoadType == RoadType.IfOut)
            {
                return Which.No;
            }
            else if (roadYes.RoadType == RoadType.IfOut)
            {
                return Which.Yes;
            }

            if (ifInOutputNo.IsInputOrOutput() == InputOutput.Input)
            {
                ifInOutputNo = ifInOutputNo.GetParentRoad().Outputs()[0];
            }
            else
            {
                ifInOutputNo = ifInOutputNo.ConnectedTo();
            }

            if (ifInOutputYes.IsInputOrOutput() == InputOutput.Input)
            {
                ifInOutputYes = ifInOutputYes.GetParentRoad().Outputs()[0];
            }
            else
            {
                ifInOutputYes = ifInOutputYes.ConnectedTo();
            }
        } while (true);
    }

    private Road ElongateLoop(Road loopIn)
    {
        if (loopIn.RoadType == RoadType.LoopIn)
        {
            RoadIO loopInOutputNo = loopIn.ReturnByIOAndDirectionAndType(InputOutput.Output, PointingTo.Forward, IOType.No)[0];
            RoadIO loopInInputGeneric = loopIn.ReturnByIOAndDirectionAndType(InputOutput.Input, PointingTo.Forward, IOType.Generic)[0];

            RoadIO loopOutInputNo = loopInOutputNo.ConnectedTo();
            RoadIO loopOutOutputGeneric = loopInInputGeneric.ConnectedTo();

            Road connector = SpawnRoad(RoadType.Double);

            connector.ReturnByIOAndDirectionAndType(InputOutput.Input, PointingTo.Back, IOType.No)[0].ConnectTo(loopInOutputNo);
            connector.ReturnByIOAndDirectionAndType(InputOutput.Output, PointingTo.Back, IOType.Generic)[0].ConnectTo(loopInInputGeneric);

            connector.ReturnByIOAndDirectionAndType(InputOutput.Output, PointingTo.Forward, IOType.No)[0].ConnectTo(loopOutInputNo);
            connector.ReturnByIOAndDirectionAndType(InputOutput.Input, PointingTo.Forward, IOType.Generic)[0].ConnectTo(loopOutOutputGeneric);
            return connector;
        }
        return null;
    }

    private void AttachRoadsByDirectionAndType(Road r1, PointingTo direction, Road r2)
    {
        List<RoadIO> r1Io = r1.ReturnByDirection(direction);

        List<RoadIO> r2Io = r2.ReturnByDirection(RoadMethods.OppositeDirection(direction));

        foreach (RoadIO io1 in r1Io)
        {
            foreach (RoadIO io2 in r2Io)
            {
                if (io1.IoType == io2.IoType && io1.IsInputOrOutput() != io2.IsInputOrOutput())
                {
                    io1.ConnectTo(io2);
                    // io2.ConnectTo(io1);
                }
            }
        }
    }

    private void SpawnAndPlaceAction(Buttons button)
    {
        if (selectedIO == null)
        {
            Road spawned = SpawnRoad(button);

            selectedIO = spawned.ReturnByIO(InputOutput.Output)[0];
            pivotIO = spawned.ReturnByIO(InputOutput.Input)[0];
            pivotIO.MoveRoadTo(roadStartMarker.transform.position);
        }
        else
        {
            List<Road> insideOf = InsideOf(selectedIO);

            if (insideOf.Count > 2)
            {
                return;
            }

            Road spawned = SpawnRoad(button);

            RoadIO selIoConection = selectedIO.ConnectedTo();

            RoadIO verticalInput = spawned.ReturnByDirection(PointingTo.Back)[0];
            RoadIO verticalOutput = spawned.ReturnByDirection(PointingTo.Forward)[0];

            if (selectedIO.IsInputOrOutput() == InputOutput.Input)
            {
                verticalOutput.ConnectTo(selectedIO);
                if (selIoConection != null)
                {
                    verticalInput.ConnectTo(selIoConection);
                }
            }
            else
            {
                verticalInput.ConnectTo(selectedIO);
                if (selIoConection != null)
                {
                    verticalOutput.ConnectTo(selIoConection);
                }
            }

            if (insideOf.Count > 0)
            {
                foreach (Road r in insideOf)
                {
                    if (r.RoadType == RoadType.LoopIn)
                    {
                        ElongateLoop(r);
                    }
                    else if (r.RoadType == RoadType.IfIn)
                    {
                        ElongateIf(r);
                    }
                }
            }
            selectedIO = spawned.ReturnByIO(InputOutput.Output)[0];
        }
        CorrectPositions(0.3f);
    }

    private void DoMove()
    {
        SpawnAndPlaceAction(Buttons.Move);
    }

    private List<Road> InsideOf(RoadIO index)
    {
        bool keepAddingLoops = true;
        bool keepAddingIfs = true;
        List<Road> result = new List<Road>();
        if (pivotIO != null)
        {
            bool exit = false;
            RoadIO backwards = index;

            do
            {
                if (backwards.ConnectedTo() == null)
                {
                    return result;
                }
                else
                {
                    backwards = Backwards(backwards);

                    Road road = backwards.GetParentRoad();
                    switch (road.RoadType)
                    {
                        case RoadType.IfIn:

                            if (keepAddingIfs)
                            {
                                if (!result.Contains(road))
                                {
                                    result.Add(road);
                                }
                            }

                            break;

                        case RoadType.IfOut:

                            keepAddingIfs = false;
                            break;

                        case RoadType.LoopIn:

                            if (keepAddingLoops)
                            {
                                if (!result.Contains(road))
                                {
                                    result.Add(road);
                                }
                            }
                            break;

                        case RoadType.LoopOut:

                            keepAddingLoops = false;
                            break;
                    }
                }
            } while (!exit);
        }
        return result;
    }

    private RoadIO Backwards(RoadIO io)
    {
        switch (io.GetParentRoad().RoadType)
        {
            case RoadType.Vertical:
                if (io.IsInputOrOutput() == InputOutput.Output)
                {
                    return io.GetParentRoad().Inputs()[0];
                }
                else
                {
                    RoadIO connectedTo = io.ConnectedTo();
                    if (connectedTo != null)
                    {
                        return connectedTo;
                    }
                }

                break;

            case RoadType.IfIn:
                if (io.IsInputOrOutput() == InputOutput.Output)
                {
                    return io.GetParentRoad().Inputs()[0];
                }
                else
                {
                    RoadIO connectedTo = io.ConnectedTo();
                    if (connectedTo != null)
                    {
                        return connectedTo;
                    }
                }

                break;

            case RoadType.IfOut:
                if (io.IsInputOrOutput() == InputOutput.Output)
                {
                    return io.GetParentRoad().Inputs()[0];
                }
                else
                {
                    RoadIO connectedTo = io.ConnectedTo();
                    if (connectedTo != null)
                    {
                        return connectedTo;
                    }
                }

                break;

            case RoadType.LoopIn:
                if (io.IsInputOrOutput() == InputOutput.Output)
                {
                    return io.GetParentRoad().ReturnByIOAndDirection(InputOutput.Input, PointingTo.Back)[0];
                }
                else
                {
                    RoadIO connectedTo = io.ConnectedTo();
                    if (connectedTo != null)
                    {
                        return connectedTo;
                    }
                }
                break;

            case RoadType.LoopOut:
                if (io.IsInputOrOutput() == InputOutput.Output)
                {
                    return io.GetParentRoad().ReturnByIOAndDirectionAndType(InputOutput.Input, PointingTo.Back, IOType.Yes)[0];
                }
                else
                {
                    RoadIO connectedTo = io.ConnectedTo();
                    if (connectedTo != null)
                    {
                        return connectedTo;
                    }
                }

                break;
        }

        return io;
    }

    private void DoPlay()
    {
        List<RoadIO> ioList = GetUnconnectedIO(pivotIO);
        if (ioList.Count != 2)
        {
            Debug.Log("Invalid road");
            return;
        }

        RoadInput firstInput = null;
        RoadOutput lastOutput = null;
        foreach (RoadIO rio in ioList)
        {
            if (rio.IsInputOrOutput() == InputOutput.Input)
            {
                firstInput = (RoadInput)rio;
            }
            else
            {
                lastOutput = (RoadOutput)rio;
            }
        }

        if (firstInput == null || lastOutput == null)
        {
            Debug.Log("Invalid road");
            return;
        }
        if (AreConditionsNotSet(pivotIO))
        {
            Debug.Log("Some ifs have no condition card");
            return;
        }
        GameObject player = LevelManager.instance.LevelObjects.GetMainCharacterInstance();
        player.transform.position = new Vector3(firstInput.transform.position.x, firstInput.transform.position.y + 0.5f, firstInput.transform.position.z);
        navmeshExtension.transform.position = firstInput.transform.position;
        LevelManager.instance.RoadMovementLogic.StartMovement(firstInput, lastOutput, player, conditionDictionary, loopsDictionary);
    }

    private bool AreConditionsNotSet(RoadIO startPoint)
    {
        if (conditionDictionary.ContainsValue(Cards.NoCard))
        {
            return true;
        }
        return false;
    }

    private List<RoadIO> GetUnconnectedIO(RoadIO startPoint)
    {
        List<RoadIO> result = new List<RoadIO>();
        if (startPoint != null)
        {
            List<Road> processedRoads = new List<Road>();

            Stack<RoadIO> ioToProc = new Stack<RoadIO>();

            RoadIO[] tmpe = startPoint.GetParentRoad().InputsAndOutputs;

            foreach (RoadIO rio in tmpe)
            {
                ioToProc.Push(rio);
            }

            while (ioToProc.Count > 0)
            {
                RoadIO toProc = ioToProc.Pop();

                processedRoads.Add(toProc.GetParentRoad());
                RoadIO connectedTo = toProc.ConnectedTo();
                if (connectedTo == null)
                {
                    if (!result.Contains(toProc))
                    {
                        result.Add(toProc);
                    }
                }
                else
                {
                    Road nextRoad = connectedTo.GetParentRoad();
                    if (!processedRoads.Contains(nextRoad))
                    {
                        tmpe = nextRoad.InputsAndOutputs;
                        foreach (RoadIO candidate in tmpe)
                        {
                            ioToProc.Push(candidate);
                        }
                    }
                }
            }
        }
        return result;
    }

    private void DoRestart()
    {
    }

    private void DoTurnLeft()
    {
        SpawnAndPlaceAction(Buttons.TurnLeft);
    }

    private void DoTurnRight()
    {
        SpawnAndPlaceAction(Buttons.TurnRight);
    }

    private Road SpawnRoad(RoadType roadType)
    {
        return LevelManager.instance.RoadFactory.GetRoadInstance(roadType);
    }

    private Road SpawnRoad(Buttons button)
    {
        return LevelManager.instance.RoadFactory.GetVerticalConnectorWithButton(button);
    }

    public void InformOfCardChanged(Road road, Cards newCard)
    {
        if (conditionDictionary.ContainsKey(road))
        {
            Debug.Log("Changed to card " + newCard.ToString() + " on road " + road.RoadType.ToString());
            conditionDictionary[road] = newCard;
        }
    }

    public void InformOfLoopRepsChanged(Road road, int newReps)
    {
        if (loopsDictionary.ContainsKey(road))
        {
            Debug.Log("Changed to number " + newReps + " on road " + road.RoadType.ToString());
            loopsDictionary[road] = newReps;
        }
    }
}