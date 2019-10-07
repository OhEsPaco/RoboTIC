using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// Defines the <see cref="NewBlock" />
/// </summary>
public class NewBlock : MonoBehaviour
{

    public GameObject cube;
    /// <summary>
    /// Defines the lwatcher
    /// </summary>
    public LevelWatcher lwatcher;

    /// <summary>
    /// Defines the type
    /// </summary>
    public string type;

    // Start is called before the first frame update
    /// <summary>
    /// The Start
    /// </summary>
    internal void Start()
    {
    }

    /// <summary>
    /// The addBlock
    /// </summary>
    public void addBlock()
    {

        bool insidePlane = false;
        GameObject plane = GameObject.Find("Table Generator");
        AABB2D aabbplane = plane.GetComponent<PlaneTest>().Aabb;

        float[] lengths = calculateMeshLength(cube);

        Vector3 planeMin = new Vector3(plane.GetComponent<PlaneTest>().X - (plane.GetComponent<PlaneTest>().LengthX), 0, plane.GetComponent<PlaneTest>().Z - (plane.GetComponent<PlaneTest>().LengthZ));

        transform.position = calculateGridPoint(transform.position, planeMin, 2);

        Vector2 minPoint = new Vector2(transform.position.x - (lengths[0] / 2f), transform.position.z - (lengths[1] / 2f));
        Vector2 maxPoint = new Vector2(transform.position.x + (lengths[0] / 2f), transform.position.z + (lengths[1] / 2f));

        AABB2D aabb = createAABB2DPlane(minPoint, maxPoint);

        

        if (aabbplane.isCompletelyInside(aabb))
        {
            insidePlane = true;
        }



        if (insidePlane)
        {

             
            //The block is inside the plane
            lwatcher.addBlock(transform.position, type);
        }
        else
        {
            //Unable to place the block
            destroyThis();
        }
    }

    /// <summary>
    /// The destroyThis
    /// </summary>
    private void destroyThis()
    {
        cube.GetComponent<Explode>().explode();
        StartCoroutine(Destroy(cube.GetComponent<Explode>().particleDuration));
        
    }

    /// <summary>
    /// The OnMouseOver
    /// </summary>
    void OnMouseOver()
    {
        if(Input.GetMouseButtonDown(1)){

            GetComponent<Rigidbody>().detectCollisions = false;
            destroyThis();
        }
    }

    internal IEnumerator Destroy(float seconds)
    {

        yield return new WaitForSecondsRealtime(seconds);
        Destroy(gameObject);
    }

    private float[] calculateMeshLength(GameObject objectToBeMeasured)
    {
        Vector3 objectSize = Vector3.Scale(objectToBeMeasured.transform.localScale, objectToBeMeasured.GetComponent<MeshFilter>().mesh.bounds.size);
        float[] size = new float[2];
        size[0] = objectSize.x;
        size[1] = objectSize.z;
        return size;
    }

    private AABB2D createAABB2DPlane(Vector2 min, Vector2 max)
    {

        AABB2D aabb = new AABB2D(min.x, min.y, max.x, max.y);
        return aabb;
    }

    //0 min point
    //1 max point
    private Vector2[] calculatePlanePoints(GameObject plane, float fullLenghtX, float fullLenghtZ)
    {
        Vector2[] points = new Vector2[4];
       
        Vector3 position = plane.transform.position;

        float lenghtX = fullLenghtX / 2f;
        float lenghtZ = fullLenghtZ / 2f;

        
        points[0] = new Vector2(position.x - lenghtX, position.z - lenghtZ);
        points[1] = new Vector2(position.x + lenghtX, position.z + lenghtZ);
        points[2] = new Vector2(position.x - lenghtX, position.z + lenghtZ);
        points[3] = new Vector2(position.x + lenghtX, position.z - lenghtZ);

        return points;
    }

    private Vector3 calculateGridPoint(Vector3 position, Vector3 planeMinPos, float blockLength)
    {
        int xfactor = (int)Math.Abs(((position.x-planeMinPos.x) / blockLength));
        int zfactor = (int)Math.Abs(((position.z - planeMinPos.z) / blockLength));
        Debug.Log(xfactor);

        Vector3 newPos = new Vector3((planeMinPos.x + (blockLength / 2f) + xfactor * blockLength),position.y, (planeMinPos.z + (blockLength / 2f) + zfactor * blockLength));
        return newPos;
    }
  
}
