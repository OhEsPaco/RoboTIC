using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static LevelObject;

public class MapRenderer : MonoBehaviour
{
    [SerializeField] private float blockLength = 1f;
    [SerializeField] private EventAggregator eventAggregator;
    private LevelObjects levelObjects;
    public float BlockLength { get => blockLength; }

    private static MapRenderer mapRenderer;

    public static MapRenderer Instance
    {
        get
        {
            if (!mapRenderer)
            {
                mapRenderer = FindObjectOfType(typeof(MapRenderer)) as MapRenderer;

                if (!mapRenderer)
                {
                    Debug.LogError("There needs to be one active MapRenderer script on a GameObject in your scene.");
                }
            }

            return mapRenderer;
        }
    }

    private void Awake()
    {
        eventAggregator.Subscribe<MsgRenderMapAndItems>(RenderMapAndItems);
        eventAggregator.Subscribe<MsgBlockLength>(ServeBlockLength);
        levelObjects = GetComponentInChildren<LevelObjects>();
    }

    private void ServeBlockLength(MsgBlockLength msg)
    {
        eventAggregator.Publish<ResponseWrapper<MsgBlockLength, float>>(new ResponseWrapper<MsgBlockLength, float>(msg, blockLength));
    }

    private void RenderMapAndItems(MsgRenderMapAndItems msg)
    {
        StartCoroutine(RenderMapAndItemsCoroutine(msg));
    }

    private IEnumerator RenderMapAndItemsCoroutine(MsgRenderMapAndItems msg)
    {
        List<int> levelSize = msg.LevelSize;
        List<int> mapAndItems = msg.MapAndItems;
        LevelObject[] objectReferences = new LevelObject[levelSize[0] * levelSize[1] * levelSize[2]];
        for (int y = 0; y < levelSize[1]; y++)
        {
            for (int x = 0; x < levelSize[0]; x++)
            {
                for (int z = 0; z < levelSize[2]; z++)
                {
                    int blockToSpawn = Get(mapAndItems, levelSize, x, y, z);

                    LevelObject block = levelObjects.GetGameObjectInstance(blockToSpawn);

                    objectReferences[x + z * levelSize[0] + y * (levelSize[0] * levelSize[2])] = block;
                    Vector3 posNew;
                    posNew.x = gameObject.transform.position.x + x * blockLength;
                    posNew.y = gameObject.transform.position.y + y * blockLength;
                    posNew.z = gameObject.transform.position.z + z * blockLength;
                    block.transform.position = posNew;
                    block.transform.parent = gameObject.transform;

                    if (x == msg.Goal[0] && y == msg.Goal[1]-1 && z == msg.Goal[2])
                    {
                        LevelObject flag = levelObjects.GetGameObjectInstance((int)Items.FlagItem);
                        Vector3 posFlag;
                        posFlag.x = gameObject.transform.position.x + msg.Goal[0] * blockLength;
                        posFlag.y = gameObject.transform.position.y + msg.Goal[1] * blockLength;
                        posFlag.z = gameObject.transform.position.z + msg.Goal[2] * blockLength;
                        flag.transform.position = posFlag;
                        flag.transform.parent = block.transform;
                    }

                    block.gameObject.SetActive(false);
                }
                yield return null;
            }
        }

        eventAggregator.Publish(new ResponseWrapper<MsgRenderMapAndItems, LevelObject[]>(msg, objectReferences));
    }

    public LevelObject SpawnBlock(int blockToSpawn)
    {
        LevelObject block = levelObjects.GetGameObjectInstance(blockToSpawn);

        return block;
    }

    public GameObject RenderMainCharacter()
    {
        return levelObjects.InstantiateMainCharacter();
    }

    private int Get(in List<int> mapAndItems, in List<int> levelSize, in int x, in int y, in int z)
    {
        if (x < 0 || x >= levelSize[0]) return (int)Blocks.NoBlock;
        if (y < 0 || y >= levelSize[1]) return (int)Blocks.NoBlock;
        if (z < 0 || z >= levelSize[2]) return (int)Blocks.NoBlock;
        return mapAndItems[x + z * levelSize[0] + y * (levelSize[0] * levelSize[2])];
    }
}