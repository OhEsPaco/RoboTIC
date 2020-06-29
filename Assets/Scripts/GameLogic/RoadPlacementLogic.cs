using System;
using System.Collections.Generic;
using UnityEngine;
using static LevelButtons;
using static RoadIO;

public class RoadPlacementLogic : MonoBehaviour
{
    [SerializeField] private Transform roadStartMarker;
    [SerializeField] private SelectedOutputMarker selectedOutputMarker;
    [SerializeField] private Transform roadParent;
    [SerializeField] private Road roadStart;
    [SerializeField] private MiniCharacter minibot;

    private Dictionary<Buttons, Action> buttonActionsDictionary;
    private RoadFactory roadFactory;

    private RoadIO selectedIO = null;

    private RoadIO firstInput = null;

    public RoadIO FirstInput
    {
        get
        {
            return firstInput;
        }
    }

    //Para escala 1 funcionaba bien 0.3, asi que para escala 0.3 lo multiplico por ella
    private const float MAX_ACCEPTABLE_DISTANCE = 0.3f * 0.3f;

    [SerializeField] private GameObject placeableMap;
    public RoadIO SelectedIO { get => selectedIO; set => selectedIO = value; }
    private Stack<RoadChanges> undoStack = new Stack<RoadChanges>();

    private void Awake()
    {
        roadFactory = GetComponentInChildren<RoadFactory>();
        //Mejor sin usar mensajes porque es imprescindible que la logica vaya sincronizada
        //EventAggregator.Instance.Subscribe<MsgAddInputFromButtonRoadPlacement>(AddInputFromButton);
    }

    private static RoadPlacementLogic roadPlacementLogic;

    public static RoadPlacementLogic Instance
    {
        get
        {
            if (!roadPlacementLogic)
            {
                roadPlacementLogic = FindObjectOfType(typeof(RoadPlacementLogic)) as RoadPlacementLogic;

                if (!roadPlacementLogic)
                {
                    Debug.LogError("There needs to be one active RoadPlacementLogic script on a GameObject in your scene.");
                }
            }

            return roadPlacementLogic;
        }
    }

    private void Start()
    {
        selectedOutputMarker.transform.parent = roadParent;
        roadStart.transform.parent = roadParent;
        roadParent.position = roadStartMarker.position;

        firstInput = roadStart.GetRoadIOByDirection(IODirection.Back)[0];
        firstInput.MoveRoadTo(roadStartMarker.position);
        this.selectedIO = roadStart.GetRoadIOByDirection(IODirection.Forward)[0];

        selectedOutputMarker.transform.position = this.selectedIO.transform.position;
        minibot.transform.parent = roadParent;
        minibot.transform.position = firstInput.transform.position;
        minibot.gameObject.SetActive(true);
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
        buttonActionsDictionary.Add(Buttons.MapMenu, MapMenu);
    }

    private void MapMenu()
    {
        ResetRoad();
        GameLogic.Instance.AddInputFromButton(Buttons.MapMenu);
    }

    private class RoadChanges
    {
        public List<Road> addedRoads = new List<Road>();
        public Dictionary<RoadIO, RoadIO> connectionsChanged = new Dictionary<RoadIO, RoadIO>();
        public Tuple<NodeVerticalButton, VerticalButton> addedButton = null;
        public RoadIO selectedIOBack = null;
    }

    private void DoUndo()
    {
        if (undoStack.Count > 0)
        {
            RoadChanges thisChanges = undoStack.Pop();
            //Delete added roads
            foreach (Road r in thisChanges.addedRoads)
            {
                Destroy(r.gameObject);
            }

            //Revert connections
            foreach (KeyValuePair<RoadIO, RoadIO> entry in thisChanges.connectionsChanged)
            {
                entry.Key.ConnectedTo = entry.Value;
                // entry.Value.ConnectedTo = entry.Key;
            }

            //Delete added buttons
            if (thisChanges.addedButton != null)
            {
                thisChanges.addedButton.Item1.DestroyButton(thisChanges.addedButton.Item2);
            }

            //Move marker to nearby io
            if (thisChanges.selectedIOBack)
            {
                this.selectedIO = thisChanges.selectedIOBack;
                selectedOutputMarker.transform.position = this.SelectedIO.transform.position;
            }
            else
            {
                selectedOutputMarker.FindAndSelectClosestIO();
            }

            //Move roads to correct positions
            CorrectPositions(MAX_ACCEPTABLE_DISTANCE, firstInput);
        }
    }

    public void AddInputFromButton(Buttons buttonIndex)
    {
        //try
        //{
        buttonActionsDictionary[buttonIndex]();
        /*}
        catch
        {
            Debug.LogError("Unknown input: " + buttonIndex.ToString());
        }*/
    }

    private void DoAction()
    {
        if (EnoughNumberOfInstrucions(Buttons.Action))
        {
            SpawnVerticalButton(Buttons.Action);
        }
    }

    private bool EnoughNumberOfInstrucions(Buttons button)
    {
        if (Application.isEditor)
        {
            //return true;
        }
        if (GameLogic.Instance.CurrentLevelData == null)
        {
            return false;
        }
        bool updateInstructions = false;

        switch (button)
        {
            case Buttons.Action:

                if (GameLogic.Instance.CurrentLevelData.availableInstructions.action > 0)
                {
                    GameLogic.Instance.CurrentLevelData.availableInstructions.action--;
                    updateInstructions = true;
                }
                break;

            case Buttons.Condition:

                if (GameLogic.Instance.CurrentLevelData.availableInstructions.condition > 0)
                {
                    GameLogic.Instance.CurrentLevelData.availableInstructions.condition--;
                    updateInstructions = true;
                }

                break;

            case Buttons.Jump:
                if (GameLogic.Instance.CurrentLevelData.availableInstructions.jump > 0)
                {
                    GameLogic.Instance.CurrentLevelData.availableInstructions.jump--;
                    updateInstructions = true;
                }

                break;

            case Buttons.Loop:

                if (GameLogic.Instance.CurrentLevelData.availableInstructions.loop > 0)
                {
                    GameLogic.Instance.CurrentLevelData.availableInstructions.loop--;
                    updateInstructions = true;
                }
                break;

            case Buttons.Move:

                if (GameLogic.Instance.CurrentLevelData.availableInstructions.move > 0)
                {
                    GameLogic.Instance.CurrentLevelData.availableInstructions.move--;
                    updateInstructions = true;
                }
                break;

            case Buttons.TurnLeft:

                if (GameLogic.Instance.CurrentLevelData.availableInstructions.turnLeft > 0)
                {
                    GameLogic.Instance.CurrentLevelData.availableInstructions.turnLeft--;
                    updateInstructions = true;
                }
                break;

            case Buttons.TurnRight:

                if (GameLogic.Instance.CurrentLevelData.availableInstructions.turnRight > 0)
                {
                    GameLogic.Instance.CurrentLevelData.availableInstructions.turnRight--;
                    updateInstructions = true;
                }
                break;
        }
        if (updateInstructions)
        {
            GameLogic.Instance.UpdateAvailableInstructions();
        }
        return updateInstructions;
    }

    private void DoCondition()
    {
        if (EnoughNumberOfInstrucions(Buttons.Condition))
        {
            //No se pueden poner ifs dentro de ifs
            bool foundIf = false;
            if (this.selectedIO != null)
            {
                List<RoadIO> processedIO = new List<RoadIO>();
                Stack<RoadIO> ioToProc = new Stack<RoadIO>();

                foreach (RoadIO io in this.selectedIO.GetParentRoad().GetRoadIOByDirection(IODirection.Back))
                {
                    ioToProc.Push(io);
                }

                while (ioToProc.Count > 0 && !foundIf)
                {
                    RoadIO thisIO = ioToProc.Pop();
                    processedIO.Add(thisIO);
                    Debug.Log(thisIO.GetParentRoad().RoadIdentifier);
                    if (thisIO.GetParentRoad().RoadIdentifier.Contains("NodeIfIn"))
                    {
                        foundIf = true;
                    }
                    else if (!thisIO.GetParentRoad().RoadIdentifier.Contains("NodeIfOut"))
                    {
                        if (thisIO.ConnectedTo != null)
                        {
                            foreach (RoadIO io in thisIO.ConnectedTo.GetParentRoad().GetRoadIOByDirection(IODirection.Back))
                            {
                                if (!processedIO.Contains(io))
                                {
                                    ioToProc.Push(io);
                                }
                            }
                        }
                    }
                }
            }

            if (!foundIf)
            {
                RoadIO selectedIOBack = this.selectedIO;
                string[] ids = { "NodeIfIn", "NodeIfOut" };
                Road[] spawnedRoad;
                List<Road> extraRoads;
                Dictionary<RoadIO, RoadIO> oldConnections;
                if (SpawnRoads(ids, IODirection.Forward, out spawnedRoad, out extraRoads, out oldConnections))
                {
                    NewActionOnStack(spawnedRoad, extraRoads, oldConnections, selectedIOBack);
                }
            }
        }
    }

    private void DoJump()
    {
        if (EnoughNumberOfInstrucions(Buttons.Jump))
        {
            SpawnVerticalButton(Buttons.Jump);
        }
    }

    private void DoLoop()
    {
        if (EnoughNumberOfInstrucions(Buttons.Loop))
        {
            //No se pueden poner loops dentro de ifs
            bool foundIf = false;
            if (this.selectedIO != null)
            {
                List<RoadIO> processedIO = new List<RoadIO>();
                Stack<RoadIO> ioToProc = new Stack<RoadIO>();

                foreach (RoadIO io in this.selectedIO.GetParentRoad().GetRoadIOByDirection(IODirection.Back))
                {
                    ioToProc.Push(io);
                }

                while (ioToProc.Count > 0 && !foundIf)
                {
                    RoadIO thisIO = ioToProc.Pop();
                    processedIO.Add(thisIO);
                    Debug.Log(thisIO.GetParentRoad().RoadIdentifier);
                    if (thisIO.GetParentRoad().RoadIdentifier.Contains("NodeIfIn"))
                    {
                        foundIf = true;
                    }
                    else if (!thisIO.GetParentRoad().RoadIdentifier.Contains("NodeIfOut"))
                    {
                        if (thisIO.ConnectedTo != null)
                        {
                            foreach (RoadIO io in thisIO.ConnectedTo.GetParentRoad().GetRoadIOByDirection(IODirection.Back))
                            {
                                if (!processedIO.Contains(io))
                                {
                                    ioToProc.Push(io);
                                }
                            }
                        }
                    }
                }
            }

            if (!foundIf)
            {
                RoadIO selectedIOBack = this.selectedIO;
                string[] ids = { "NodeLoopIn", "NodeLoopOut" };
                Road[] spawnedRoad;
                List<Road> extraRoads;
                Dictionary<RoadIO, RoadIO> oldConnections;
                if (SpawnRoads(ids, IODirection.Forward, out spawnedRoad, out extraRoads, out oldConnections))
                {
                    NewActionOnStack(spawnedRoad, extraRoads, oldConnections, selectedIOBack);
                }
            }
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
                r2IO.ConnectedTo = r1IO;
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
        if (roadFactory.SpawnRoadByID(ids[0], out spw))
        {
            spawnedRoads[0] = Instantiate(spw, roadStart.transform.localPosition, roadStart.transform.rotation);

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

            if (roadFactory.SpawnRoadByID(ids[i + 1], ioToMatch, out nextRoad, out connectionsR_C))
            {
                spawnedRoads[i + 1] = Instantiate(nextRoad, roadStart.transform.localPosition, roadStart.transform.rotation);

                spawnedRoads[i + 1].transform.parent = roadParent;
                if (!ConnectRoads(spawnedRoads[i], spawnedRoads[i + 1], connectionsR_C))
                {
                    DestroyRoads(spawnedRoads);
                    return false;
                }
                else
                {
                    RoadIO io = spawnedRoads[i + 1].GetRoadIOByDirection(RoadIO.GetOppositeDirection(direction))[0];
                    io.MoveRoadTo(io.ConnectedTo.transform.position);
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

    private bool SpawnRoads(in string[] ids, in IODirection direction, out Road[] spawnedRoads, out List<Road> extraSpawnedRoads, out Dictionary<RoadIO, RoadIO> oldConnections)
    {
        Vector3 pos = this.selectedIO != null ? this.selectedIO.transform.position : roadStartMarker.position;
        oldConnections = new Dictionary<RoadIO, RoadIO>();
        spawnedRoads = null;
        extraSpawnedRoads = new List<Road>();
        if (this.selectedIO != null && this.selectedIO == firstInput)
        {
            //AVISO DE QUE NO SE PUEDE PONER AHI
            Debug.LogWarning("No se puede poner aqui");
            return false;
        }

        if (!GenerateRoads(ids, direction, pos, out spawnedRoads))
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

            oldConnections.Add(roadIOL, roadIOL.ConnectedTo);
            roadIOL.ConnectedTo = spawnedRoads[0].GetRoadIOByDirection(RoadIO.GetOppositeDirection(direction))[0];

            if (roadIOR != null)
            {
                roadIOR.ConnectedTo = spawnedRoads[spawnedRoads.Length - 1].GetRoadIOByDirection(direction)[0];
            }

            roadIOL.ConnectedTo.MoveRoadTo(roadIOL.transform.position);
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
                    //MAX_ACCEPTABLE_DISTANCE
                    //Llenamos el hueco
                    List<RoadIO> currentRoadIO = new List<RoadIO>();
                    List<RoadIO> nextRoadIO = new List<RoadIO>();

                    foreach (RoadIO r in currentIO.GetParentRoad().GetRoadIOByDirection(currentIO.Direction))
                    {
                        if (Vector3.Distance(r.transform.position, r.ConnectedTo.transform.position) > MAX_ACCEPTABLE_DISTANCE)
                        {
                            oldConnections.Add(r, r.ConnectedTo);
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

                            if (roadFactory.FillGapWithConnector(nextRoadIO, currentRoadIO, out gap, out connectionsR1_Connector, out connectionsR2_Connector))
                            {
                                // processedRoads.Add(gap);
                                // gap.transform.parent = roadParent;

                                string[] idsGap = new string[numberOfPiecesGap];
                                for (int i = 0; i < idsGap.Length; i++)
                                {
                                    idsGap[i] = gap.RoadIdentifier;
                                }

                                Road[] spanedRoads;
                                if (GenerateRoads(idsGap, nextRoadIO[0].Direction, nextRoadIO[0].transform.position, out spanedRoads))
                                {
                                    ConnectRoads(nextRoadIO[0].GetParentRoad(), spanedRoads[0], connectionsR1_Connector);
                                    ConnectRoads(currentRoadIO[0].GetParentRoad(), spanedRoads[spanedRoads.Length - 1], connectionsR2_Connector);
                                    foreach (Road newR in spanedRoads)
                                    {
                                        processedRoads.Add(newR);
                                        extraSpawnedRoads.Add(newR);
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
        bool placed = false;

        if (this.selectedIO != null)
        {
            //Debug.LogError(this.selectedIO.IOIdentifier);
            VerticalButton addedButton;
            if (this.selectedIO.GetParentRoad() is NodeVerticalButton)
            {
                NodeVerticalButton node = (NodeVerticalButton)this.selectedIO.GetParentRoad();

                if (node.AddButton(button.ToString(), this.selectedIO, out addedButton))
                {
                    RoadChanges c = new RoadChanges();
                    c.addedButton = new Tuple<NodeVerticalButton, VerticalButton>(node, addedButton);
                    c.selectedIOBack = this.selectedIO;
                    undoStack.Push(c);
                    placed = true;
                }
            }
            if (!placed && this.selectedIO.ConnectedTo != null)
            {
                //Debug.LogError(this.selectedIO.connectedTo.IOIdentifier);
                if (this.selectedIO.ConnectedTo.GetParentRoad() is NodeVerticalButton)
                {
                    NodeVerticalButton node1 = (NodeVerticalButton)this.selectedIO.ConnectedTo.GetParentRoad();
                    if (node1.AddButton(button.ToString(), this.selectedIO.ConnectedTo, out addedButton))
                    {
                        RoadChanges c = new RoadChanges();
                        c.addedButton = new Tuple<NodeVerticalButton, VerticalButton>(node1, addedButton);
                        c.selectedIOBack = this.selectedIO;
                        undoStack.Push(c);
                        placed = true;
                    }
                }
            }
        }

        if (!placed)
        {
            RoadIO selectedIOBack = this.selectedIO;
            string[] ids = { "NodeVerticalButton" };
            Road[] spawnedRoad;
            List<Road> extraRoads;
            Dictionary<RoadIO, RoadIO> oldConnections;
            if (SpawnRoads(ids, IODirection.Forward, out spawnedRoad, out extraRoads, out oldConnections))
            {
                string[] args = { "activate", button.ToString() };
                spawnedRoad[0].ExecuteAction(args);
                NewActionOnStack(spawnedRoad, extraRoads, oldConnections, selectedIOBack);
            }
        }
    }

    private void NewActionOnStack(Road[] r, List<Road> rl, Dictionary<RoadIO, RoadIO> o, RoadIO selectedIOback)
    {
        RoadChanges rChanges = new RoadChanges();
        if (r != null)
        {
            foreach (Road rr in r)
            {
                rChanges.addedRoads.Add(rr);
            }
        }

        if (rl != null)
        {
            foreach (Road rrl in rl)
            {
                rChanges.addedRoads.Add(rrl);
            }
        }

        if (o != null)
        {
            foreach (KeyValuePair<RoadIO, RoadIO> entry in o)
            {
                rChanges.connectionsChanged.Add(entry.Key, entry.Value);
            }
        }

        rChanges.selectedIOBack = selectedIOback;

        undoStack.Push(rChanges);
    }

    private void DoMove()
    {
        if (EnoughNumberOfInstrucions(Buttons.Move))
        {
            SpawnVerticalButton(Buttons.Move);
        }
    }

    private Road GetLastRoad()
    {
        List<Road> allRoads = new List<Road>();
        Stack<Road> roadsToProccess = new Stack<Road>();
        roadsToProccess.Push(firstInput.GetParentRoad());
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
                    if (rIO is RoadOutput)
                    {
                        return rIO.GetParentRoad();
                    }
                }
            }

            if (!allRoads.Contains(r))
            {
                allRoads.Add(r);
            }
        }

        return null;
    }

    private void DoPlay()
    {
        if (selectedIO != null)
        {
            EventAggregator.Instance.Publish(new MsgDisableAllButtons());
            EventAggregator.Instance.Publish(new MsgEnableButton(Buttons.Restart));
            EventAggregator.Instance.Publish(new MsgEnableButton(Buttons.MapMenu));

            List<Road> allRoads = new List<Road>();
            Stack<Road> roadsToProccess = new Stack<Road>();

            RoadInput roadInput = null;
            RoadOutput roadOutput = null;

            roadsToProccess.Push(selectedIO.GetParentRoad());
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
                selectedOutputMarker.gameObject.SetActive(false);
                EventAggregator.Instance.Publish(new MsgStartRoadMovement(roadInput, roadOutput));
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

    public void DoRestart()
    {
        ResetRoad();
        GameLogic.Instance.AddInputFromButton(Buttons.Restart);
    }

    public void ResetRoad()
    {
        EventAggregator.Instance.Publish(new MsgEnableAllButtons());
        EventAggregator.Instance.Publish(new MsgStopMovement());

        while (undoStack.Count > 0)
        {
            RoadChanges c = undoStack.Pop();
            foreach (Road r in c.addedRoads)
            {
                Destroy(r.gameObject);
            }
        }

        selectedOutputMarker.transform.parent = roadParent;
        roadStart.transform.parent = roadParent;
        roadParent.position = roadStartMarker.position;

        firstInput = roadStart.GetRoadIOByDirection(IODirection.Back)[0];
        firstInput.MoveRoadTo(roadStartMarker.position);
        this.selectedIO = roadStart.GetRoadIOByDirection(IODirection.Forward)[0];

        selectedOutputMarker.transform.position = this.selectedIO.transform.position;
        selectedOutputMarker.gameObject.SetActive(true);
        minibot.transform.position = firstInput.transform.position;
        minibot.transform.rotation = new Quaternion();
        minibot.gameObject.SetActive(true);
    }

    private void DoTurnLeft()
    {
        if (EnoughNumberOfInstrucions(Buttons.TurnLeft))
        {
            SpawnVerticalButton(Buttons.TurnLeft);
        }
    }

    private void DoTurnRight()
    {
        if (EnoughNumberOfInstrucions(Buttons.TurnRight))
        {
            SpawnVerticalButton(Buttons.TurnRight);
        }
    }

    private void CorrectPositions(in float maxAcceptableDistance, RoadIO pivotIO)
    {
        if (pivotIO != null)
        {
            pivotIO.MoveRoadTo(roadStartMarker.transform.position);

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