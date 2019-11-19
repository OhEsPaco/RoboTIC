using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelLoader : MonoBehaviour
{

    public string levelPath = "Assets/StoryLevels/test.json";
    public GameObject LevelObjects;
    public float blockLength = 1f;
    public Vector3 startPos = new Vector3(0, 0, 0);
    // Start is called before the first frame update
    void Start()
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
        
    }
    string ReadString(string path)
    { 
        System.IO.StreamReader reader = new System.IO.StreamReader(levelPath);
        string output = reader.ReadToEnd();
        reader.Close();
        return output;
    }

}
