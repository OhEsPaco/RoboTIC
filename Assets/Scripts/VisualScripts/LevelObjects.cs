using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    public GameObject GetGameObjectInstance(int id)
    {
        GameObject reference = MegaSwitch(id);
        return InstantiateObject(reference);

    }
    private GameObject MegaSwitch(int id)
    {
        switch (id)
        {
            case ObjectConstants.NoBlock:
                return NoBlock;
                break;
            case ObjectConstants.WaterBlock:
                return WaterBlock;
                break;
            case ObjectConstants.LavaBlock:
                return LavaBlock;
                break;
            case ObjectConstants.SolidBlock:
                return SolidBlock;
                break;
            case ObjectConstants.LiftBlock:
                return LiftBlock;
                break;
            case ObjectConstants.SpikesBlock:
                return SpikesBlock;
                break;

            case ObjectConstants.IceBlock:
                return IceBlock;
                break;

            case ObjectConstants.PlankItem:
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
