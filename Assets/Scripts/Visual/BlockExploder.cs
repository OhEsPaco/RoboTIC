using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Defines the <see cref="Explode" />
/// </summary>
[RequireComponent(typeof(Renderer))]
public class BlockExploder : MonoBehaviour
{
    /// <summary>
    /// Defines the cubeSize
    /// </summary>
    public float cubeSize = 0.2f;

    /// <summary>
    /// Defines the cubesInRow
    /// </summary>
    public int cubesInRow = 5;

    /// <summary>
    /// Defines the particleDuration
    /// </summary>
    public float particleDuration = 1f;

    /// <summary>
    /// Defines the explosionForce
    /// </summary>
    public float explosionForce = 50f;

    /// <summary>
    /// Defines the explosionRadius
    /// </summary>
    public float explosionRadius = 4f;

    /// <summary>
    /// Defines the explosionUpward
    /// </summary>
    public float explosionUpward = 0.4f;

    /// <summary>
    /// Defines the mass
    /// </summary>
    public float mass = 0.1f;

    /// <summary>
    /// Defines the particleMaterial
    /// </summary>
    private Material particleMaterial;

    /// <summary>
    /// Defines the pieces
    /// </summary>
    private List<GameObject> pieces;

    /// <summary>
    /// The Start
    /// </summary>
    internal void Start()
    {
        Renderer r = GetComponent<Renderer>();
        if (r != null)
        {
            Material m = r.material;
            if (m != null)
            {
                particleMaterial = m;
            }
        }

        pieces = new List<GameObject>();
    }

    /// <summary>
    /// The explode
    /// </summary>
    public void Explode()
    {
        transform.position = new Vector3(transform.position.x, -1f, transform.position.z);
        //loop 3 times to create pieces in x,y,z coordinates
        for (int x = 0; x < cubesInRow; x++)
        {
            for (int y = 0; y < cubesInRow; y++)
            {
                for (int z = 0; z < cubesInRow; z++)
                {
                    CreatePiece(x, y, z);
                }
            }
        }
        GetComponent<Renderer>().enabled = false;
        //get explosion position
        Vector3 explosionPos = transform.position;

        //get colliders in that position and radius
        Collider[] colliders = Physics.OverlapSphere(explosionPos, explosionRadius);
        //add explosion force to all colliders in that overlap sphere
        foreach (Collider hit in colliders)
        {
            //get rigidbody from collider object
            Rigidbody rb = hit.GetComponent<Rigidbody>();
            if (rb != null)
            {
                //add explosion force to this body with given parameters
                rb.AddExplosionForce(explosionForce, transform.position, explosionRadius, explosionUpward);
            }
        }

        //make object disappear

        // gameObject.transform.localScale = new Vector3(0, 0, 0);
        StartCoroutine(DestroyParticlesAndGameobject(pieces, particleDuration));
    }

    /// <summary>
    /// The createPiece
    /// </summary>
    /// <param name="x">The x<see cref="int"/></param>
    /// <param name="y">The y<see cref="int"/></param>
    /// <param name="z">The z<see cref="int"/></param>
    private void CreatePiece(int x, int y, int z)
    {
        //create piece
        GameObject piece;

        piece = GameObject.CreatePrimitive(PrimitiveType.Cube);
        pieces.Add(piece);
        piece.GetComponent<Renderer>().material = particleMaterial;
        //set piece position and scale
        Vector3 cubesPivot = new Vector3(0, 1, 0);
        piece.transform.parent = transform;
        piece.transform.position = transform.position + new Vector3(cubeSize * x, cubeSize * y, cubeSize * z) - cubesPivot;
        piece.transform.rotation = transform.rotation;
        piece.transform.localScale = new Vector3(cubeSize, cubeSize, cubeSize);

        //add rigidbody and set mass
        piece.AddComponent<Rigidbody>();
        piece.GetComponent<Rigidbody>().mass = mass;
    }

    //Destroys all the particles when a given number of seconds have passed
    /// <summary>
    /// The DestroyParticles
    /// </summary>
    /// <param name="particles">The particles<see cref="List{GameObject}"/></param>
    /// <param name="seconds">The seconds<see cref="float"/></param>
    /// <returns>The <see cref="IEnumerator"/></returns>
    private IEnumerator DestroyParticlesAndGameobject(List<GameObject> particles, float seconds)
    {
        yield return new WaitForSecondsRealtime(seconds);
        foreach (GameObject g in particles)
        {
            g.SetActive(false);
        }
        yield return null;
        foreach (GameObject g in particles)
        {
            Destroy(g);
            yield return null;
        }

        Destroy(this.gameObject);
    }
}