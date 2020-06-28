using UnityEngine;
using static LevelObject;

public class LevelObjects : MonoBehaviour
{
    private Block[] blocks;
    private Item[] items;
    [SerializeField] private GameObject MainCharacter;
    [SerializeField] private GameObject MiniCharacter;

    private void Awake()
    {
        // blocks = GetComponentsInChildren<Block>(true);
        // items = GetComponentsInChildren<Item>(true);
        blocks = Resources.LoadAll<Block>("Prefabs/Blocks");
        items = Resources.LoadAll<Item>("Prefabs/Items");
       
        EventAggregator.Instance.Subscribe<MsgRenderMainCharacter>(GetMainCharacterInstance);
    }

    public LevelObject GetGameObjectInstance(in int id)
    {
        LevelObject reference = MegaSwitch(id);
        return InstantiateObject(reference);
    }

    private LevelObject MegaSwitch(in int id)
    {
        foreach (Block block in blocks)
        {
            if (block.BlockType == (Blocks)id)
            {
                return block;
            }
        }

        foreach (Item item in items)
        {
            if (item.ItemType == (Items)id)
            {
                return item;
            }
        }

        return null;
    }

    private LevelObject InstantiateObject(LevelObject reference)
    {
        return Instantiate(reference, reference.transform.position, reference.transform.rotation);
    }

    public void GetMainCharacterInstance(MsgRenderMainCharacter msg)
    {
       
        GameObject mainCharacter = Instantiate(MainCharacter);
        Debug.LogError("3333");
        EventAggregator.Instance.Publish(new ResponseWrapper<MsgRenderMainCharacter, GameObject>(msg, mainCharacter));

      
    }

    public GameObject InstantiateMainCharacter()
    {
        return Instantiate(MainCharacter);
    }

    public GameObject GetMiniCharacterInstance()
    {
        return Instantiate(MiniCharacter, MiniCharacter.transform.position, MiniCharacter.transform.rotation);
    }
}