using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapRenderer : MonoBehaviour
{
    public float blockLength = 1f;

    private LevelManager manager;
    private GameObject mainCharacter;

    public GameObject MainCharacter { get => mainCharacter; }

    // Start is called before the first frame update
    void Awake()
    {
        manager = LevelManager.instance;
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void RenderMapAndItems(LevelData data)
    {
        for (int x = 0; x < data.levelSize[0]; x++)
        {
            for (int y = 0; y < data.levelSize[1]; y++)
            {
                for (int z = 0; z < data.levelSize[2]; z++)
                {
                    int blockToSpawn = get(data, x, y, z);
                  
                    GameObject block = manager.LevelObjects.GetGameObjectInstance(blockToSpawn);
                    Vector3 posNew = new Vector3(x * blockLength, y * blockLength, z * blockLength);
                    block.transform.position = posNew;
                    block.transform.parent = gameObject.transform;
                }
            }
        }
    }

    public void RenderScenery(LevelData data)
    {
        GameObject flag = manager.LevelObjects.GetFlagInstance();
        Vector3 posFlag = new Vector3(data.goal[0] * blockLength, data.goal[1] * blockLength, data.goal[2] * blockLength);
        flag.transform.position = posFlag;
        flag.transform.parent = gameObject.transform;
    }

    public GameObject RenderMainCharacter(LevelData data)
    {
        mainCharacter = manager.LevelObjects.GetMainCharacterInstance();
        Vector3 posNewChar = new Vector3(data.playerStart[0] * blockLength, data.playerStart[1] * blockLength, data.playerStart[2] * blockLength);
        mainCharacter.transform.Rotate(0, 90f * data.playerOrientation, 0);
        mainCharacter.transform.position = posNewChar;
        mainCharacter.transform.parent = gameObject.transform;
        return mainCharacter;
    }

    public GameObject GetMainCharacter()
    {
        return mainCharacter;
    }
    private int get(LevelData data, int x, int y, int z)
    {
        if (x < 0 || x >= data.levelSize[0]) return ObjectConstants.NoBlock;
        if (y < 0 || y >= data.levelSize[1]) return ObjectConstants.NoBlock;
        if (z < 0 || z >= data.levelSize[2]) return ObjectConstants.NoBlock;
        return data.mapAndItems[x + z * data.levelSize[0] + y * (data.levelSize[0] * data.levelSize[2])];

    }
}
