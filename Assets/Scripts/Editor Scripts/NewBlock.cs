using System.Collections;
using System.Collections.Generic;
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

        bool insidePlane = true;
        // Bit shift the index of the layer to get a bit mask
        int layerMask = 1 << 10;

        // This would cast rays only against colliders in layer 8.
        // But instead we want to collide against everything except layer 8. The ~ operator does this, it inverts a bitmask.
        layerMask = ~layerMask;

        RaycastHit hit;

        Vector3 posRay = new Vector3(transform.position.x - 1, transform.position.y, transform.position.z + 1);
        // Does the ray intersect any objects excluding the player layer
        if (Physics.Raycast(posRay, transform.TransformDirection(Vector3.down), out hit, 5f))
        {
            Debug.DrawRay(posRay, transform.TransformDirection(Vector3.down) * hit.distance, Color.yellow);
            
        }
        else
        {
            Debug.DrawRay(posRay, transform.TransformDirection(Vector3.down) * 1000, Color.white);
            insidePlane = false;

        }


        posRay = new Vector3(transform.position.x - 1, transform.position.y, transform.position.z - 1);
        // Does the ray intersect any objects excluding the player layer
        if (Physics.Raycast(posRay, transform.TransformDirection(Vector3.down), out hit, 5f))
        {
            Debug.DrawRay(posRay, transform.TransformDirection(Vector3.down) * hit.distance, Color.yellow);
          
        }
        else
        {
            Debug.DrawRay(posRay, transform.TransformDirection(Vector3.down) * 1000, Color.white);
            insidePlane = false;
        }


        posRay = new Vector3(transform.position.x + 1, transform.position.y, transform.position.z + 1);
        // Does the ray intersect any objects excluding the player layer
        if (Physics.Raycast(posRay, transform.TransformDirection(Vector3.down), out hit, 5f))
        {
            Debug.DrawRay(posRay, transform.TransformDirection(Vector3.down) * hit.distance, Color.yellow);
           
        }
        else
        {
            Debug.DrawRay(posRay, transform.TransformDirection(Vector3.down) * 1000, Color.white);
            insidePlane = false;
        }


        posRay = new Vector3(transform.position.x + 1, transform.position.y, transform.position.z - 1);
        // Does the ray intersect any objects excluding the player layer
        if (Physics.Raycast(posRay, transform.TransformDirection(Vector3.down), out hit, 5f))
        {
            Debug.DrawRay(posRay, transform.TransformDirection(Vector3.down) * hit.distance, Color.yellow);
            
        }
        else
        {
            Debug.DrawRay(posRay, transform.TransformDirection(Vector3.down) * 1000, Color.white);
            insidePlane = false;
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
            destroyThis();
        }
    }

    internal IEnumerator Destroy(float seconds)
    {

        yield return new WaitForSecondsRealtime(seconds);
        Destroy(gameObject);
    }
}
