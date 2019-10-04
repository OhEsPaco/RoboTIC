using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Defines the <see cref="CreateNewBlock" />
/// </summary>
public class CreateNewBlock : MonoBehaviour
{
    /// <summary>
    /// Defines the mOffset
    /// </summary>
    private Vector3 mOffset;

    /// <summary>
    /// Defines the mZCoord
    /// </summary>
    private float mZCoord;

    /// <summary>
    /// Defines the origY
    /// </summary>
    private float origY;

    /// <summary>
    /// Defines the blockToSpawn
    /// </summary>
    public GameObject blockToSpawn;

    /// <summary>
    /// Defines the spawnedBlock
    /// </summary>
    private GameObject spawnedBlock;

    /// <summary>
    /// The Start
    /// </summary>
    internal void Start()
    {
        origY = gameObject.transform.position.y;
    }

    // Update is called once per frame
    /// <summary>
    /// The Update
    /// </summary>
    internal void Update()
    {
    }

    /// <summary>
    /// The OnMouseDrag
    /// </summary>
    internal void OnMouseDrag()
    {
            if (spawnedBlock != null){
                Vector3 mousePos = GetMouseWorldPos() + mOffset;
             
                spawnedBlock.transform.position = new Vector3(Mathf.Round(mousePos.x),
                                     1,
                                     Mathf.Round(mousePos.z));
            }
    }

    /// <summary>
    /// The OnMouseUp
    /// </summary>
    internal void OnMouseUp()
    {
        spawnedBlock.GetComponent<NewBlock>().addBlock();
        spawnedBlock = null;
    }

    /// <summary>
    /// The OnMouseDown
    /// </summary>
    internal void OnMouseDown()
    {

            if (spawnedBlock == null)
            {
                spawnedBlock=Instantiate(blockToSpawn,new Vector3(gameObject.transform.position.x, gameObject.transform.position.y, gameObject.transform.position.z), blockToSpawn.transform.rotation);
                spawnedBlock.SetActive(true);
            }

            if (spawnedBlock != null)
            {
                mZCoord = Camera.main.WorldToScreenPoint(spawnedBlock.transform.position).z;
                mOffset = spawnedBlock.transform.position - GetMouseWorldPos();
            }
    }

    /// <summary>
    /// The GetMouseWorldPos
    /// </summary>
    /// <returns>The <see cref="Vector3"/></returns>
    private Vector3 GetMouseWorldPos()
    {
        Vector3 mousePoint = Input.mousePosition;
        mousePoint.z = mZCoord;
        return Camera.main.ScreenToWorldPoint(mousePoint);
    }
}
