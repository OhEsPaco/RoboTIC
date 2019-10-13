using UnityEngine;

public class LevelWatcher : MonoBehaviour
{
    public static LevelWatcher instance = null;
    public static readonly int OK = 0;
    public static readonly int OCCUPIED_SPACE = 1;
    public static readonly int OUT = -1;
    /// <summary>
    /// Defines the number of blocks on the X axis
    /// </summary>
    public readonly int blocksX = 10;

    public readonly int blocksY = 5;

    public readonly int blocksZ = 10;

    private IBlockArray blocks;

    private void Awake()
    {
        //Check if instance already exists
        if (instance == null)

            //if not, set instance to this
            instance = this;

        //If instance already exists and it's not this:
        else if (instance != this)

            //Then destroy this. This enforces our singleton pattern, meaning there can only ever be one instance of a GameManager.
            Destroy(gameObject);

        //Sets this to not be destroyed when reloading scene
        DontDestroyOnLoad(gameObject);
    }

    // Start is called before the first frame update
    private void Start()
    {
        this.blocks = new BlockArrayLinear(blocksX, blocksY, blocksZ);
    }

    public int Lowest(int x, int z)
    {
        return blocks.Lowest(x, z);
    }

    public bool IsOccupied(int x, int y, int z)
    {
        Block tmp = blocks.Get(x, y, z);

        if (tmp == null)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    

    public int AddBlock(int x, int y, int z, Block block)
    {
        Block tmp = blocks.Get(x, y, z);
        Debug.Log(x + " " + y + " " + z);

        if (x < 0 || x >= blocksX || y<0 || y>=blocksY ||z<0||z>=blocksZ)
        {
            Debug.Log("OUT");
            return OUT;
        }
        if (tmp == null)
        {
            Debug.Log("OK");
            blocks.Set(x, y, z, block);
            return OK;
        }
        else
        {
            Debug.Log("OCUPPIED");
            return OCCUPIED_SPACE;
        }
    }

    /// <summary>
    /// The deleteBlock
    /// </summary>
    /// <param name="position">The position<see cref="Vector3"/></param>
    /// <param name="type">The type<see cref="string"/></param>
    public void DeleteBlock(int x, int y, int z)
    {
        blocks.Set(x, y, z, null);
    }
}