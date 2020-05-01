using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Block;
using static LevelButtons;
using static LevelObject;

/// <summary>
/// Defines the <see cref="Logic" />
/// </summary>
public class Logic : MonoBehaviour
{
    /// <summary>
    /// Defines the manager
    /// </summary>
    private LevelManager levelManagerReference;

    /// <summary>
    /// Defines the levelData
    /// </summary>
    private LevelData currentLevelData;

    /// <summary>
    /// Defines the mainCharacter
    /// </summary>
    private GameObject mainCharacterGameObject;

    private LevelObject[] objectReferences;

    /// <summary>
    /// Defines the buttonInputList
    /// </summary>
    private List<Buttons> buttonInputBuffer;

    /// <summary>
    /// Defines the initialInputCapacity
    /// </summary>
    public int initialCapacityOfTheInputBuffer = 20;

    /// <summary>
    /// Gets the LevelData
    /// </summary>
    public LevelData CurrentLevelData { get => currentLevelData; }

    private Stack<Item> inventory = new Stack<Item>();

    /// <summary>
    /// Defines the maxFallHeightOfCharactersInBlocks
    /// </summary>
    public int maxFallHeightOfCharactersInBlocks = 1;

    private Dictionary<Buttons, Action> buttonActionsDictionary;

    private Animator mainCharacterAnimator;

    private bool haltExecution = false;

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

        buttonInputBuffer = new List<Buttons>(initialCapacityOfTheInputBuffer);
        LoadLevelData();
        LoadVisual();
        Debug.Log(currentLevelData.levelName);
    }

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

    private bool CheckWinState()
    {
        if (currentLevelData.goal[0] == currentLevelData.playerPos[0] + 1 && currentLevelData.goal[1] == currentLevelData.playerPos[1] + 1 && currentLevelData.goal[2] == currentLevelData.playerPos[2])
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private void YouWin()
    {
        mainCharacterAnimator.SetTrigger("HacerSaludo");
        haltExecution = true;
        Debug.Log("A winner is you");
    }

    /// <summary>
    /// The Awake
    /// </summary>
    internal void Awake()
    {
        levelManagerReference = LevelManager.instance;
    }

    // Update is called once per frame
    /// <summary>
    /// The Update
    /// </summary>
    internal void Update()
    {
        ExecuteNextAvailableInput();

        if (!haltExecution)
        {
            if (CheckWinState())
            {
                YouWin();
            }
        }
    }

    /// <summary>
    /// The LoadVisual
    /// </summary>
    private void LoadVisual()
    {
        objectReferences = levelManagerReference.MapRenderer.RenderMapAndItems(currentLevelData.mapAndItems, currentLevelData.levelSize);
        levelManagerReference.MapRenderer.RenderScenery(currentLevelData.goal);
        mainCharacterGameObject = levelManagerReference.MapRenderer.RenderMainCharacter(currentLevelData.playerPos, currentLevelData.playerOrientation);
        Vector3 playerPos;
        if (GetBlockSurfacePoint(currentLevelData.playerPos[0], currentLevelData.playerPos[1] - 1, currentLevelData.playerPos[2], out playerPos))
        {
            mainCharacterGameObject.transform.position = playerPos;
        }
        // mainCharacterAnimator = mainCharacterGameObject.GetComponent<MainCharacterController>().GetAnimator();
        levelManagerReference.LevelButtons.SetNumberOfAvailableInstructions(currentLevelData);
    }

    private void LoadLevelData()
    {
        currentLevelData = levelManagerReference.JSonLoader.LoadLevelData(GetLevelPath());
    }

    /// <summary>
    /// The ExecuteNextInput
    /// </summary>
    private void ExecuteNextAvailableInput()
    {
        if (buttonInputBuffer.Count > 0)
        {
            Buttons buttonPressed = buttonInputBuffer[0];
            buttonInputBuffer.RemoveAt(0);
            buttonActionsDictionary[buttonPressed]();
        }
    }

    /// <summary>
    /// The ButtonInput
    /// </summary>
    /// <param name="buttonIndex">The buttonIndex<see cref="int"/></param>
    public void AddInputFromButton(Buttons buttonIndex)
    {
        if (!haltExecution)
        {
            buttonInputBuffer.Add(buttonIndex);
        }
    }

    //Ejecuta un efecto sobre un bloque
    private bool ExecuteActionEffect(LevelObject blockLevelObject, Item item, int x, int y, int z)
    {
        if (blockLevelObject != null && blockLevelObject.IsBlock())
        {
            Block frontBlock = (Block)blockLevelObject;

            EffectReaction[] effectReactions = frontBlock.EffectReactions;
            Effects itemEffect = item.Effect;

            if (itemEffect != Effects.None)
            {
                foreach (EffectReaction reaction in effectReactions)
                {
                    //Comprobar si esta reaccion se activa con el efecto del item
                    if (reaction.effect == itemEffect)
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
                            foreach (BlockActions blockAction in reaction.actionsToExecute)
                            {
                                frontBlock.ExecuteAction(blockAction);
                            }

                            if (reaction.newProperties.Length != 0)
                            {
                                frontBlock._BlockProperties = reaction.newProperties;
                            }

                            foreach (string trigger in reaction.animationTriggers)
                            {
                                frontBlock.SetAnimationTrigger(trigger);
                            }

                            if (reaction.replaceBlock)
                            {
                                //Aqui lanzar corrutina que espere a que todo lo anterior se haya acabado
                                //y acto seguido replace el bloque por uno nuevo y ejecute la accion place
                                StartCoroutine(ReplaceBlock(frontBlock, reaction.block, x, y, z));
                            }
                            //Se ha podido usar el item
                            return true;
                        }
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
                Debug.Log("About to use " + item.ToString());
                List<int> intendedBlock = BlockToAdvanceTo(currentLevelData.playerOrientation, currentLevelData.playerPos[0], currentLevelData.playerPos[1], currentLevelData.playerPos[2]);
                LevelObject blockLevelObject = GetBlock(currentLevelData, objectReferences, intendedBlock[0], intendedBlock[1], intendedBlock[2]);

                //Intentamos aplicar el item al bloque de enfrente
                if (ExecuteActionEffect(blockLevelObject, item, intendedBlock[0], intendedBlock[1], intendedBlock[2]))
                {
                    item.transform.parent = LevelManager.instance.MapRenderer.transform;

                    item.transform.localScale = new Vector3(1, 1, 1);
                    Vector3 posNew;
                    posNew.x = intendedBlock[0] * 1;
                    posNew.y = intendedBlock[1] * 1;
                    posNew.z = intendedBlock[2] * 1;
                    item.transform.position = posNew;
                    item.Use();

                    //Cambiar esto por llamadas al objeto BigCharacter
                    mainCharacterAnimator.SetTrigger("Usar");

                    inventory.Pop();
                }
                else
                {
                    //Intentamos aplicar el item al bloque de abajo
                    if (blockLevelObject != null && blockLevelObject.IsBlock())
                    {
                        Block frontBlock = (Block)blockLevelObject;

                        if (frontBlock.CheckProperty(BlockProperties.Immaterial))
                        {
                            blockLevelObject = GetBlock(currentLevelData, objectReferences, intendedBlock[0], intendedBlock[1] - 1, intendedBlock[2]);
                            if (ExecuteActionEffect(blockLevelObject, item, intendedBlock[0], intendedBlock[1] - 1, intendedBlock[2]))
                            {
                                item.transform.parent = LevelManager.instance.MapRenderer.transform;

                                item.transform.localScale = new Vector3(1, 1, 1);
                                Vector3 posNew;
                                posNew.x = intendedBlock[0] * 1;
                                posNew.y = intendedBlock[1] * 1;
                                posNew.z = intendedBlock[2] * 1;
                                item.transform.position = posNew;
                                item.Use();

                                //Cambiar esto por llamadas al objeto BigCharacter
                                mainCharacterAnimator.SetTrigger("Usar");

                                inventory.Pop();
                            }
                        }
                    }
                }
            }
        }
    }

    private IEnumerator ReplaceBlock(Block block, Blocks newBlock, int x, int y, int z)
    {
        bool allDone = false;
        while (!allDone)
        {
            if (block.ActionsDone())
            {
                block.gameObject.SetActive(false);
                Destroy(block.gameObject);
                LevelObject newlySpawnedObject = LevelManager.instance.MapRenderer.RenderBlock(objectReferences, currentLevelData.levelSize, (int)newBlock, x, y, z);
                if (newlySpawnedObject != null && newlySpawnedObject.IsBlock())
                {
                    Block newlySpawnedBlock = (Block)newlySpawnedObject;
                    newlySpawnedBlock.ExecuteAction(BlockActions.Place);
                }

                allDone = true;
            }
            yield return null;
        }
    }

    /// <summary>
    /// The DoCondition
    /// </summary>
    private void DoCondition()
    {
    }

    /// <summary>
    /// The DoJump
    /// </summary>
    private void DoJump()
    {
        //blockC
        List<int> intendedBlock = BlockToAdvanceTo(currentLevelData.playerOrientation, currentLevelData.playerPos[0], currentLevelData.playerPos[1] + 1, currentLevelData.playerPos[2]);

        //Bloque de enfrente y arriba

        //blockA blockB 3
        //Robot  blockC 2
        //       blockD 1
        //       blockE 0
        int fallDamage = 0;
        bool isTopEmpty = CheckBlockProperty(currentLevelData.playerPos[0], currentLevelData.playerPos[1] + 1, currentLevelData.playerPos[2], currentLevelData, BlockProperties.Immaterial);

        if (CheckBlockProperty(intendedBlock[0], intendedBlock[1] - 1, intendedBlock[2], currentLevelData, BlockProperties.Walkable) && isTopEmpty && (CheckBlockProperty(intendedBlock[0], intendedBlock[1], intendedBlock[2], currentLevelData, BlockProperties.Immaterial) || TakeItem(intendedBlock)))
        {
            Vector3 target;
            if (GetBlockSurfacePoint(intendedBlock[0], intendedBlock[1] - 1, intendedBlock[2], out target))
            {
                levelManagerReference.ActionRenderer.DoJump(mainCharacterGameObject, target, currentLevelData.playerOrientation);
                currentLevelData.playerPos = intendedBlock;
            }
        }
        else
        {
            for (int y = isTopEmpty ? currentLevelData.playerPos[1] : currentLevelData.playerPos[1] - 1; y >= 0; y--)
            {
                if (CheckBlockProperty(intendedBlock[0], y, intendedBlock[2], currentLevelData, BlockProperties.Immaterial) && CheckBlockProperty(intendedBlock[0], y - 1, intendedBlock[2], currentLevelData, BlockProperties.Walkable))
                {
                    intendedBlock[1] = y;
                    fallDamage = currentLevelData.playerPos[1] - y;
                    Vector3 target;
                    if (GetBlockSurfacePoint(intendedBlock[0], intendedBlock[1] - 1, intendedBlock[2], out target))
                    {
                        levelManagerReference.ActionRenderer.DoJump(mainCharacterGameObject, target, currentLevelData.playerOrientation);
                        currentLevelData.playerPos = intendedBlock;
                    }

                    break;
                }
            }
        }

        if (fallDamage > maxFallHeightOfCharactersInBlocks)
        {
            YouLose();
        }
    }

    /// <summary>
    /// The DoLoop
    /// </summary>
    private void DoLoop()
    {
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
                if (CheckBlockProperty(intendedBlock[0], intendedBlock[1] - 1, intendedBlock[2], currentLevelData, BlockProperties.Walkable))
                {
                    Vector3 target;
                    if (GetBlockSurfacePoint(intendedBlock[0], intendedBlock[1] - 1, intendedBlock[2], out target))
                    {
                        Debug.Log(target);
                        levelManagerReference.ActionRenderer.DoMove(mainCharacterGameObject, target);
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
                //Collision
                if (TakeItem(intendedBlock))
                {
                    Vector3 target;
                    if (GetBlockSurfacePoint(intendedBlock[0], intendedBlock[1] - 1, intendedBlock[2], out target))
                    {
                        levelManagerReference.ActionRenderer.DoMove(mainCharacterGameObject, target);
                        currentLevelData.playerPos = intendedBlock;
                    }
                }
                else
                {
                    Debug.Log("You are colliding against a block");
                }
            }
        }
        else
        {
            DoJump();
        }
    }

    private bool TakeItem(List<int> intendedBlock)
    {
        if ((int)GetBlockType(currentLevelData, intendedBlock[0], intendedBlock[1], intendedBlock[2]) >= 25)
        {
            SetBlockType(currentLevelData, (int)Blocks.NoBlock, intendedBlock[0], intendedBlock[1], intendedBlock[2]);
            LevelObject thisItem = GetBlock(currentLevelData, objectReferences, intendedBlock[0], intendedBlock[1], intendedBlock[2]).GetComponent<Item>();

            if (thisItem != null && thisItem.IsItem())
            {
                inventory.Push((Item)thisItem);
                // thisItem.SetActive(false);
                thisItem.transform.parent = mainCharacterGameObject.transform;

                thisItem.transform.localScale = new Vector3(0.9f, 0.9f, 0.9f);

                //thisItem.transform.position = mainCharacterGameObject.GetComponent<AnimatedObject>().InventoryMarker.transform.position;
                Vector3 invMarker = mainCharacterGameObject.GetComponent<BigCharacter>().GetInventoryPosition();
                thisItem.transform.position = new Vector3(invMarker.x, invMarker.y + 0.45f * (inventory.Count - 1), invMarker.z);

                return true;
            }
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
    /// The YouLose
    /// </summary>
    private void YouLose()
    {
        haltExecution = true;
        Debug.Log("You lose :(");
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

    /// <summary>
    /// The BlockToMove
    /// </summary>
    /// <param name="playerOrientation">The playerOrientation<see cref="int"/></param>
    /// <param name="x">The x<see cref="int"/></param>
    /// <param name="y">The y<see cref="int"/></param>
    /// <param name="z">The z<see cref="int"/></param>
    /// <returns>The <see cref="List{int}"/></returns>

    public bool IsWalkable()
    {
        List<int> intendedBlock = BlockToAdvanceTo(currentLevelData.playerOrientation, currentLevelData.playerPos[0], currentLevelData.playerPos[1] + 1, currentLevelData.playerPos[2]);
        //Bloque de enfrente y arriba

        bool isTopEmpty = CheckBlockProperty(currentLevelData.playerPos[0], currentLevelData.playerPos[1] + 1, currentLevelData.playerPos[2], currentLevelData, BlockProperties.Immaterial);

        if (CheckBlockProperty(intendedBlock[0], intendedBlock[1] - 1, intendedBlock[2], currentLevelData, BlockProperties.Walkable) && CheckBlockProperty(intendedBlock[0], intendedBlock[1], intendedBlock[2], currentLevelData, BlockProperties.Immaterial)
            && isTopEmpty)
        {
            return true;
        }
        else
        {
            bool found = false;
            for (int y = isTopEmpty ? currentLevelData.playerPos[1] : currentLevelData.playerPos[1] - 1; y >= 0; y--)
            {
                if (CheckBlockProperty(intendedBlock[0], y, intendedBlock[2], currentLevelData, BlockProperties.Immaterial) && CheckBlockProperty(intendedBlock[0], y - 1, intendedBlock[2], currentLevelData, BlockProperties.Walkable))
                {
                    return true;
                }
            }
            if (!found)
            {
                return false;
            }
        }

        return false;
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
        levelManagerReference.ActionRenderer.DoTurnLeft(mainCharacterGameObject);
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
        levelManagerReference.ActionRenderer.DoTurnRight(mainCharacterGameObject);
    }

    /// <summary>
    /// The GetLevelPath
    /// </summary>
    /// <returns>The <see cref="string"/></returns>
    private string GetLevelPath()
    {
        return "Assets/StoryLevels/evalLevel.json";
    }
}