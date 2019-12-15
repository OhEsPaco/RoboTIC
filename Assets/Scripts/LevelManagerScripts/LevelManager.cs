using UnityEngine;

public class LevelManager : MonoBehaviour
{
    //VISUAL
    public LevelObjects LevelObjects;
    public MapRenderer MapRenderer;
    public LevelButtons LevelButtons;
    public LevelRoads LevelRoads;
    public ActionRenderer ActionRenderer;

    //LOGIC
    public Logic Logic;

    //PERSISTENCE
    public JSonLoader JSonLoader;

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
}