﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectFactory : MonoBehaviour
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

    public GameObject GetGameObjectInstance(int id)
    {
        GameObject reference = MegaSwitch(id);
        return Instantiate(reference, new Vector3(0, 0, 0), Quaternion.identity);

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

    public GameObject GetMainCharacterInstance()
    {
        
        return Instantiate(MainCharacter, new Vector3(0, 0, 0), Quaternion.identity);

    }

    public GameObject GetFlagInstance()
    {

        return Instantiate(Flag, new Vector3(0, 0, 0), Quaternion.identity);

    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
