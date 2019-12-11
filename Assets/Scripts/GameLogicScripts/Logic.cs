using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Logic : MonoBehaviour
{
    private LevelManager manager;
    private LevelData levelData;
    private GameObject mainCharacter;
    
    public LevelData LevelData { get => levelData; }
    public int maxFallHeight=1;
    private bool isActionRenderingDone = true;

    // Start is called before the first frame update
    private void Start()
    {
      
        levelData = manager.JSonLoader.LoadData(GetLevelPath());


        Debug.Log(levelData.levelName);
        LoadVisual();
    }
    void Awake()
    {
        manager = LevelManager.instance;
      
       
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    #region graphics
    private void LoadVisual()
    {
        manager.MapRenderer.RenderMapAndItems(levelData.mapAndItems, levelData.levelSize);
            manager.MapRenderer.RenderScenery(levelData.goal);
        mainCharacter = manager.MapRenderer.RenderMainCharacter(levelData.playerPos,levelData.playerOrientation);

            SetAvailableInstructions(levelData);

        
    }

    private void SetAvailableInstructions(LevelData data)
    {
        LevelButtons lButtonsScript = manager.LevelButtons;
        lButtonsScript.setNumber(ButtonConstants.Action, data.availableInstructions.action);
        lButtonsScript.setNumber(ButtonConstants.Condition, data.availableInstructions.condition);
        lButtonsScript.setNumber(ButtonConstants.Jump, data.availableInstructions.jump);
        lButtonsScript.setNumber(ButtonConstants.Loop, data.availableInstructions.loop);
        lButtonsScript.setNumber(ButtonConstants.Move, data.availableInstructions.move);
        lButtonsScript.setNumber(ButtonConstants.TurnLeft, data.availableInstructions.turnLeft);
        lButtonsScript.setNumber(ButtonConstants.TurnRight, data.availableInstructions.turnRight);
    }
    #endregion

    #region buttons
    public void ButtonInput(int buttonIndex)
    {

        if(buttonIndex<0||buttonIndex>= ButtonConstants.ButtonNames.Length)
        {
            Debug.Log("Invalid button");
        }
        else
        {
            if (isActionRenderingDone)
            {
                Debug.Log("Pressed " + ButtonConstants.ButtonNames[buttonIndex] + " button.");
                isActionRenderingDone = false;
                switch (buttonIndex)
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
                     default:
                        isActionRenderingDone = true;
                        break;
                }
            }
            else
            {
                Debug.Log("You can't do that now. Something is moving.");
            }
          
        }
       

    }

    public void NotifyEndOfAction()
    {
        Debug.Log("End Coroutine");

        isActionRenderingDone = true;
    }

    private void DoAction()
    {

    }

    private void DoCondition()
    {

    }

    private void DoJump()
    {

        //blockC
        List<int> intendedBlock = BlockToMove(levelData.playerOrientation, levelData.playerPos[0], levelData.playerPos[1]+1, levelData.playerPos[2]);
        //Bloque de enfrente y arriba

        //blockA blockB 3
        //Robot  blockC 2
        //       blockD 1
        //       blockE 0  
        int fallDamage = 0;
        bool isTopEmpty=IsEmptyBlock(levelData.playerPos[0], levelData.playerPos[1]+1, levelData.playerPos[2], levelData);

        if (WalkableBlock(intendedBlock[0],intendedBlock[1]-1,intendedBlock[2],levelData)&&IsEmptyBlock(intendedBlock[0], intendedBlock[1], intendedBlock[2], levelData)
            && isTopEmpty)
        {
            manager.ActionRenderer.DoJump(mainCharacter, levelData.playerPos, intendedBlock, levelData.playerOrientation);
            levelData.playerPos = intendedBlock;
        }
        else
        {
            bool found = false;
            for(int y=isTopEmpty?levelData.playerPos[1]: levelData.playerPos[1] - 1; y >= 0; y--)
            {
                if(IsEmptyBlock(intendedBlock[0],y,intendedBlock[2],levelData)&&WalkableBlock(intendedBlock[0], y-1, intendedBlock[2], levelData))
                {
                    found = true;
                    intendedBlock[1] = y;
                    fallDamage = levelData.playerPos[1] - y;
                    manager.ActionRenderer.DoJump(mainCharacter, levelData.playerPos, intendedBlock, levelData.playerOrientation);
                    levelData.playerPos = intendedBlock;
                    
                    break;
                }
            }
            if (!found)
            {
                intendedBlock[1] = -1;
                manager.ActionRenderer.DoJump(mainCharacter, levelData.playerPos, intendedBlock, levelData.playerOrientation);
                YouLose();
            }
            
        }


        if (fallDamage > maxFallHeight)
        {
            YouLose();
        }


       
    }
    private void DoLoop()
    {

    }

    private void DoMove()
    {

        List<int> intendedBlock = BlockToMove(levelData.playerOrientation, levelData.playerPos[0], levelData.playerPos[1], levelData.playerPos[2]);

        if (IsInsideMap(intendedBlock, levelData))
        {
            if (IsEmptyBlock(intendedBlock[0], intendedBlock[1], intendedBlock[2], levelData))
            {
                //Miramos el bloque de debajo
                if (WalkableBlock(intendedBlock[0], intendedBlock[1]-1, intendedBlock[2], levelData))
                {
                    manager.ActionRenderer.DoMove(mainCharacter, levelData.playerPos, intendedBlock);
                    levelData.playerPos = intendedBlock;
  
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
                NotifyEndOfAction();

            }

        }
        else
        {
            DoJump();
        }

       
    }

    private bool IsEmptyBlock(int x, int y, int z, LevelData data)
    {
        int blockIndex = Get(data, x, y, z);
        if (blockIndex == ObjectConstants.NoBlock)
        {
            return true;
        }
        return false;
    }

    private bool WalkableBlock(int x, int y, int z,LevelData data)
    {
        int blockIndex = Get(data, x, y, z);

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

    private void YouLose()
    {
        Debug.Log("You lose :(");
       
    }
    private int Get(LevelData data, int x, int y, int z)
    {
        if (x < 0 || x >= data.levelSize[0]) return ObjectConstants.NoBlock;
        if (y < 0 || y >= data.levelSize[1]) return ObjectConstants.NoBlock;
        if (z < 0 || z >= data.levelSize[2]) return ObjectConstants.NoBlock;
        return data.mapAndItems[x + z * data.levelSize[0] + y * (data.levelSize[0] * data.levelSize[2])];

    }

    private void Set(LevelData data,int value, int x, int y, int z)
    {
        if (x < 0 || x >= data.levelSize[0]) return;
        if (y < 0 || y >= data.levelSize[1]) return;
        if (z < 0 || z >= data.levelSize[2]) return;
        data.mapAndItems[x + z * data.levelSize[0] + y * (data.levelSize[0] * data.levelSize[2])]=value;

    }
    private bool IsInsideMap(List<int>posToCheck,LevelData data)
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
    private List<int> BlockToMove(int playerOrientation, int x,int y,int z)
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
    private void DoPlay()
    {

    }
    private void DoRestart()
    {

    }

    private void DoTurnLeft()
    {

        levelData.playerOrientation--;
        if (levelData.playerOrientation < 0)
        {
            levelData.playerOrientation = 3;
        }
        manager.ActionRenderer.DoTurnLeft(mainCharacter);
    }
    private void DoTurnRight()
    {
        levelData.playerOrientation++;
        if (levelData.playerOrientation > 3)
        {
            levelData.playerOrientation = 0;
        }
        manager.ActionRenderer.DoTurnRight(mainCharacter);
    }
    #endregion

    private string GetLevelPath()
    {
        return "Assets/StoryLevels/test.json";
    }
}
