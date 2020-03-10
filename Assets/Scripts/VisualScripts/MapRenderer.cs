using System.Collections.Generic;
using UnityEngine;
using static ObjectConstants;

public class MapRenderer : MonoBehaviour
{
    [SerializeField] private float blockLength = 1f;

    public float BlockLength { get => blockLength; }

  

    public LevelObject[] RenderMapAndItems(in List<int> mapAndItems, in List<int> levelSize)
    {
        LevelObject[] objectReferences = new LevelObject[levelSize[0]*levelSize[1]*levelSize[2]];
        for (int x = 0; x < levelSize[0]; x++)
        {
            for (int y = 0; y < levelSize[1]; y++)
            {
                for (int z = 0; z < levelSize[2]; z++)
                {
                    ObjectType blockToSpawn = Get(mapAndItems, levelSize, x, y, z);

                    LevelObject block = LevelManager.instance.LevelObjects.GetGameObjectInstance(blockToSpawn);
                    objectReferences[x + z * levelSize[0] + y * (levelSize[0] * levelSize[2])] = block;
                    Vector3 posNew;
                    posNew.x = x * blockLength;
                    posNew.y = y * blockLength;
                    posNew.z = z * blockLength;
                    block.transform.position = posNew;
                    block.transform.parent = gameObject.transform;
                }
            }
        }
        return objectReferences;
    }

    public void RenderConcreteBlock(LevelObject[] objectReferences, List<int> levelSize, ObjectType blockToSpawn, int x, int y, int z)
    {

        LevelObject block = LevelManager.instance.LevelObjects.GetGameObjectInstance(blockToSpawn);
        objectReferences[x + z * levelSize[0] + y * (levelSize[0] * levelSize[2])] = block;
        Vector3 posNew;
        posNew.x = x * blockLength;
        posNew.y = y * blockLength;
        posNew.z = z * blockLength;
        block.transform.position = posNew;
        block.transform.parent = gameObject.transform;
    }
    public void RenderScenery(in List<int> goal)
    {
        LevelObject flag = LevelManager.instance.LevelObjects.GetFlagInstance();
        Vector3 posFlag;
        posFlag.x = goal[0] * blockLength;
        posFlag.y = goal[1] * blockLength;
        posFlag.z = goal[2] * blockLength;
        flag.transform.position = posFlag;
        flag.transform.parent = gameObject.transform;
    }

    public GameObject RenderMainCharacter(in List<int> playerStart, in int playerOrientation)
    {
        GameObject mainCharacter = LevelManager.instance.LevelObjects.GetMainCharacterInstance();
        Vector3 posNewChar;
        posNewChar.x = playerStart[0] * blockLength;
        posNewChar.y = playerStart[1] * blockLength;
        posNewChar.z = playerStart[2] * blockLength;

        mainCharacter.transform.Rotate(0, 90f * playerOrientation, 0);
        mainCharacter.transform.position = posNewChar;
        mainCharacter.transform.parent = gameObject.transform;
        return mainCharacter;
    }

    private ObjectType Get(in List<int> mapAndItems, in List<int> levelSize, in int x, in int y, in int z)
    {
        if (x < 0 || x >= levelSize[0]) return ObjectType.NoBlock;
        if (y < 0 || y >= levelSize[1]) return ObjectType.NoBlock;
        if (z < 0 || z >= levelSize[2]) return ObjectType.NoBlock;
        return (ObjectType)mapAndItems[x + z * levelSize[0] + y * (levelSize[0] * levelSize[2])];
    }
}