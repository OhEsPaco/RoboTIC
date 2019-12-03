using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelRoads : MonoBehaviour
{
    private LevelManager manager;
    void Awake()
    {
        manager = LevelManager.instance;
        manager.LevelRoads = this;
        gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
