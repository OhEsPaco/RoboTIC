using System;
using System.Collections.Generic;
using UnityEngine;
using static ButtonConstants;
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
        SpawnVerticalButton("action");
    }

    private void DoCondition()
    {
        Road loopIn;
        /*Vector3 loopPos = roadStartMarker.transform.position;
        if (selectedIO != null)
        {
            loopPos = new Vector3(this.selectedIO.transform.position.x, this.selectedIO.transform.position.y, this.selectedIO.transform.position.z);
        }*/
        if (SpawnRoad("NodeIfIn", out loopIn))
        {
            
            Road loopOut;
            LevelManager.instance.RoadFactory.SpawnRoadByName("NodeIfOut", loopIn.GetRoadIOByDirection(RoadIO.IODirection.Forward), out loopOut);
            loopOut.transform.parent = roadParent;
           
            loopOut.GetRoadIOByDirection(IODirection.Back)[0].MoveRoadTo(loopOut.GetRoadIOByDirection(IODirection.Back)[0].connectedTo.transform.position);

            //FillGaps(this.pivotIO, MAX_ACCEPTABLE_DISTANCE);
        }

        /*//Intenta spawnear la carretera que se le pide en el conjunto de io que se le pasa
     public bool SpawnAndConnectRoad(in Road roadToSpawn, in List<RoadIO> ioToMatch, in float errorMargin, out Road spawnedRoad)
     */
        /*  Road ifIn;
          if (LevelManager.instance.RoadFactory.SpawnRoadByName("NodeIfIn", out ifIn))
          {
              Road ifOut;
              if (LevelManager.instance.RoadFactory.SpawnAndConnectRoad("NodeIfOut", ifIn.GetRoadIOByDirection(IODirection.Forward),MAX_ACCEPTABLE_DISTANCE, out ifOut))
              {
              }
          }*/
    }

    private void DoJump()
    {
        SpawnVerticalButton("jump");
    }

    private void DoLoop()
    {
        Road loopIn;
        /*Vector3 loopPos = roadStartMarker.transform.position;
        if (selectedIO != null)
        {
            loopPos = new Vector3(this.selectedIO.transform.position.x, this.selectedIO.transform.position.y, this.selectedIO.transform.position.z);
        }*/
        if (SpawnRoad("NodeLoopIn", out loopIn))
        {
       
            Road loopOut;
           
            LevelManager.instance.RoadFactory.SpawnRoadByName("NodeLoopOut", loopIn.GetRoadIOByDirection(RoadIO.IODirection.Forward), out loopOut);
   
            loopOut.transform.parent = roadParent;
           
            loopOut.GetRoadIOByDirection(IODirection.Back)[0].MoveRoadTo(loopOut.GetRoadIOByDirection(IODirection.Back)[0].connectedTo.transform.position);

            //FillGaps(this.pivotIO, MAX_ACCEPTABLE_DISTANCE);
        }
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

                this.pivotIO.MoveRoadTo(roadStartMarker.transform.position);
            }
        }
        else
        {
            //Buscar los io en el mismo lado y conectarlos
            if (this.selectedIO.connectedTo == null)
            {
                List<RoadIO> ioToMatch = new List<RoadIO>();
                ioToMatch.Add(this.selectedIO);

                if (LevelManager.instance.RoadFactory.SpawnRoadByName(id, ioToMatch, out spawnedRoad))
                {
                    spawnedRoad.transform.parent = roadParent;
                   
                }

                this.selectedIO.connectedTo.MoveRoadTo(this.selectedIO.transform.position);
            }
            else
            {
                List<RoadIO> ioToMatch = new List<RoadIO>();
                ioToMatch.Add(this.selectedIO);

                List<RoadIO> ioToMatch2 = new List<RoadIO>();
                ioToMatch2.Add(this.selectedIO.connectedTo);

                if (LevelManager.instance.RoadFactory.SpawnRoadByName(id, ioToMatch, ioToMatch2, out spawnedRoad))
                {
                    spawnedRoad.transform.parent = roadParent;
                  
                    this.selectedIO.connectedTo.MoveRoadTo(this.selectedIO.transform.position);
                    //Hacer la magia

                    //Guardo la direccion en la que se ha puesto esta carretera
                    IODirection newRoadDir = this.selectedIO.Direction;
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
                    foreach (RoadIO r in spawnedRoad.GetAllIO())
                    {
                        if (r.connectedTo != null)
                        {
                            ioToProccess.Push(r);
                        }
                    }

                    processedRoads.Add(spawnedRoad);
                    //Marco la nueva carretera con un 0
                    roadAndValue.Add(spawnedRoad, 0);

                    while (ioToProccess.Count > 0)
                    {
                        //Tomamos la io a procesar
                        RoadIO currentIO = ioToProccess.Pop();

                        //La añadimos a la lista de procesadas
                        processedIO.Add(currentIO);

                        if (currentIO.connectedTo != null)
                        {
                            RoadIO connectedTo = currentIO.connectedTo;
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
                                    if (Vector3.Distance(r.transform.position, r.connectedTo.transform.position) > MAX_ACCEPTABLE_DISTANCE)
                                    {
                                        currentRoadIO.Add(r);
                                        nextRoadIO.Add(r.connectedTo);
                                    }

                                    if (currentRoadIO.Count > 0)
                                    {
                                        Road gap;
                                        if (LevelManager.instance.RoadFactory.FillGap(nextRoadIO, currentRoadIO, out gap))
                                        {

                                            processedRoads.Add(gap);
                                            gap.transform.parent = roadParent;
                                      
                                            nextRoadIO[0].connectedTo.MoveRoadTo(nextRoadIO[0].transform.position);
                                        }
                                    }
                                }
                                //currentIO.GetParentRoad().GetRoadIOByDirection(currentIO.Direction);
                            }
                            else
                            {
                                //Si es mayor o igual que cero seguimos
                                if (!roadAndValue.ContainsKey(connectedTo.GetParentRoad()))
                                {
                                    //Si no contiene esta carretera, la añadimos
                                    roadAndValue.Add(connectedTo.GetParentRoad(), nextRoadLevel);
                                }
                                else
                                {
                                    //Si la contiene, modificamos el valor de esta carretera si es menor que el que tiene
                                    /*if (roadAndValue[connectedTo.GetParentRoad()] > nextRoadLevel)
                                    {
                                        roadAndValue[connectedTo.GetParentRoad()] = nextRoadLevel;
                                    }*/
                                }

                                //Movemos la nueva carretera a su posicion
                                if (!processedRoads.Contains(connectedTo.GetParentRoad()))
                                {
                                    connectedTo.MoveRoadTo(currentIO.transform.position);
                                    processedRoads.Add(connectedTo.GetParentRoad());
                                }

                                //Añadimos nueva io
                                foreach (RoadIO r in connectedTo.GetParentRoad().GetAllIO())
                                {
                                    if (r.connectedTo != null)
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
                }
            }
        }

        if (spawnedRoad == null)
        {
            return false;
        }
        else
        {
            
            if (this.selectedIO is RoadInput)
            {
                RoadIO f = FindFartestInput(this.selectedIO, spawnedRoad.GetAllIO());
                if (f != null)
                {
                    this.selectedIO = f;
                    this.pivotIO = f;
                }
            }

            this.selectedOutputMarker.transform.position = this.selectedIO.transform.position;
            return true;
        }
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
                    if (rIO.connectedTo != null)
                    {
                        if (!allRoads.Contains(rIO.connectedTo.GetParentRoad()))
                        {
                            roadsToProccess.Push(rIO.connectedTo.GetParentRoad());
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
                RoadIO connectedTo = toProc.ConnectedTo;
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
        SpawnVerticalButton("turn_left");
    }

    private void DoTurnRight()
    {
        SpawnVerticalButton("turn_right");
    }
}