using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Block;
using static LevelButtons;
using static LevelObject;
using static MessageScreenButton;
using static MessageScreenManager;

/// <summary>
/// Defines the <see cref="GameLogic" />
/// </summary>
public class GameLogic : MonoBehaviour
{
    [SerializeField] private GameObject placeableMap;

    /// <summary>
    /// Defines the levelData
    /// </summary>
    private LevelData currentLevelData;

    private LevelObject[] objectReferences;

    /// <summary>
    /// Gets the LevelData
    /// </summary>
    public LevelData CurrentLevelData { get => currentLevelData; }

    private Item[] items;
    private LevelData clonedLevelData;
    private Stack<Item> inventory = new Stack<Item>();

    private Dictionary<Buttons, Action> buttonActionsDictionary;

    //private Animator mainCharacterAnimator;

    private bool haltExecution = false;

    private bool loading = true;

    private MessageWarehouse msgWar;
    private GameObject bigCharater;
    private bool finishedMinibotMovement = false;

    // Start is called before the first frame update
    /// <summary>
    /// The Start
    /// </summary>
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
        buttonActionsDictionary.Add(Buttons.MapMenu, GoToMainMenu);
    }

    private static GameLogic gameLogic;

    public static GameLogic Instance
    {
        get
        {
            if (!gameLogic)
            {
                gameLogic = FindObjectOfType(typeof(GameLogic)) as GameLogic;

                if (!gameLogic)
                {
                    Debug.LogError("There needs to be one active GameLogic script on a GameObject in your scene.");
                }
            }

            return gameLogic;
        }
    }

    public bool FinishedMinibotMovement { get => finishedMinibotMovement; set => finishedMinibotMovement = value; }

    public void StartLevel(LevelData levelData, GameObject mapParent)
    {
        this.loading = true;
        this.haltExecution = true;
        finishedMinibotMovement = false;
        placeableMap.GetComponent<MapController>().EnableGameControls();

        inventory = new Stack<Item>();
        haltExecution = false;

        clonedLevelData = levelData.Clone();
        currentLevelData = levelData.Clone();
        items = null;

        this.mapParent = mapParent;
        StartCoroutine(RenderALevel());
    }

    private GameObject mapParent;

    private bool GetBlockSurfacePoint(in int x, in int y, in int z, out Vector3 surfacePoint)
    {
        LevelObject rawObj = GetBlock(CurrentLevelData, objectReferences, x, y, z);

        if (rawObj != null)
        {
            if (rawObj is Block)
            {
                Block b = (Block)rawObj;
                surfacePoint = b.SurfacePoint;
                return true;
            }
        }

        surfacePoint = new Vector3();
        return false;
    }

    private void CheckState()
    {
        if (!haltExecution)
        {
            if (CheckWinState())
            {
                haltExecution = true;
                StartCoroutine(YouWin());
            }
            else if (CheckLoseState())
            {
                haltExecution = true;
                StartCoroutine(YouLose());
            }
        }
    }

    private bool CheckWinState()
    {
        if (currentLevelData.goal[0] == currentLevelData.playerPos[0] && currentLevelData.goal[1] == currentLevelData.playerPos[1] && currentLevelData.goal[2] == currentLevelData.playerPos[2])
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private bool CheckLoseState()
    {
        if (CheckBlockProperty(currentLevelData.playerPos[0], currentLevelData.playerPos[1] - 1, currentLevelData.playerPos[2], currentLevelData, BlockProperties.Dangerous)
            || !IsPositionInsideMap(currentLevelData.playerPos, currentLevelData) || (finishedMinibotMovement && !CheckWinState()))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private IEnumerator YouWin()
    {
        // mainCharacterAnimator.SetTrigger("HacerSaludo");
        haltExecution = true;
        Debug.Log("A winner is you");
        MsgBigCharacterAllActionsFinished msg = new MsgBigCharacterAllActionsFinished();
        msgWar.PublishMsgAndWaitForResponse<MsgBigCharacterAllActionsFinished, bool>(msg);
        bool outTmp;
        yield return new WaitUntil(() => msgWar.IsResponseReceived<MsgBigCharacterAllActionsFinished, bool>(msg, out outTmp));
        EventAggregator.Instance.Publish(new MsgBigRobotAction(MsgBigRobotAction.BigRobotActions.Win, new Vector3()));
        EventAggregator.Instance.Publish<MsgShowScreen>(new MsgShowScreen("win", new Tuple<string, OnMessageScreenButtonPressed>[] {
            Tuple.Create<string, OnMessageScreenButtonPressed>("Yes", YesButton),
            Tuple.Create<string, OnMessageScreenButtonPressed>("No", NoButton)}));
    }

    private void YesButton()
    {
        RoadPlacementLogic.Instance.ResetRoad();
        DoRestart();
    }

    private void NoButton()
    {
        RoadPlacementLogic.Instance.ResetRoad();
        GoToMainMenu();
    }

    private IEnumerator YouLose()
    {
        // mainCharacterAnimator.SetTrigger("HacerSaludo");
        haltExecution = true;
        Debug.Log("A loser is you");
        MsgBigCharacterAllActionsFinished msg = new MsgBigCharacterAllActionsFinished();
        msgWar.PublishMsgAndWaitForResponse<MsgBigCharacterAllActionsFinished, bool>(msg);
        bool outTmp;
        yield return new WaitUntil(() => msgWar.IsResponseReceived<MsgBigCharacterAllActionsFinished, bool>(msg, out outTmp));
        EventAggregator.Instance.Publish(new MsgBigRobotAction(MsgBigRobotAction.BigRobotActions.Lose, new Vector3()));
        EventAggregator.Instance.Publish<MsgShowScreen>(new MsgShowScreen("lose", new Tuple<string, OnMessageScreenButtonPressed>[] {
            Tuple.Create<string, OnMessageScreenButtonPressed>("Yes", YesButton),
            Tuple.Create<string, OnMessageScreenButtonPressed>("No", NoButton)}));
    }

    /// <summary>
    /// The Awake
    /// </summary>
    internal void Awake()
    {
        msgWar = new MessageWarehouse(EventAggregator.Instance);
    }

    private IEnumerator RenderALevel()
    {
        this.loading = true;
        this.haltExecution = true;

        //Reseteamos el inventario
        inventory = new Stack<Item>();

        //Reseteamos el leveldata
        currentLevelData = clonedLevelData.Clone();

        //Pedimos que se renderice el nivel
        MsgRenderMapAndItems msg = new MsgRenderMapAndItems(currentLevelData.mapAndItems, currentLevelData.levelSize, currentLevelData.goal);
        LevelObject[] loadedLevel = null;
        msgWar.PublishMsgAndWaitForResponse<MsgRenderMapAndItems, LevelObject[]>(msg);
        yield return new WaitUntil(() => msgWar.IsResponseReceived<MsgRenderMapAndItems, LevelObject[]>(msg, out loadedLevel));

        if (loadedLevel != null)
        {
            //Ponemos el numero de instrucciones en los botones
            EventAggregator.Instance.Publish<MsgSetAvInstructions>(new MsgSetAvInstructions(currentLevelData.availableInstructions));
            MapContainer mcont = mapParent.GetComponent<MapContainer>();

            Vector3 lpos = placeableMap.transform.position;
            Quaternion lrot = placeableMap.transform.rotation;

            placeableMap.transform.position = new Vector3();
            placeableMap.transform.rotation = new Quaternion();

            mapParent.transform.position = new Vector3();
            mapParent.transform.rotation = new Quaternion();

            foreach (LevelObject obj in loadedLevel)
            {
                obj.gameObject.transform.parent = mapParent.transform;
            }

            mcont.UpdateMapCenter();
            mcont.MoveMapTo(placeableMap.GetComponent<MapController>().MapControllerCenter);
            objectReferences = loadedLevel;

            Vector3 playerPos;
            //Podria dar fallo si el personaje esta mal colocado
            GetBlockSurfacePoint(currentLevelData.playerPos[0], currentLevelData.playerPos[1] - 1, currentLevelData.playerPos[2], out playerPos);

            MsgPlaceCharacter msgLld = new MsgPlaceCharacter(playerPos, new Vector3(0, 90f * currentLevelData.playerOrientation, 0), mapParent.transform);
            msgWar.PublishMsgAndWaitForResponse<MsgPlaceCharacter, GameObject>(msgLld);
            //bigCharater = null;
            yield return new WaitUntil(() => msgWar.IsResponseReceived<MsgPlaceCharacter, GameObject>(msgLld, out bigCharater));
            bigCharater.SetActive(false);

            placeableMap.transform.position = lpos;
            placeableMap.transform.rotation = lrot;

            bool doReturn = true;
            for (int y = 0; y < currentLevelData.levelSize[1]; y++)
            {
                for (int x = 0; x < currentLevelData.levelSize[0]; x++)
                {
                    for (int z = 0; z < currentLevelData.levelSize[2]; z++)
                    {
                        doReturn = true;

                        if (currentLevelData.playerPos[0] == x && currentLevelData.playerPos[1] - 1 == y && currentLevelData.playerPos[2] == z)
                        {
                            bigCharater.SetActive(true);
                        }

                        try
                        {
                            LevelObject lOb = GetBlock(currentLevelData, loadedLevel, x, y, z);
                            lOb.gameObject.SetActive(true);
                            if (lOb is Block)
                            {
                                Block lOb2 = (Block)lOb;
                                if (lOb2.BlockType == Blocks.NoBlock)
                                {
                                    doReturn = false;
                                }
                            }
                        }
                        catch
                        {
                            //Do nothing
                        }
                        if (doReturn)
                        {
                            yield return null;
                        }
                    }
                }
            }

            //Separar los items y en su lugar spawnear noblocks
            Vector3 itemPosition;
            Quaternion itemRotation;
            Transform itemParent;
            items = new Item[loadedLevel.Length];
            for (int i = 0; i < items.Length; i++)
            {
                items[i] = null;
            }
            for (int y = 0; y < currentLevelData.levelSize[1]; y++)
            {
                for (int x = 0; x < currentLevelData.levelSize[0]; x++)
                {
                    for (int z = 0; z < currentLevelData.levelSize[2]; z++)
                    {
                        if ((int)GetBlockType(currentLevelData, x, y, z) >= 25)
                        {
                            SetBlockType(currentLevelData, (int)Blocks.NoBlock, x, y, z);
                            Item thisItem = GetBlock(currentLevelData, loadedLevel, x, y, z).GetComponent<Item>();

                            if (thisItem != null && thisItem.IsItem())
                            {
                                items[x + z * currentLevelData.levelSize[0] + y * (currentLevelData.levelSize[0] * currentLevelData.levelSize[2])] = thisItem;
                                itemPosition = thisItem.transform.position;
                                itemRotation = thisItem.transform.rotation;
                                itemParent = thisItem.transform.parent;
                                LevelObject spawnedObject = MapRenderer.Instance.SpawnBlock((int)Blocks.NoBlock);
                                spawnedObject.gameObject.transform.parent = itemParent;
                                spawnedObject.gameObject.transform.position = itemPosition;
                                spawnedObject.gameObject.transform.rotation = itemRotation;
                                SetBlock(currentLevelData, loadedLevel, x, y, z, spawnedObject);
                            }
                        }
                    }
                }
            }

            objectReferences = loadedLevel;
            //Por si acaso hay un item en la casilla inicial o está el jugador en mala posicion
            CheckState();
            TakeItem();
        }

        haltExecution = false;
        this.loading = false;
    }

    /// <summary>
    /// The ButtonInput
    /// </summary>
    /// <param name="buttonIndex">The buttonIndex<see cref="int"/></param>
    public void AddInputFromButton(Buttons button)
    {
        if (button == Buttons.Restart)
        {
            haltExecution = false;
        }

        if (!haltExecution)
        {
            try
            {
                buttonActionsDictionary[button]();
            }
            catch
            {
                Debug.LogError("Unknown input: " + button.ToString());
            }
        }
    }

    private void GoToMainMenu()
    {
        EventAggregator.Instance.Publish<MsgHideAllScreens>(new MsgHideAllScreens());
        finishedMinibotMovement = false;
        bigCharater.SetActive(false);
        currentLevelData = null;
        clonedLevelData = null;
        if (objectReferences != null)
        {
            StartCoroutine(DestroyLevelObjectsOnBackground((LevelObject[])objectReferences.Clone(), (Item[])items.Clone()));
            objectReferences = null;
            items = null;
        }
        //MapMenuLogic.Instance.ShowMapMenu();
        MainMenuLogic.Instance.ShowMainMenu();
    }

    private IEnumerator DestroyLevelObjectsOnBackground(LevelObject[] levelObjects, Item[] items)
    {
        foreach (Item i in items)
        {
            if (i != null)
            {
                i.gameObject.SetActive(false);
            }
        }
        foreach (LevelObject l in levelObjects)
        {
            l.gameObject.SetActive(false);
        }

        yield return null;
        foreach (LevelObject l in levelObjects)
        {
            Destroy(l.gameObject);
            yield return null;
        }
        foreach (Item i in items)
        {
            if (i != null)
            {
                Destroy(i.gameObject);
                yield return null;
            }
        }
    }

    private void TakeItem()
    {
        Item item = items[currentLevelData.playerPos[0] + currentLevelData.playerPos[2] * currentLevelData.levelSize[0] + currentLevelData.playerPos[1] * (currentLevelData.levelSize[0] * currentLevelData.levelSize[2])];
        if (item != null)
        {
            if (!inventory.Contains(item))
            {
                Debug.Log("Item taken");
                inventory.Push(item);
                EventAggregator.Instance.Publish<MsgTakeItem>(new MsgTakeItem(item, inventory.Count));
            }
        }
    }

    //Ejecuta un efecto sobre un bloque
    private bool ExecuteActionEffect(LevelObject blockLevelObject, Item item, int x, int y, int z, Vector3 itemPos)
    {
        if (blockLevelObject != null && blockLevelObject.IsBlock())
        {
            Block frontBlock = (Block)blockLevelObject;

            EffectReaction[] effectReactions = frontBlock.EffectReactions;
            Effects itemEffect = item.Effect;

            if (itemEffect != Effects.None)
            {
                EffectReaction reaction = Array.Find<EffectReaction>(effectReactions, value => value.effect == itemEffect);

                if (reaction != null)
                {
                    //Miramos si el item es compatible con este objeto
                    bool compatible = false;
                    if (reaction.compatibleItems.Length <= 0)
                    {
                        compatible = true;
                    }
                    else
                    {
                        foreach (Items itemSearch in reaction.compatibleItems)
                        {
                            if (itemSearch == item.ItemType)
                            {
                                compatible = true;
                                break;
                            }
                        }
                    }

                    //Si es compatible ejecutamos las acciones necesarias
                    if (compatible)
                    {
                        if (reaction.newProperties.Length != 0)
                        {
                            frontBlock._BlockProperties = reaction.newProperties;
                        }
                        LevelObject spawnedObject = null;

                        if (reaction.replaceBlock)
                        {
                            spawnedObject = MapRenderer.Instance.SpawnBlock((int)reaction.block);
                            spawnedObject.gameObject.transform.parent = frontBlock.transform.parent;
                            spawnedObject.gameObject.transform.position = frontBlock.transform.position;
                            spawnedObject.gameObject.transform.rotation = frontBlock.transform.rotation;

                            spawnedObject.gameObject.SetActive(false);

                            objectReferences[x + z * currentLevelData.levelSize[0] + y * (currentLevelData.levelSize[0] * currentLevelData.levelSize[2])] = spawnedObject;
                        }
                        inventory.Pop();
                        EventAggregator.Instance.Publish(new MsgUseItem(frontBlock, reaction, spawnedObject, itemPos, item, inventory));

                        return true;
                    }
                }
            }
        }

        //No se ha podido usar el item
        return false;
    }

    /// <summary>
    /// The DoAction
    /// </summary>
    private void DoAction()
    {
        if (inventory.Count > 0)
        {
            Item item = inventory.Peek();
            if (item != null)
            {
                List<int> intendedBlock = BlockToAdvanceTo(currentLevelData.playerOrientation, currentLevelData.playerPos[0], currentLevelData.playerPos[1], currentLevelData.playerPos[2]);
                LevelObject frontBlock = GetBlock(currentLevelData, objectReferences, intendedBlock[0], intendedBlock[1], intendedBlock[2]);
                LevelObject frontBelowBlock = GetBlock(currentLevelData, objectReferences, intendedBlock[0], intendedBlock[1] - 1, intendedBlock[2]);
                bool usedItem = false;
                if (frontBlock != null && item.UseOnFrontBlock && frontBlock.IsBlock())
                {
                    Vector3 posNew;
                    GetBlockSurfacePoint(intendedBlock[0], intendedBlock[1], intendedBlock[2], out posNew);
                    if (ExecuteActionEffect(frontBlock, item, intendedBlock[0], intendedBlock[1], intendedBlock[2], posNew))
                    {
                        usedItem = true;
                    }
                }
                if (frontBlock == null || CheckBlockProperty(frontBlock, BlockProperties.Immaterial))
                {
                    if (frontBelowBlock != null && item.UseOnFrontBelowBlock && !usedItem && frontBelowBlock.IsBlock())
                    {
                        Vector3 posNew;
                        GetBlockSurfacePoint(intendedBlock[0], intendedBlock[1] - 1, intendedBlock[2], out posNew);
                        ExecuteActionEffect(frontBelowBlock, item, intendedBlock[0], intendedBlock[1] - 1, intendedBlock[2], posNew);
                    }
                }
            }
        }
        CheckState();
    }

    /// <summary>
    /// The DoCondition
    /// </summary>
    private void DoCondition()
    {
        CheckState();
    }

    private void DoJump()
    {
        for (int y = currentLevelData.playerPos[1]; y >= 0; y--)
        {
            List<int> intendedBlock = BlockToAdvanceTo(currentLevelData.playerOrientation, currentLevelData.playerPos[0], y, currentLevelData.playerPos[2]);
            if (IsPositionInsideMap(intendedBlock, currentLevelData))
            {
                if (!CheckBlockProperty(intendedBlock[0], intendedBlock[1], intendedBlock[2], currentLevelData, BlockProperties.Immaterial))
                {
                    Vector3 target;
                    if (GetBlockSurfacePoint(intendedBlock[0], intendedBlock[1], intendedBlock[2], out target))
                    {
                        EventAggregator.Instance.Publish(new MsgBigRobotAction(MsgBigRobotAction.BigRobotActions.Jump, target));

                        currentLevelData.playerPos[0] = intendedBlock[0];
                        //Al saltar queda por encima del bloque
                        currentLevelData.playerPos[1] = intendedBlock[1] + 1;
                        currentLevelData.playerPos[2] = intendedBlock[2];

                        break;
                    }
                }
            }
        }

        TakeItem();
        CheckState();
    }

    /// <summary>
    /// The DoLoop
    /// </summary>
    private void DoLoop()
    {
        CheckState();
    }

    /// <summary>
    /// The DoMove
    /// </summary>
    private void DoMove()
    {
        List<int> intendedBlock = BlockToAdvanceTo(currentLevelData.playerOrientation, currentLevelData.playerPos[0], currentLevelData.playerPos[1], currentLevelData.playerPos[2]);

        if (IsPositionInsideMap(intendedBlock, currentLevelData))
        {
            if (CheckBlockProperty(intendedBlock[0], intendedBlock[1], intendedBlock[2], currentLevelData, BlockProperties.Immaterial))
            {
                //Miramos el bloque de debajo
                if (!CheckBlockProperty(intendedBlock[0], intendedBlock[1] - 1, intendedBlock[2], currentLevelData, BlockProperties.Immaterial))
                {
                    Vector3 target;
                    if (GetBlockSurfacePoint(intendedBlock[0], intendedBlock[1] - 1, intendedBlock[2], out target))
                    {
                        Debug.Log(target);
                        EventAggregator.Instance.Publish(new MsgBigRobotAction(MsgBigRobotAction.BigRobotActions.Move, target));

                        currentLevelData.playerPos = intendedBlock;
                    }
                }
                else
                {
                    DoJump();
                }
            }
            else
            {
                Debug.Log("You are colliding against a block");
            }
        }
        else
        {
            DoJump();
        }
        TakeItem();
        CheckState();
    }

    public bool CheckNextBlockDownProperty(BlockProperties property)
    {
        //-1 a la y para mirar el que va a pisar el robot
        List<int> nextBlock = BlockToAdvanceTo(currentLevelData.playerOrientation, currentLevelData.playerPos[0], currentLevelData.playerPos[1], currentLevelData.playerPos[2]);

        return CheckBlockProperty(nextBlock[0], nextBlock[1] - 1, nextBlock[2], currentLevelData, property);
    }

    private bool CheckBlockProperty(LevelObject blockLevelObject, BlockProperties property)
    {
        if (blockLevelObject != null && blockLevelObject.IsBlock())
        {
            Block block = (Block)blockLevelObject;
            return block.CheckProperty(property);
        }
        return false;
    }

    private bool CheckBlockProperty(int x, int y, int z, LevelData data, BlockProperties property)
    {
        LevelObject blockLevelObject = GetBlock(data, objectReferences, x, y, z);
        if (blockLevelObject != null && blockLevelObject.IsBlock())
        {
            Block block = (Block)blockLevelObject;
            return block.CheckProperty(property);
        }
        return false;
    }

    /// <summary>
    /// The Get
    /// </summary>
    /// <param name="data">The data<see cref="CurrentLevelData"/></param>
    /// <param name="x">The x<see cref="int"/></param>
    /// <param name="y">The y<see cref="int"/></param>
    /// <param name="z">The z<see cref="int"/></param>
    /// <returns>The <see cref="int"/></returns>
    private int GetBlockType(LevelData data, int x, int y, int z)
    {
        if (x < 0 || x >= data.levelSize[0]) return (int)Blocks.NoBlock;
        if (y < 0 || y >= data.levelSize[1]) return (int)Blocks.NoBlock;
        if (z < 0 || z >= data.levelSize[2]) return (int)Blocks.NoBlock;
        return data.mapAndItems[x + z * data.levelSize[0] + y * (data.levelSize[0] * data.levelSize[2])];
    }

    private LevelObject GetBlock(LevelData data, LevelObject[] objects, int x, int y, int z)
    {
        if (x < 0 || x >= data.levelSize[0]) return null;
        if (y < 0 || y >= data.levelSize[1]) return null;
        if (z < 0 || z >= data.levelSize[2]) return null;
        return objects[x + z * data.levelSize[0] + y * (data.levelSize[0] * data.levelSize[2])];
    }

    private void SetBlock(LevelData data, LevelObject[] objects, int x, int y, int z, LevelObject levelObject)
    {
        if (x < 0 || x >= data.levelSize[0]) return;
        if (y < 0 || y >= data.levelSize[1]) return;
        if (z < 0 || z >= data.levelSize[2]) return;
        objects[x + z * data.levelSize[0] + y * (data.levelSize[0] * data.levelSize[2])] = levelObject;
    }

    /// <summary>
    /// The Set
    /// </summary>
    /// <param name="data">The data<see cref="CurrentLevelData"/></param>
    /// <param name="value">The value<see cref="int"/></param>
    /// <param name="x">The x<see cref="int"/></param>
    /// <param name="y">The y<see cref="int"/></param>
    /// <param name="z">The z<see cref="int"/></param>
    private void SetBlockType(LevelData data, int value, int x, int y, int z)
    {
        if (x < 0 || x >= data.levelSize[0]) return;
        if (y < 0 || y >= data.levelSize[1]) return;
        if (z < 0 || z >= data.levelSize[2]) return;
        data.mapAndItems[x + z * data.levelSize[0] + y * (data.levelSize[0] * data.levelSize[2])] = (int)value;
    }

    /// <summary>
    /// The IsInsideMap
    /// </summary>
    /// <param name="posToCheck">The posToCheck<see cref="List{int}"/></param>
    /// <param name="data">The data<see cref="CurrentLevelData"/></param>
    /// <returns>The <see cref="bool"/></returns>
    private bool IsPositionInsideMap(List<int> posToCheck, LevelData data)
    {
        if (posToCheck[0] < 0)
        {
            return false;
        }

        if (posToCheck[1] < 0)
        {
            return false;
        }

        if (posToCheck[2] < 0)
        {
            return false;
        }

        if (posToCheck[0] >= data.levelSize[0])
        {
            return false;
        }

        if (posToCheck[1] >= data.levelSize[1])
        {
            return false;
        }

        if (posToCheck[2] >= data.levelSize[2])
        {
            return false;
        }

        return true;
    }

    private List<int> BlockToAdvanceTo(int playerOrientation, int x, int y, int z)
    {
        List<int> output = new List<int>();
        output.Add(x);
        output.Add(y);
        output.Add(z);

        //0 - z+
        //1 - x+
        //2 - z-
        //3 - x-
        //90f * data.playerOrientation
        switch (playerOrientation)
        {
            case 0:
                output[2]++;

                break;

            case 1:
                output[0]++;

                break;

            case 2:
                output[2]--;

                break;

            case 3:
                output[0]--;

                break;

            default:
                Debug.LogError("Unknown orientation");
                break;
        }

        return output;
    }

    /// <summary>
    /// The DoPlay
    /// </summary>
    private void DoPlay()
    {
    }

    /// <summary>
    /// The DoRestart
    /// </summary>
    private void DoRestart()
    {
        EventAggregator.Instance.Publish<MsgHideAllScreens>(new MsgHideAllScreens());
        currentLevelData = clonedLevelData.Clone();
        finishedMinibotMovement = false;
        bigCharater.SetActive(false);
        if (objectReferences != null)
        {
            StartCoroutine(DestroyLevelObjectsOnBackground((LevelObject[])objectReferences.Clone(), (Item[])items.Clone()));
            objectReferences = null;
            items = null;
        }

        StartCoroutine(RenderALevel());
    }

    /// <summary>
    /// The DoTurnLeft
    /// </summary>
    private void DoTurnLeft()
    {
        currentLevelData.playerOrientation--;
        if (currentLevelData.playerOrientation < 0)
        {
            currentLevelData.playerOrientation = 3;
        }
        EventAggregator.Instance.Publish(new MsgBigRobotAction(MsgBigRobotAction.BigRobotActions.TurnLeft, new Vector3(0, 0, 0)));
    }

    /// <summary>
    /// The DoTurnRight
    /// </summary>
    private void DoTurnRight()
    {
        currentLevelData.playerOrientation++;
        if (currentLevelData.playerOrientation > 3)
        {
            currentLevelData.playerOrientation = 0;
        }
        EventAggregator.Instance.Publish(new MsgBigRobotAction(MsgBigRobotAction.BigRobotActions.TurnRight, new Vector3(0, 0, 0)));
    }
}