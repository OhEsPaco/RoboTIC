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

    public void RenderMapAndItems(List<int> mapAndItems, List<int>levelSize)
    {
        for (int x = 0; x <levelSize[0]; x++)
        {
            for (int y = 0; y < levelSize[1]; y++)
            {
                for (int z = 0; z <levelSize[2]; z++)
                {
                    int blockToSpawn = Get(mapAndItems,levelSize, x, y, z);
                  
                    GameObject block = manager.LevelObjects.GetGameObjectInstance(blockToSpawn);
                    Vector3 posNew = new Vector3(x * blockLength, y * blockLength, z * blockLength);
                    block.transform.position = posNew;
                    block.transform.parent = gameObject.transform;
                }
            }
        }
    }

    public void RenderScenery(List<int>goal)
    {
        GameObject flag = manager.LevelObjects.GetFlagInstance();
        Vector3 posFlag = new Vector3(goal[0] * blockLength, goal[1] * blockLength, goal[2] * blockLength);
        flag.transform.position = posFlag;
        flag.transform.parent = gameObject.transform;
    }

    public GameObject RenderMainCharacter(List<int>playerStart,int playerOrientation)
    {
        mainCharacter = manager.LevelObjects.GetMainCharacterInstance();
        Vector3 posNewChar = new Vector3(playerStart[0] * blockLength,playerStart[1] * blockLength,playerStart[2] * blockLength);
        mainCharacter.transform.Rotate(0, 90f *playerOrientation, 0);
        mainCharacter.transform.position = posNewChar;
        mainCharacter.transform.parent = gameObject.transform;
        return mainCharacter;
    }

    public GameObject GetMainCharacter()
    {
        return mainCharacter;
    }
    private int Get(List<int> mapAndItems, List<int> levelSize, int x, int y, int z)
    {
        if (x < 0 || x >= levelSize[0]) return ObjectConstants.NoBlock;
        if (y < 0 || y >= levelSize[1]) return ObjectConstants.NoBlock;
        if (z < 0 || z >= levelSize[2]) return ObjectConstants.NoBlock;
        return mapAndItems[x + z * levelSize[0] + y * (levelSize[0] *levelSize[2])];

    }
}
