using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelLoader : MonoBehaviour
{

    public string levelPath = "Assets/StoryLevels/test.json";
    public GameObject LevelObjects;
    public GameObject LevelButtons;
    public float blockLength = 1f;
    public Vector3 startPos = new Vector3(0, 0, 0);
    private bool dirty = true;
    
    void Load()
    {
        LevelData data = LoadData(levelPath);
        ObjectFactory objectFactory = LevelObjects.GetComponent<ObjectFactory>();
        for (int x = 0; x < data.levelSize[0]; x++)
        {
            for (int y = 0; y < data.levelSize[1]; y++)
            {
                for (int z = 0; z < data.levelSize[2]; z++)
                {
                    int blockToSpawn = get(data, x, y, z);
                    Debug.Log(x + " " + y + " " + z + "-" + blockToSpawn);
                    GameObject block = objectFactory.GetGameObjectInstance(blockToSpawn);
                    Vector3 posNew = new Vector3(x * blockLength, y * blockLength, z * blockLength);
                    block.transform.position = posNew;
                }
            }
        }

        GameObject mainCharacter = objectFactory.GetMainCharacterInstance();
        Vector3 posNewChar = new Vector3(data.playerStart[0] * blockLength, data.playerStart[1]* blockLength, data.playerStart[2] * blockLength);
        mainCharacter.transform.Rotate(0, 90f * data.playerOrientation, 0);
        mainCharacter.transform.position = posNewChar;

        GameObject flag = objectFactory.GetFlagInstance();
        Vector3 posFlag = new Vector3(data.goal[0] * blockLength, data.goal[1] * blockLength, data.goal[2] * blockLength);
        flag.transform.position = posFlag;

        LevelButtons lButtonsScript = LevelButtons.GetComponent<LevelButtons>();
        lButtonsScript.setNumber(ButtonConstants.Action, data.availableInstructions.action);
        lButtonsScript.setNumber(ButtonConstants.Condition, data.availableInstructions.condition);
        lButtonsScript.setNumber(ButtonConstants.Jump, data.availableInstructions.jump);
        lButtonsScript.setNumber(ButtonConstants.Loop, data.availableInstructions.loop);
        lButtonsScript.setNumber(ButtonConstants.Move, data.availableInstructions.move);
        lButtonsScript.setNumber(ButtonConstants.TurnLeft, data.availableInstructions.turnLeft);
        lButtonsScript.setNumber(ButtonConstants.TurnRight, data.availableInstructions.turnRight);

    }

    public LevelData LoadData(string jsonPath)
    {
        LevelData levelData = new LevelData();
        string readedString = ReadString(jsonPath);
        JsonUtility.FromJsonOverwrite(readedString, levelData);
        return levelData;

    }
    public int get(LevelData data,int x, int y, int z)
    {
        if (x < 0 || x >= data.levelSize[0]) return ObjectConstants.NoBlock;
        if (y < 0 || y >= data.levelSize[1]) return ObjectConstants.NoBlock;
        if (z < 0 || z >= data.levelSize[2]) return ObjectConstants.NoBlock;
        return data.mapAndItems[x + z * data.levelSize[0] + y * (data.levelSize[0]* data.levelSize[2])];

    }


    // Update is called once per frame
    void Update()
    {
        if (dirty)
        {
            Load();
            dirty = false;
        }
    }
    string ReadString(string path)
    { 
        System.IO.StreamReader reader = new System.IO.StreamReader(levelPath);
        string output = reader.ReadToEnd();
        reader.Close();
        return output;
    }

}
