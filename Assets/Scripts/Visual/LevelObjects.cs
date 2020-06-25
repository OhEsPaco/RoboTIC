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
        blocks = GetComponentsInChildren<Block>(true);
        items = GetComponentsInChildren<Item>(true);
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

    public GameObject GetMainCharacterInstance()
    {
        return MainCharacter;
    }

    public GameObject InstantiateMainCharacter()
    {
        return Instantiate(MainCharacter);
    }

    public GameObject GetMiniCharacterInstance()
    {
        return Instantiate(MiniCharacter, MiniCharacter.transform.position, MiniCharacter.transform.rotation);
    }

    // Start is called before the first frame update
}