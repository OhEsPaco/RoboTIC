using UnityEngine;

public class LevelManager : MonoBehaviour
{
    //VISUAL
    [SerializeField] private LevelObjects levelObjects;

    [SerializeField] private MapRenderer mapRenderer;
    [SerializeField] private LevelButtons levelButtons;
    [SerializeField] private RoadFactory roadFactory;

    //LOGIC
    [SerializeField] private Logic logic;
   [SerializeField] private RoadMovementLogic roadMovement;
 

    [SerializeField] private RoadPlacementLogic roadPlacementLogic;

    //PERSISTENCE
    [SerializeField] private JSonLoader jSonLoader;

    [SerializeField] private Shader commonShader;

    private static LevelManager levelManager;

    private void Awake()
    {
       /* MeshRenderer[] allRenderers = FindObjectsOfType<MeshRenderer>();

        foreach (MeshRenderer r in allRenderers)
        {
            foreach(Material m in r.materials)
            {
                m.shader = commonShader;
            }

        }*/

    }
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
    public Logic Logic { get => logic; }
    public JSonLoader JSonLoader { get => jSonLoader; }
    public RoadPlacementLogic RoadPlacementLogic { get => roadPlacementLogic; }
    public RoadFactory RoadFactory { get => roadFactory; }
    public RoadMovementLogic RoadMovement { get => roadMovement;  }
}