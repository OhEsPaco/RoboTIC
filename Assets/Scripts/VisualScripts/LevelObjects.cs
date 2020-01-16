using UnityEngine;
using static ObjectConstants;

public class LevelObjects : MonoBehaviour
{
    public GameObject NoBlock;
    public GameObject WaterBlock;
    public GameObject LavaBlock;
    public GameObject SolidBlock;
    public GameObject LiftBlock;
    public GameObject SpikesBlock;
    public GameObject IceBlock;

    public GameObject PlankItem;
    public GameObject MainCharacter;
    public GameObject Flag;

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