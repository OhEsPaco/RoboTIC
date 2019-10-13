using UnityEngine;

public class PlaygroundGrid : MonoBehaviour
{
    /// <summary>
    /// Defines the size of the blocks
    /// </summary>
    public float blockSize = 2f;

    /// <summary>
    /// Defines the x position of the table
    /// </summary>
    public float x = 0f;

    /// <summary>
    /// Defines the y position of the table
    /// </summary>
    public float y = 0f;

    /// <summary>
    /// Defines the z position of the table
    /// </summary>
    public float z = 0f;


    /// <summary>
    /// Defines the material of the plane
    /// </summary>
    public Material planeMaterial;

    /// <summary>
    /// GameObject for the plane
    /// </summary>
    private GameObject usableSurface;
    private LevelWatcher lwatcher;

    public static PlaygroundGrid instance = null;
    private float lengthX;
    private float lengthZ;
    private AABB2D aabb;

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
       // DontDestroyOnLoad(gameObject);
    }

    /// <summary>
    /// Start
    /// </summary>
    internal void Start()
    {
        lwatcher = LevelWatcher.instance;
        CreatePlane();
    }

    /// <summary>
    /// Creates a plane.
    /// </summary>
    private void CreatePlane()
    {
        //First, we spawn a plane at the specified rotation.
        usableSurface = GameObject.CreatePrimitive(PrimitiveType.Plane);
        //plane.transform.parent = gameObject.transform;
        usableSurface.GetComponent<Renderer>().material = planeMaterial;
        usableSurface.transform.position = new Vector3(x, y, z);
        usableSurface.transform.rotation = Quaternion.identity;

        //Second, the plane is measured.
        float[] planeLengths = CalculateMeshLength(usableSurface.transform.localScale, usableSurface.GetComponent<MeshFilter>().mesh.bounds.size);

        //The plane is scaled acoordingly.
        float scaleX = CalculateScale(blockSize, lwatcher.blocksX, planeLengths[0], usableSurface.transform.localScale.x);
        float scaleZ = CalculateScale(blockSize, lwatcher.blocksZ, planeLengths[1], usableSurface.transform.localScale.z);
        float scaleY = usableSurface.transform.localScale.y;
        usableSurface.transform.localScale = new Vector3(scaleX, scaleY, scaleZ);

        //The plane final length is measured again.
        planeLengths = CalculateMeshLength(usableSurface.transform.localScale, usableSurface.GetComponent<MeshFilter>().mesh.bounds.size);
        lengthX = planeLengths[0];
        lengthZ = planeLengths[1];

        //An AABB for this object is created.
        Vector2 minPoint = new Vector2(usableSurface.transform.position.x - (planeLengths[0] / 2f), usableSurface.transform.position.z - (planeLengths[1] / 2f));
        Vector2 maxPoint = new Vector2(usableSurface.transform.position.x + (planeLengths[0] / 2f), usableSurface.transform.position.z + (planeLengths[1] / 2f));

        aabb = new AABB2D(minPoint.x, minPoint.y, maxPoint.x, maxPoint.y);

        usableSurface.gameObject.SetActive(true);
    }

    public Vector3 SnapToGrid(Vector3 position)
    {
        Vector3 planeMin = new Vector3(usableSurface.transform.position.x - (lengthX / 2f), usableSurface.transform.position.y, usableSurface.transform.position.z - (lengthZ / 2f));

        int xfactor = (int)((position.x - planeMin.x) / blockSize);
        int zfactor = (int)((position.z - planeMin.z) / blockSize);

        float newX = (planeMin.x + (blockSize / 2f) + xfactor * blockSize);
        float newZ = (planeMin.z + (blockSize / 2f) + zfactor * blockSize)- (blockSize/2f);
        float newY = planeMin.y + (lwatcher.Lowest(xfactor, zfactor) * blockSize) + (blockSize / 2f);


        Vector3 newPos = new Vector3(newX, newY,newZ);
        return newPos;
    }

    public Block AddBlock(GameObject spawnedBlock, string type)
    {
        spawnedBlock.transform.position = SnapToGrid(spawnedBlock.transform.position);
        Vector3 position = spawnedBlock.transform.position;
        Vector3 planeMin = new Vector3(usableSurface.transform.position.x - (lengthX / 2f), usableSurface.transform.position.y, usableSurface.transform.position.z - (lengthZ / 2f));
        int xfactor = (int)((position.x - planeMin.x) / blockSize);
        int yfactor = (int)((position.y - planeMin.y) / blockSize);
        int zfactor = (int)((position.z - planeMin.z) / blockSize);

        if (lwatcher.IsOccupied(xfactor, yfactor, zfactor))
        {
            return null;
        }

        Block b = new Block(type);
        if(lwatcher.AddBlock(xfactor, yfactor, zfactor, b) == LevelWatcher.OK)
        {
            return b;
        }

        return null;


    }
    private float[] CalculateMeshLength(Vector3 localScale, Vector3 boundsSize)
    {
        Vector3 objectSize = Vector3.Scale(localScale, boundsSize);
        float[] size = new float[2];
        size[0] = objectSize.x;
        size[1] = objectSize.z;
        return size;
    }

    private float CalculateScale(float blockSize, float numberOfBlocks, float actualLenght, float actualScale)
    {
        return (numberOfBlocks * blockSize * actualScale) / actualLenght;
    }
}