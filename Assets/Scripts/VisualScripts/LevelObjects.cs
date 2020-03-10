using UnityEngine;
using static ObjectConstants;

public class LevelObjects : MonoBehaviour
{
    [SerializeField] private Block NoBlock;
    [SerializeField] private Block WaterBlock;
    [SerializeField] private Block LavaBlock;
    [SerializeField] private Block SolidBlock;
    [SerializeField] private Block LiftBlock;
    [SerializeField] private Block SpikesBlock;
    [SerializeField] private Block IceBlock;

    [SerializeField] private Item PlankItem;
    [SerializeField] private Item Flag;
    [SerializeField] private Item Fan;

    [SerializeField] private GameObject MainCharacter;
    [SerializeField] private GameObject MiniCharacter;


    public LevelObject GetGameObjectInstance(in ObjectType id)
    {
        LevelObject reference = MegaSwitch(id);
        return InstantiateObject(reference);
    }

    private LevelObject MegaSwitch(in ObjectType id)
    {
        switch (id)
        {
            case ObjectType.NoBlock:
                return NoBlock;

            case ObjectType.WaterBlock:
                return WaterBlock;

            case ObjectType.LavaBlock:
                return LavaBlock;

            case ObjectType.SolidBlock:
                return SolidBlock;

            case ObjectType.LiftBlock:
                return LiftBlock;

            case ObjectType.SpikesBlock:
                return SpikesBlock;

            case ObjectType.IceBlock:
                return IceBlock;

            case ObjectType.PlankItem:
                return PlankItem;
            case ObjectType.FanItem:
                return Fan;
            default:
                return NoBlock;
        }
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

    public LevelObject GetFlagInstance()
    {
        return InstantiateObject(Flag);
    }

    // Start is called before the first frame update
    private void Start()
    {
        gameObject.SetActive(false);
    }
}