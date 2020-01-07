using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ObjectConstants;

public class LevelObjects : MonoBehaviour
{
    private LevelManager manager;
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

    

    public GameObject GetGameObjectInstance(ObjectType id)
    {
        GameObject reference = MegaSwitch(id);
        return InstantiateObject(reference);

    }
    private GameObject MegaSwitch(ObjectType id)
    {
        switch (id)
        {
            case ObjectType.NoBlock:
                return NoBlock;
                break;
            case ObjectType.WaterBlock:
                return WaterBlock;
                break;
            case ObjectType.LavaBlock:
                return LavaBlock;
                break;
            case ObjectType.SolidBlock:
                return SolidBlock;
                break;
            case ObjectType.LiftBlock:
                return LiftBlock;
                break;
            case ObjectType.SpikesBlock:
                return SpikesBlock;
                break;

            case ObjectType.IceBlock:
                return IceBlock;
                break;

            case ObjectType.PlankItem:
                return PlankItem;
                break;

            default:
                return NoBlock;
                break;

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
    void Awake()
    {
        manager = LevelManager.instance;
        
        gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
