using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static LevelObject;

public class MapRenderer : MonoBehaviour
{
    [SerializeField] private float blockLength = 1f;
    [SerializeField] private EventAggregator eventAggregator;
    private LevelObjectFactory levelObjects;
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
        levelObjects = GetComponentInChildren<LevelObjectFactory>();
    }

    private void ServeBlockLength(MsgBlockLength msg)
    {
        eventAggregator.Publish<ResponseWrapper<MsgBlockLength, float>>(new ResponseWrapper<MsgBlockLength, float>(msg, blockLength));
    }

    private void RenderMapAndItems(MsgRenderMapAndItems msg)
    {
        StartCoroutine(RenderMapAndItemsCoroutine(msg));
    }

    //objToSpawn.transform.position = new Vector3(transform.position.x + blockLength * x, transform.position.y - blockLength / 2, transform.position.z + blockLength * z);
    //objToSpawn.transform.RotateAround(transform.position, Vector3.up, transform.eulerAngles.y);
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
                    block.transform.position = new Vector3(msg.MapParent.transform.position.x + blockLength * x, msg.MapParent.transform.position.y + blockLength * y, msg.MapParent.transform.position.z + blockLength * z);
                    block.transform.RotateAround(msg.MapParent.transform.position, Vector3.up, msg.MapParent.transform.eulerAngles.y);
                    block.transform.parent = msg.MapParent.transform;

                    if (x == msg.Goal[0] && y == msg.Goal[1] - 1 && z == msg.Goal[2])
                    {
                        LevelObject flag = levelObjects.GetGameObjectInstance((int)Items.FlagItem);
                        flag.transform.position = new Vector3(msg.MapParent.transform.position.x + blockLength * msg.Goal[0], msg.MapParent.transform.position.y + blockLength * msg.Goal[1], msg.MapParent.transform.position.z + blockLength * msg.Goal[2]);
                        flag.transform.RotateAround(msg.MapParent.transform.position, Vector3.up, msg.MapParent.transform.eulerAngles.y);
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