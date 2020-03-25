using UnityEngine;
using static LevelObject;

public class LevelObjects : MonoBehaviour
{
    [SerializeField] private Block[] blocks = new Block[0];
    [SerializeField] private Item[] items = new Item[0];

    /*[SerializeField] private Block NoBlock;
    [SerializeField] private Block WaterBlock;
    [SerializeField] private Block LavaBlock;
    [SerializeField] private Block SolidBlock;
    [SerializeField] private Block LiftBlock;
    [SerializeField] private Block SpikesBlock;
    [SerializeField] private Block IceBlock;

    [SerializeField] private Item PlankItem;
    [SerializeField] private Item Flag;
    [SerializeField] private Item Fan;*/

    [SerializeField] private GameObject MainCharacter;
    [SerializeField] private GameObject MiniCharacter;

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
        return Instantiate(MainCharacter, MainCharacter.transform.position, MainCharacter.transform.rotation);
    }

    public GameObject GetMiniCharacterInstance()
    {
        return Instantiate(MiniCharacter, MiniCharacter.transform.position, MiniCharacter.transform.rotation);
    }

    // Start is called before the first frame update
    private void Start()
    {
        gameObject.SetActive(false);
    }
}