using System;
using System.Collections.Generic;
using UnityEngine;
using static ButtonConstants;
using static ObjectConstants;

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

    private GameObject[] objectReferences;

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
        mainCharacterAnimator = mainCharacterGameObject.GetComponent<MainCharacterController>().GetAnimator();
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
              /*  List<int> intendedBlock = BlockToAdvanceTo(currentLevelData.playerOrientation, currentLevelData.playerPos[0], currentLevelData.playerPos[1], currentLevelData.playerPos[2]);
                bool metConditions = false;
                ObjectType convertBlockTo=ObjectType.SolidBlock;
                ObjectType blockToSpawn = ObjectType.SolidBlock;
                switch (item.ObjectType)
                {
                    case ObjectType.PlankItem:
                        metConditions = GetBlockType(currentLevelData, intendedBlock[0], intendedBlock[1] - 1, intendedBlock[2]) == ObjectConstants.ObjectType.SpikesBlock;
                        convertBlockTo = ObjectType.SpikesBlockActivated;
                        blockToSpawn = ObjectType.SpikesBlock;
                        break;
                    case ObjectType.FanItem:
                        metConditions = GetBlockType(currentLevelData, intendedBlock[0], intendedBlock[1] - 1, intendedBlock[2]) == ObjectConstants.ObjectType.WaterBlock;
                        convertBlockTo = ObjectType.IceBlock;
                        blockToSpawn = ObjectType.IceBlock;
                        break;
                }
                if (metConditions)
                {
                    inventory.Pop();
                    SetBlockType(currentLevelData, convertBlockTo, intendedBlock[0], intendedBlock[1] - 1, intendedBlock[2]);
                    GetBlock(currentLevelData, objectReferences, intendedBlock[0], intendedBlock[1] - 1, intendedBlock[2]).SetActive(false);
                    LevelManager.instance.MapRenderer.RenderConcreteBlock(objectReferences, currentLevelData.levelSize, blockToSpawn, intendedBlock[0], intendedBlock[1] - 1, intendedBlock[2]);

                    item.transform.parent = LevelManager.instance.MapRenderer.transform;

                    item.transform.localScale = new Vector3(1, 1, 1);
                    Vector3 posNew;
                    posNew.x = intendedBlock[0] * 1;
                    posNew.y = intendedBlock[1] * 1;
                    posNew.z = intendedBlock[2] * 1;
                    item.transform.position = posNew;
                    item.GetComponent<Animator>().SetTrigger("Usar");
                    mainCharacterAnimator.SetTrigger("Usar");
                }*/

            }
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
        bool isTopEmpty = IsEmptyBlock(currentLevelData.playerPos[0], currentLevelData.playerPos[1] + 1, currentLevelData.playerPos[2], currentLevelData);

        if (WalkableBlock(intendedBlock[0], intendedBlock[1] - 1, intendedBlock[2], currentLevelData) && isTopEmpty && (IsEmptyBlock(intendedBlock[0], intendedBlock[1], intendedBlock[2], currentLevelData) || TakeItem(intendedBlock)))
        {
            levelManagerReference.ActionRenderer.DoJump(mainCharacterGameObject, currentLevelData.playerPos, intendedBlock, currentLevelData.playerOrientation);
            currentLevelData.playerPos = intendedBlock;
        }
        else
        {
            bool found = false;
            for (int y = isTopEmpty ? currentLevelData.playerPos[1] : currentLevelData.playerPos[1] - 1; y >= 0; y--)
            {
                if (IsEmptyBlock(intendedBlock[0], y, intendedBlock[2], currentLevelData) && WalkableBlock(intendedBlock[0], y - 1, intendedBlock[2], currentLevelData))
                {
                    found = true;
                    intendedBlock[1] = y;
                    fallDamage = currentLevelData.playerPos[1] - y;
                    levelManagerReference.ActionRenderer.DoJump(mainCharacterGameObject, currentLevelData.playerPos, intendedBlock, currentLevelData.playerOrientation);
                    currentLevelData.playerPos = intendedBlock;

                    break;
                }
            }
            if (!found)
            {
                intendedBlock[1] = -1;
                levelManagerReference.ActionRenderer.DoJump(mainCharacterGameObject, currentLevelData.playerPos, intendedBlock, currentLevelData.playerOrientation);
                YouLose();
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
            if (IsEmptyBlock(intendedBlock[0], intendedBlock[1], intendedBlock[2], currentLevelData))
            {
                //Miramos el bloque de debajo
                if (WalkableBlock(intendedBlock[0], intendedBlock[1] - 1, intendedBlock[2], currentLevelData))
                {
                    levelManagerReference.ActionRenderer.DoMove(mainCharacterGameObject, currentLevelData.playerPos, intendedBlock);
                    currentLevelData.playerPos = intendedBlock;
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
                    levelManagerReference.ActionRenderer.DoMove(mainCharacterGameObject, currentLevelData.playerPos, intendedBlock);
                    currentLevelData.playerPos = intendedBlock;
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
            SetBlockType(currentLevelData, ObjectType.NoBlock, intendedBlock[0], intendedBlock[1], intendedBlock[2]);
            Item thisItem = GetBlock(currentLevelData, objectReferences, intendedBlock[0], intendedBlock[1], intendedBlock[2]).GetComponent<Item>();

            if (thisItem != null)
            {
                inventory.Push(thisItem);
                // thisItem.SetActive(false);
                thisItem.transform.parent = mainCharacterGameObject.transform;

                thisItem.transform.localScale = new Vector3(0.9f, 0.9f, 0.9f);
                //thisItem.transform.position = mainCharacterGameObject.GetComponent<AnimatedObject>().InventoryMarker.transform.position;
                Vector3 invMarker = mainCharacterGameObject.GetComponent<MainCharacterController>().InventoryMarker.transform.position;
                thisItem.transform.position = new Vector3(invMarker.x, invMarker.y + 0.45f * (inventory.Count - 1), invMarker.z);

                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// The IsEmptyBlock
    /// </summary>
    /// <param name="x">The x<see cref="int"/></param>
    /// <param name="y">The y<see cref="int"/></param>
    /// <param name="z">The z<see cref="int"/></param>
    /// <param name="data">The data<see cref="CurrentLevelData"/></param>
    /// <returns>The <see cref="bool"/></returns>
    private bool IsEmptyBlock(int x, int y, int z, LevelData data)
    {
        ObjectType type = GetBlockType(data, x, y, z);
        if (type == ObjectType.NoBlock)
        {
            return true;
        }
        return false;
    }

    /// <summary>
    /// The WalkableBlock
    /// </summary>
    /// <param name="x">The x<see cref="int"/></param>
    /// <param name="y">The y<see cref="int"/></param>
    /// <param name="z">The z<see cref="int"/></param>
    /// <param name="data">The data<see cref="CurrentLevelData"/></param>
    /// <returns>The <see cref="bool"/></returns>
    private bool WalkableBlock(int x, int y, int z, LevelData data)
    {
        ObjectType type = GetBlockType(data, x, y, z);
        switch (type)
        {
            case ObjectType.SolidBlock:
                return true;

            case ObjectType.IceBlock:
                return true;

            case ObjectType.LiftBlockActivated:
                return true;

            case ObjectType.SpikesBlockActivated:
                return true;

            default:
                return false;
        }
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
    private ObjectType GetBlockType(LevelData data, int x, int y, int z)
    {
        if (x < 0 || x >= data.levelSize[0]) return ObjectType.NoBlock;
        if (y < 0 || y >= data.levelSize[1]) return ObjectType.NoBlock;
        if (z < 0 || z >= data.levelSize[2]) return ObjectType.NoBlock;
        return (ObjectType)data.mapAndItems[x + z * data.levelSize[0] + y * (data.levelSize[0] * data.levelSize[2])];
    }

    private GameObject GetBlock(LevelData data, GameObject[] objects, int x, int y, int z)
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
    private void SetBlockType(LevelData data, ObjectType value, int x, int y, int z)
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

        bool isTopEmpty = IsEmptyBlock(currentLevelData.playerPos[0], currentLevelData.playerPos[1] + 1, currentLevelData.playerPos[2], currentLevelData);

        if (WalkableBlock(intendedBlock[0], intendedBlock[1] - 1, intendedBlock[2], currentLevelData) && IsEmptyBlock(intendedBlock[0], intendedBlock[1], intendedBlock[2], currentLevelData)
            && isTopEmpty)
        {
            return true;
        }
        else
        {
            bool found = false;
            for (int y = isTopEmpty ? currentLevelData.playerPos[1] : currentLevelData.playerPos[1] - 1; y >= 0; y--)
            {
                if (IsEmptyBlock(intendedBlock[0], y, intendedBlock[2], currentLevelData) && WalkableBlock(intendedBlock[0], y - 1, intendedBlock[2], currentLevelData))
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