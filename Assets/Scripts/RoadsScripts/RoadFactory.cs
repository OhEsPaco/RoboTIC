﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static RoadConstants;

public class RoadFactory : MonoBehaviour
{

   [SerializeField] private Road[] roads;
    private LevelManager manager;

    // Start is called before the first frame update
    void Start()
    {
        roads = GetComponentsInChildren<Road>(true);
    }

    void Awake()
    {
        manager = LevelManager.instance;

    }
    private GameObject InstantiateObject(GameObject reference)
    {
        return Instantiate(reference, reference.transform.position, reference.transform.rotation);
    }

    public GameObject GetGameObjectInstance(RoadType id)
    {

        foreach(Road road in roads)
        {
            if (road.RoadType == id)
            {
                GameObject reference = road.gameObject;
                return InstantiateObject(reference);
            }
        }
        return null;
        
    }
}
