﻿using UnityEngine;

public class LevelManager : MonoBehaviour
{
    //VISUAL
    [SerializeField] private LevelObjects levelObjects;

    [SerializeField] private MapRenderer mapRenderer;
    [SerializeField] private LevelButtons levelButtons;
    [SerializeField] private ActionRenderer actionRenderer;
    [SerializeField] private RoadFactory roadFactory;

    //LOGIC
    [SerializeField] private Logic logic;
   
 

    [SerializeField] private RoadPlacementLogic roadPlacementLogic;

    //PERSISTENCE
    [SerializeField] private JSonLoader jSonLoader;

    private static LevelManager levelManager;

    public static LevelManager instance
    {
        get
        {
            if (!levelManager)
            {
                levelManager = FindObjectOfType(typeof(LevelManager)) as LevelManager;

                if (!levelManager)
                {
                    Debug.LogError("There needs to be one active LevelManager script on a GameObject in your scene.");
                }
            }

            return levelManager;
        }
    }

    public LevelObjects LevelObjects { get => levelObjects; }
    public MapRenderer MapRenderer { get => mapRenderer; }
    public LevelButtons LevelButtons { get => levelButtons; }
    public ActionRenderer ActionRenderer { get => actionRenderer; }
    public Logic Logic { get => logic; }
    public JSonLoader JSonLoader { get => jSonLoader; }
    public RoadPlacementLogic RoadPlacementLogic { get => roadPlacementLogic; }
    public RoadFactory RoadFactory { get => roadFactory; }
}