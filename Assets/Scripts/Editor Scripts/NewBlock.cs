using System.Collections;
using UnityEngine;

/// <summary>
/// Defines the <see cref="NewBlock" />
/// </summary>
public class NewBlock : MonoBehaviour
{
    public GameObject cube;

    /// <summary>
    /// Defines the lwatcher
    /// </summary>
    private LevelWatcher lwatcher;

    /// <summary>
    /// Defines the type
    /// </summary>
    public string type;

    private bool placed = false;

    internal void Start()
    {
        lwatcher = GameObject.Find("Level Watcher").GetComponent<LevelWatcher>();
    }

    public void UpdatePos(float newX, float newZ)
    {
        if (!placed)
        {
            GameObject plane = GameObject.Find("Table Generator");

            Vector3 planeMin = new Vector3(plane.GetComponent<PlaneTest>().x - (plane.GetComponent<PlaneTest>().LengthX / 2f), 0, plane.GetComponent<PlaneTest>().z - (plane.GetComponent<PlaneTest>().LengthZ / 2f));

            int xFactor = CommonMath.GetXfactor(transform.position, planeMin, 2);

            int zFactor = CommonMath.GetZfactor(transform.position, planeMin, 2);

            transform.position = CommonMath.CalculateGridPoint(new Vector3(newX, 0, newZ), planeMin, 2);

            transform.position = new Vector3(transform.position.x, plane.GetComponent<PlaneTest>().y + (lwatcher.Lowest(xFactor, zFactor) * 2) + 1, transform.position.z);

            //}
        }
    }

    internal void Update()
    {
    }

    /// <summary>
    /// The addBlock
    /// </summary>
    public void addBlock()
    {
        GameObject plane = GameObject.Find("Table Generator");

        Vector3 planeMin = new Vector3(plane.GetComponent<PlaneTest>().x - (plane.GetComponent<PlaneTest>().LengthX / 2f), plane.GetComponent<PlaneTest>().y, plane.GetComponent<PlaneTest>().z - (plane.GetComponent<PlaneTest>().LengthZ / 2f));

        int xFactor = CommonMath.GetXfactor(transform.position, planeMin, 2);
        int yFactor = CommonMath.GetYfactor(transform.position, planeMin, 2);
        int zFactor = CommonMath.GetZfactor(transform.position, planeMin, 2);

        if (xFactor >= lwatcher.maxBlocksX || xFactor < 0 || zFactor >= lwatcher.maxBlocksZ || zFactor < 0 || yFactor >= lwatcher.maxBlocksY || yFactor < 0)
        {
            //Unable to place the block
            destroyThis();
        }
        else
        {
            //The block is inside the plane
            Block newBlock = new Block(type);
            Debug.Log(yFactor);
            int res = lwatcher.AddBlock(xFactor, yFactor, zFactor, newBlock);
            if (res != LevelWatcher.OK)
            {
                destroyThis();
            }
            else
            {
                placed = true;
            }
        }
    }

    /// <summary>
    /// The destroyThis
    /// </summary>
    private void destroyThis()
    {
        GameObject plane = GameObject.Find("Table Generator");
        Vector3 planeMin = new Vector3(plane.GetComponent<PlaneTest>().x - (plane.GetComponent<PlaneTest>().LengthX / 2f), 0, plane.GetComponent<PlaneTest>().z - (plane.GetComponent<PlaneTest>().LengthZ / 2f));
        int xFactor = CommonMath.GetXfactor(transform.position, planeMin, 2);
        int yFactor = CommonMath.GetYfactor(transform.position, planeMin, 2);
        int zFactor = CommonMath.GetZfactor(transform.position, planeMin, 2);

        lwatcher.deleteBlock(xFactor, yFactor, zFactor);
        cube.GetComponent<Explode>().explode();
        StartCoroutine(DestroyAfterSeconds(cube.GetComponent<Explode>().particleDuration));
    }

    /// <summary>
    /// The OnMouseOver
    /// </summary>
    private void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(1))
        {
            GetComponent<Rigidbody>().detectCollisions = false;
            destroyThis();
        }
    }

    internal IEnumerator DestroyAfterSeconds(float seconds)
    {
        yield return new WaitForSecondsRealtime(seconds);
        Destroy(gameObject);
    }
}