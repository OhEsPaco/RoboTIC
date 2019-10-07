using UnityEngine;

/// <summary>
/// Defines the <see cref="PlaneTest" />
/// </summary>
public class PlaneTest : MonoBehaviour
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
    /// Defines the number of blocks on the X axis
    /// </summary>
    public int blocksX = 10;

    /// <summary>
    /// Defines the number of blocks on the Z axis
    /// </summary>
    public int blocksZ = 10;

    /// <summary>
    /// Defines the material of the plane
    /// </summary>
    public Material planeMaterial;

    /// <summary>
    /// GameObject for the plane
    /// </summary>
    private GameObject plane;

    /// <summary>
    /// Gets or sets the lenght of the plane on the X axis
    /// </summary>
    public float LengthX { get; set; }

    /// <summary>
    /// Gets or sets the lenght of the plane on the Z axis
    /// </summary>
    public float LengthZ { get; set; }

    /// <summary>
    /// Gets or sets the aabb
    /// </summary>
    public AABB2D Aabb { get; set; }

    /// <summary>
    /// Start
    /// </summary>
    internal void Start()
    {
        CreatePlane();
    }

    /// <summary>
    /// Creates a plane.
    /// </summary>
    private void CreatePlane()
    {
        //First, we spawn a plane at the specified rotation.
        plane = GameObject.CreatePrimitive(PrimitiveType.Plane);
        plane.GetComponent<Renderer>().material = planeMaterial;
        plane.transform.position = new Vector3(x, y, z);
        plane.transform.rotation = Quaternion.identity;

        //Second, the plane is measured.
        float[] planeLengths = CommonMath.CalculateMeshLength(plane.transform.localScale, plane.GetComponent<MeshFilter>().mesh.bounds.size);

        //The plane is scaled acoordingly.
        float scaleX = CommonMath.CalculateScale(blockSize, blocksX, planeLengths[0], plane.transform.localScale.x);
        float scaleZ = CommonMath.CalculateScale(blockSize, blocksZ, planeLengths[1], plane.transform.localScale.z);
        float scaleY = plane.transform.localScale.y;
        plane.transform.localScale = new Vector3(scaleX, scaleY, scaleZ);

        //The plane final length is measured again.
        planeLengths = CommonMath.CalculateMeshLength(plane.transform.localScale, plane.GetComponent<MeshFilter>().mesh.bounds.size);
        LengthX = planeLengths[0];
        LengthZ = planeLengths[1];

        //An AABB for this object is created.
        Vector2 minPoint = new Vector2(plane.transform.position.x - (planeLengths[0] / 2f), plane.transform.position.z - (planeLengths[1] / 2f));
        Vector2 maxPoint = new Vector2(plane.transform.position.x + (planeLengths[0] / 2f), plane.transform.position.z + (planeLengths[1] / 2f));

        Aabb = new AABB2D(minPoint.x, minPoint.y, maxPoint.x, maxPoint.y);

        plane.gameObject.SetActive(true);
    }
}
