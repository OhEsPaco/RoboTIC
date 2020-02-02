using UnityEngine;
using static ObjectConstants;

public class LevelObjects : MonoBehaviour
{
    [SerializeField] private GameObject NoBlock;
    [SerializeField] private GameObject WaterBlock;
    [SerializeField] private GameObject LavaBlock;
    [SerializeField] private GameObject SolidBlock;
    [SerializeField] private GameObject LiftBlock;
    [SerializeField] private GameObject SpikesBlock;
    [SerializeField] private GameObject IceBlock;

    [SerializeField] private GameObject PlankItem;
    [SerializeField] private GameObject MainCharacter;
    [SerializeField] private GameObject MiniCharacter;
    [SerializeField] private GameObject Flag;
    [SerializeField] private GameObject Fan;
    public GameObject GetGameObjectInstance(in ObjectType id)
    {
        GameObject reference = MegaSwitch(id);
        return InstantiateObject(reference);
    }

    private GameObject MegaSwitch(in ObjectType id)
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

    private GameObject InstantiateObject(GameObject reference)
    {
        return Instantiate(reference, reference.transform.position, reference.transform.rotation);
    }

    public GameObject GetMainCharacterInstance()
    {
        return InstantiateObject(MainCharacter);
    }

    public GameObject GetMiniCharacterInstance()
    {
        return InstantiateObject(MiniCharacter);
    }

    public GameObject GetFlagInstance()
    {
        return InstantiateObject(Flag);
    }

    // Start is called before the first frame update
    private void Start()
    {
        gameObject.SetActive(false);
    }
}