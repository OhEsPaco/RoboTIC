using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{

    //VISUAL
    private LevelObjects levelObjects;
    private MapRenderer mapRenderer;
    private LevelButtons levelButtons;
    private LevelRoads levelRoads;

    //LOGIC
    private Logic logic;

    //PERSISTENCE
    private JSonLoader jSonLoader;

    static LevelManager levelManager;

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

 
    public MapRenderer MapRenderer { get => mapRenderer; set => mapRenderer = value; }
    public JSonLoader JSonLoader { get => jSonLoader; set => jSonLoader = value; }
    public Logic Logic { get => logic; set => logic = value; }
    public LevelButtons LevelButtons { get => levelButtons; set => levelButtons = value; }
    public LevelRoads LevelRoads { get => levelRoads; set => levelRoads = value; }
    public LevelObjects LevelObjects { get => levelObjects; set => levelObjects = value; }
}
