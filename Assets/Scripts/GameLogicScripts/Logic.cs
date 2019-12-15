using System.Collections.Generic;
using UnityEngine;

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

    /// <summary>
    /// Defines the buttonInputList
    /// </summary>
    private List<int> buttonInputBuffer;

    /// <summary>
    /// Defines the initialInputCapacity
    /// </summary>
    public int initialCapacityOfTheInputBuffer = 20;

    /// <summary>
    /// Gets the LevelData
    /// </summary>
    public LevelData CurrentLevelData { get => currentLevelData; }

    /// <summary>
    /// Defines the maxFallHeightOfCharactersInBlocks
    /// </summary>
    public int maxFallHeightOfCharactersInBlocks = 1;

    // Start is called before the first frame update
    /// <summary>
    /// The Start
    /// </summary>
    internal void Start()
    {

        
        buttonInputBuffer = new List<int>(initialCapacityOfTheInputBuffer);
        LoadLevelData();
        LoadVisual();
        Debug.Log(currentLevelData.levelName);
        
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
    }

    /// <summary>
    /// The LoadVisual
    /// </summary>
    private void LoadVisual()
    {
        levelManagerReference.MapRenderer.RenderMapAndItems(currentLevelData.mapAndItems, currentLevelData.levelSize);
        levelManagerReference.MapRenderer.RenderScenery(currentLevelData.goal);
        mainCharacterGameObject = levelManagerReference.MapRenderer.RenderMainCharacter(currentLevelData.playerPos, currentLevelData.playerOrientation);
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
            int buttonPressed = buttonInputBuffer[0];
            buttonInputBuffer.RemoveAt(0);
            if (buttonPressed < 0 || buttonPressed >= ButtonConstants.ButtonNames.Length)
            {
                Debug.Log("Invalid button");
            }
            else
            {

                Debug.Log("Pressed " + ButtonConstants.ButtonNames[buttonPressed] + " button.");
                
                switch (buttonPressed)
                {
                    case ButtonConstants.Action:

                        DoAction();
                        break;
                    case ButtonConstants.Condition:

                        DoCondition();
                        break;
                    case ButtonConstants.Jump:

                        DoJump();
                        break;
                    case ButtonConstants.Loop:

                        DoLoop();
                        break;
                    case ButtonConstants.Move:

                        DoMove();
                        break;
                    case ButtonConstants.Play:

                        DoPlay();
                        break;
                    case ButtonConstants.Restart:

                        DoRestart();
                        break;
                    case ButtonConstants.TurnLeft:

                        DoTurnLeft();
                        break;
                    case ButtonConstants.TurnRight:

                        DoTurnRight();
                        break;

                }


            }
        }
    }

    /// <summary>
    /// The ButtonInput
    /// </summary>
    /// <param name="buttonIndex">The buttonIndex<see cref="int"/></param>
    public void AddInputFromButton(int buttonIndex)
    {
        buttonInputBuffer.Add(buttonIndex);
    }

    /// <summary>
    /// The DoAction
    /// </summary>
    private void DoAction()
    {
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

        if (WalkableBlock(intendedBlock[0], intendedBlock[1] - 1, intendedBlock[2], currentLevelData) && IsEmptyBlock(intendedBlock[0], intendedBlock[1], intendedBlock[2], currentLevelData)
            && isTopEmpty)
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
                Debug.Log("You are colliding against a block");
 
            }

        }
        else
        {
            DoJump();
        }
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
        int blockIndex = GetBlockType(data, x, y, z);
        if (blockIndex == ObjectConstants.NoBlock)
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
        int blockIndex = GetBlockType(data, x, y, z);

        switch (blockIndex)
        {
            case ObjectConstants.SolidBlock:
                return true;

            case ObjectConstants.IceBlock:
                return true;


            case ObjectConstants.LiftBlockActivated:
                return true;

            case ObjectConstants.SpikesBlockActivated:
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
        if (x < 0 || x >= data.levelSize[0]) return ObjectConstants.NoBlock;
        if (y < 0 || y >= data.levelSize[1]) return ObjectConstants.NoBlock;
        if (z < 0 || z >= data.levelSize[2]) return ObjectConstants.NoBlock;
        return data.mapAndItems[x + z * data.levelSize[0] + y * (data.levelSize[0] * data.levelSize[2])];
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
        data.mapAndItems[x + z * data.levelSize[0] + y * (data.levelSize[0] * data.levelSize[2])] = value;
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
        return "Assets/StoryLevels/test.json";
    }
}
