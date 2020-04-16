using System;
using System.Collections.Generic;
using UnityEngine;
using static LevelButtons;
using static RoadIO;

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
        selectedOutputMarker.transform.parent = roadParent;
        roadParent.position = roadStartMarker.position;
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
        //selectedOutputMarker.transform.parent = roadStartMarker;
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
        SpawnVerticalButton(Buttons.Action);
    }

    private void DoCondition()
    {
        string[] ids = { "NodeIfIn", "NodeIfOut" };
        Road[] spawnedRoad;
        if (SpawnRoads(ids, IODirection.Forward, out spawnedRoad))
        {
        }
    }

    private void DoJump()
    {
        SpawnVerticalButton(Buttons.Jump);
    }

    private void DoLoop()
    {
        string[] ids = { "NodeLoopIn", "NodeLoopOut" };
        Road[] spawnedRoad;
        if (SpawnRoads(ids, IODirection.Forward, out spawnedRoad))
        {
        }
    }

    private bool ConnectRoads(in Road road1, in Road road2, in Dictionary<string, string> ioR1_ioR2)
    {
        bool success = true;
        //Conectamos una carretera a la otra
        foreach (KeyValuePair<string, string> entry in ioR1_ioR2)
        {
            RoadIO r1IO = road1.GetRoadIOByID(entry.Key);
            RoadIO r2IO = road2.GetRoadIOByID(entry.Value);

            if (r1IO != null && r2IO != null)
            {
                r1IO.ConnectedTo = r2IO;
            }
            else
            {
                Debug.LogError("Impossible to connect this roads");
                success = false;
                break;
            }
        }

        if (!success)
        {
            //Desconectamos todo
            foreach (KeyValuePair<string, string> entry in ioR1_ioR2)
            {
                RoadIO r1IO = road1.GetRoadIOByID(entry.Key);
                RoadIO r2IO = road2.GetRoadIOByID(entry.Value);

                if (r1IO != null)
                {
                    r1IO.ConnectedTo = null;
                }

                if (r2IO != null)
                {
                    r2IO.ConnectedTo = null;
                }
            }
        }

        return success;
    }

    //Tiene que tener un solo input y un solo output
    //La direccion es hacia la que se va a poner la carretera
    private bool ValidRoadSequence(in Road[] roads, IODirection direction)
    {
        //Si no tiene carreteras no es una secuencia válida
        if (roads.Length == 0)
        {
            return false;
        }

        //La primera solo puede tener un io en dirección opuesta
        if (roads[0].GetRoadIOByDirection(RoadIO.GetOppositeDirection(direction)).Count != 1)
        {
            return false;
        }

        //La ultima solo puede tener un io en esta direccion
        if (roads[roads.Length - 1].GetRoadIOByDirection(direction).Count != 1)
        {
            return false;
        }

        for (int i = 0; i < roads.Length - 1; i++)
        {
            Road thisRoad = roads[i];
            Road nextRoad = roads[i + 1];
            if (thisRoad.GetRoadIOByDirection(direction).Count != nextRoad.GetRoadIOByDirection(RoadIO.GetOppositeDirection(direction)).Count)
            {
                return false;
            }
        }

        return true;
    }

    //Genera y conecta una serie de carreteras
    private bool GenerateRoads(in string[] ids, in IODirection direction, in Vector3 position, out Road[] spawnedRoads)
    {
        spawnedRoads = new Road[ids.Length];

        if (ids.Length == 0)
        {
            return false;
        }

        //Generamos la primera
        Road spw;
        if (LevelManager.instance.RoadFactory.SpawnRoadByID(ids[0], out spw))
        {
            spawnedRoads[0] = Instantiate(spw);
            spawnedRoads[0].transform.parent = roadParent;
            spawnedRoads[0].GetRoadIOByDirection(RoadIO.GetOppositeDirection(direction))[0].MoveRoadTo(position);
        }
        else
        {
            return false;
        }

        for (int i = 0; i < ids.Length - 1; i++)
        {
            Road thisRoad = spawnedRoads[i];
            List<RoadIO> ioToMatch = thisRoad.GetRoadIOByDirection(direction);
            Road nextRoad;
            Dictionary<string, string> connectionsR_C;

            if (LevelManager.instance.RoadFactory.SpawnRoadByID(ids[i + 1], ioToMatch, out nextRoad, out connectionsR_C))
            {
                spawnedRoads[i + 1] = Instantiate(nextRoad);
                spawnedRoads[i + 1].transform.parent = roadParent;
                if (!ConnectRoads(spawnedRoads[i], spawnedRoads[i + 1], connectionsR_C))
                {
                    DestroyRoads(spawnedRoads);
                    return false;
                }
                else
                {
                    RoadIO io = spawnedRoads[i + 1].GetRoadIOByDirection(RoadIO.GetOppositeDirection(direction))[0];
                    io.MoveRoadTo(io.connectedTo.transform.position);
                }
            }
            else
            {
                DestroyRoads(spawnedRoads);
                return false;
            }
        }

        foreach (Road r in spawnedRoads)
        {
            r.transform.parent = roadParent;
        }

        return true;
    }

    private void DestroyRoads(Road[] roads)
    {
        for (int i = 0; i < roads.Length; i++)
        {
            if (roads[i] != null)
            {
                Destroy(roads[i]);
            }
        }
    }

    private bool SpawnRoads(in string[] ids, in IODirection direction, out Road[] spawnedRoads)
    {
        Vector3 pos = this.selectedIO != null?this.selectedIO.transform.position:roadStartMarker.position;

        if (!GenerateRoads(ids,direction,pos, out spawnedRoads))
        {
            return false;
        }

        RoadIO roadIOL = null;
        RoadIO roadIOR = null;

        //Si hay io seleccionada
        if (this.selectedIO != null)
        {
            roadIOL = this.selectedIO;

            if (roadIOL.ConnectedTo != null)
            {
                roadIOR = this.selectedIO.ConnectedTo;
            }

            roadIOL.ConnectedTo = spawnedRoads[0].GetRoadIOByDirection(RoadIO.GetOppositeDirection(direction))[0];

            if (roadIOR != null)
            {
                roadIOR.ConnectedTo = spawnedRoads[spawnedRoads.Length - 1].GetRoadIOByDirection(direction)[0];
            }

            this.pivotIO = roadIOL.ConnectedTo;
            roadIOL.ConnectedTo.MoveRoadTo(roadIOL.transform.position);
        }
        else
        {
            //Si no hay io seleccionada
            this.selectedIO = spawnedRoads[0].GetRoadIOByDirection(RoadIO.GetOppositeDirection(direction))[0];
            this.pivotIO = this.selectedIO;
        }


        int numberOfPiecesGap = spawnedRoads.Length;

        //Hacer la magia

        //Guardo la direccion en la que se ha puesto esta carretera
        IODirection newRoadDir = direction;
        IODirection oppositeDirection = RoadIO.GetOppositeDirection(newRoadDir);
        //Cada vez que se pasa a una carretera por esa direccion, se suma un nivel
        //En direccion contraria se resta
        //Y en el resto se queda igual

        //Creo un diccionario de carreteras y el numero que tienen
        Dictionary<Road, int> roadAndValue = new Dictionary<Road, int>();

        //Lista de IO procesadas
        List<RoadIO> processedIO = new List<RoadIO>();
        List<Road> processedRoads = new List<Road>();

        //Stack de IO a procesar
        Stack<RoadIO> ioToProccess = new Stack<RoadIO>();

        //Añado el IO de la nueva carretera a la pila
        foreach (RoadIO r in spawnedRoads[spawnedRoads.Length - 1].GetAllIO())
        {
            if (r.ConnectedTo != null)
            {
                ioToProccess.Push(r);
            }
        }

        processedRoads.Add(spawnedRoads[spawnedRoads.Length - 1]);
        //Marco la nueva carretera con un 0
        roadAndValue.Add(spawnedRoads[spawnedRoads.Length - 1], 0);

        while (ioToProccess.Count > 0)
        {
            //Tomamos la io a procesar
            RoadIO currentIO = ioToProccess.Pop();

            //La añadimos a la lista de procesadas
            processedIO.Add(currentIO);

            if (currentIO.ConnectedTo != null)
            {
                RoadIO ConnectedTo = currentIO.ConnectedTo;
                int nextRoadLevel = roadAndValue[currentIO.GetParentRoad()];

                if (currentIO.Direction == newRoadDir)
                {
                    //La siguiente carretera suma
                    nextRoadLevel++;
                }
                else if (currentIO.Direction == oppositeDirection)
                {
                    //La siguiente carretera resta
                    nextRoadLevel--;
                }
                else
                {
                    ////Queda igual
                }

                //Si es menor que cero hemos encontrado un hueco
                if (nextRoadLevel <= 0)
                {
                    Debug.Log("Hueco");
                    //MAX_ACCEPTABLE_DISTANCE
                    //Llenamos el hueco
                    List<RoadIO> currentRoadIO = new List<RoadIO>();
                    List<RoadIO> nextRoadIO = new List<RoadIO>();

                    foreach (RoadIO r in currentIO.GetParentRoad().GetRoadIOByDirection(currentIO.Direction))
                    {
                        if (Vector3.Distance(r.transform.position, r.ConnectedTo.transform.position) > MAX_ACCEPTABLE_DISTANCE)
                        {
                            currentRoadIO.Add(r);
                            nextRoadIO.Add(r.ConnectedTo);
                        }

                        if (currentRoadIO.Count > 0)
                        {
                            Road gap;
                            //FillGapWithConnector(in List<RoadIO> ioToMatch, in List<RoadIO> ioToMatch2, out Road road, out Dictionary<string, string> connectionsR1_Connector, out Dictionary<string, string> connectionsR2_Connector)
                            Dictionary<string, string> connectionsR1_Connector;
                            Dictionary<string, string> connectionsR2_Connector;
                            // GenerateRoads(in string[] ids, in IODirection direction, out Road[] spawnedRoads)

                            if (LevelManager.instance.RoadFactory.FillGapWithConnector(nextRoadIO, currentRoadIO, out gap, out connectionsR1_Connector, out connectionsR2_Connector))
                            {
                                // processedRoads.Add(gap);
                                // gap.transform.parent = roadParent;

                                string[] idsGap = new string[numberOfPiecesGap];
                                for(int i = 0; i < idsGap.Length; i++)
                                {
                                    idsGap[i] = gap.RoadIdentifier;
                                }

                                Road[] spanedRoads;
                                if(GenerateRoads(idsGap, nextRoadIO[0].Direction, nextRoadIO[0].transform.position,out spanedRoads))
                                {
                                    ConnectRoads(nextRoadIO[0].GetParentRoad(), spanedRoads[0], connectionsR1_Connector);
                                    ConnectRoads(currentRoadIO[0].GetParentRoad(), spanedRoads[spanedRoads.Length - 1], connectionsR2_Connector);
                                    foreach(Road newR in spanedRoads)
                                    {
                                        processedRoads.Add(newR);
                                    }
                                }


                                //nextRoadIO[0].ConnectedTo.MoveRoadTo(nextRoadIO[0].transform.position);
                            }
                        }
                    }
                    //currentIO.GetParentRoad().GetRoadIOByDirection(currentIO.Direction);
                }
                else
                {
                    //Si es mayor o igual que cero seguimos
                    if (!roadAndValue.ContainsKey(ConnectedTo.GetParentRoad()))
                    {
                        //Si no contiene esta carretera, la añadimos
                        roadAndValue.Add(ConnectedTo.GetParentRoad(), nextRoadLevel);
                    }
                    else
                    {
                        //Si la contiene, modificamos el valor de esta carretera si es menor que el que tiene
                        /*if (roadAndValue[ConnectedTo.GetParentRoad()] > nextRoadLevel)
                        {
                            roadAndValue[ConnectedTo.GetParentRoad()] = nextRoadLevel;
                        }*/
                    }

                    //Movemos la nueva carretera a su posicion
                    if (!processedRoads.Contains(ConnectedTo.GetParentRoad()))
                    {
                        ConnectedTo.MoveRoadTo(currentIO.transform.position);
                        processedRoads.Add(ConnectedTo.GetParentRoad());
                    }

                    //Añadimos nueva io
                    foreach (RoadIO r in ConnectedTo.GetParentRoad().GetAllIO())
                    {
                        if (r.ConnectedTo != null)
                        {
                            if (!processedIO.Contains(r))
                            {
                                ioToProccess.Push(r);
                            }
                        }
                    }
                }
            }
        }




        return true;
    }

    private RoadIO FindFartestInput(RoadIO pivot, RoadIO[] allIO)
    {
        RoadIO fartest = allIO[0];
        foreach (RoadIO rIO in allIO)
        {
            if (fartest is RoadOutput && rIO is RoadInput && rIO.CanBeSelected)
            {
                fartest = rIO;
            }
            else if (rIO is RoadInput && rIO.CanBeSelected)
            {
                if (Vector3.Distance(pivot.transform.position, fartest.transform.position) < Vector3.Distance(pivot.transform.position, rIO.transform.position))
                {
                    fartest = rIO;
                }
            }
        }

        if (fartest is RoadOutput || !fartest.CanBeSelected)
        {
            return null;
        }
        return fartest;
    }

    private void SpawnVerticalButton(Buttons button)
    {
        string[] ids = { "NodeVerticalButton" };
        Road[] spawnedRoad;
        if (SpawnRoads(ids, IODirection.Forward, out spawnedRoad))
        {
            string[] args = { "activate", button.ToString() };
            spawnedRoad[0].ExecuteAction(args);
        }
    }

    private void DoMove()
    {
        SpawnVerticalButton(Buttons.Move);
    }

    private void DoPlay()
    {
        //LevelManager.instance.RoadMovementLogic.StartMovement(firstInput, lastOutput, player, conditionDictionary, loopsDictionary);
        //Quiza se podrian acelerar estas cosas añadiendo un flag de procesamiento a cada RoadIO y reseteandolo antes de empezar el algoritmo

        if (pivotIO != null)
        {
            List<Road> allRoads = new List<Road>();
            Stack<Road> roadsToProccess = new Stack<Road>();

            RoadInput roadInput = null;
            RoadOutput roadOutput = null;

            roadsToProccess.Push(pivotIO.GetParentRoad());
            while (roadsToProccess.Count > 0)
            {
                Road r = roadsToProccess.Pop();

                foreach (RoadIO rIO in r.GetAllIO())
                {
                    if (rIO.ConnectedTo != null)
                    {
                        if (!allRoads.Contains(rIO.ConnectedTo.GetParentRoad()))
                        {
                            roadsToProccess.Push(rIO.ConnectedTo.GetParentRoad());
                        }
                    }
                    else
                    {
                        if (rIO is RoadInput)
                        {
                            roadInput = (RoadInput)rIO;
                        }
                        else
                        {
                            roadOutput = (RoadOutput)rIO;
                        }
                    }
                }

                if (!allRoads.Contains(r))
                {
                    allRoads.Add(r);
                }
            }

            Debug.Log("Number of roads: " + allRoads.Count);
            Debug.Log("RoadInput: " + roadInput.IOIdentifier);
            Debug.Log("RoadOutput: " + roadOutput.IOIdentifier);

            bool invalidRoad = false;
            foreach (Road r in allRoads)
            {
                if (!r.RoadReady())
                {
                    invalidRoad = true;
                    Debug.LogError(r.RoadIdentifier + " not ready");
                }
            }
            //Comprobar condiciones

            if (!invalidRoad)
            {
                //Lock all roads
                string[] lockArgs = { "lock" };
                foreach (Road r in allRoads)
                {
                    r.ExecuteAction(lockArgs);
                }
                selectedOutputMarker.SetActive(false);

                LevelManager.instance.RoadMovement.StartMovement(roadInput, roadOutput);
            }
            else
            {
                InvalidRoad();
            }
        }
        else
        {
            InvalidRoad();
        }
    }

    private void InvalidRoad()
    {
        Debug.LogError("Invalid Road");
    }

    private List<RoadIO> GetUnconnectedIO(RoadIO startPoint)
    {
        List<RoadIO> result = new List<RoadIO>();
        if (startPoint != null)
        {
            List<Road> processedRoads = new List<Road>();

            Stack<RoadIO> ioToProc = new Stack<RoadIO>();

            RoadIO[] tmpe = startPoint.GetParentRoad().GetAllIO();

            foreach (RoadIO rio in tmpe)
            {
                ioToProc.Push(rio);
            }

            while (ioToProc.Count > 0)
            {
                RoadIO toProc = ioToProc.Pop();

                processedRoads.Add(toProc.GetParentRoad());
                RoadIO ConnectedTo = toProc.ConnectedTo;
                if (ConnectedTo == null)
                {
                    if (!result.Contains(toProc))
                    {
                        result.Add(toProc);
                    }
                }
                else
                {
                    Road nextRoad = ConnectedTo.GetParentRoad();
                    if (!processedRoads.Contains(nextRoad))
                    {
                        tmpe = nextRoad.GetAllIO();
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
        SpawnVerticalButton(Buttons.TurnLeft);
    }

    private void DoTurnRight()
    {
        SpawnVerticalButton(Buttons.TurnRight);
    }
}