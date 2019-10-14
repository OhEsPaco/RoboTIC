using UnityEngine;

public class BlockButton : MonoBehaviour
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
    public string type;
    /// <summary>
    /// Defines the blockToSpawn
    /// </summary>
    public GameObject blockToSpawn;
    private PlaygroundGrid grid;
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
        grid = PlaygroundGrid.instance;
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
        if (spawnedBlock != null)
        {
            Vector3 mousePos = GetMouseWorldPos() + mOffset;

            spawnedBlock.transform.position = grid.SnapToGrid(mousePos);
            //spawnedBlock.GetComponent<NewBlock>().UpdatePos(mousePos.x, mousePos.z);
        }
    }

    /// <summary>
    /// The OnMouseUp
    /// </summary>
    internal void OnMouseUp()
    {
        //spawnedBlock.GetComponent<NewBlock>().addBlock();
        int[] pos = grid.AddBlock(spawnedBlock, type);
        if (pos == null)
        {
            spawnedBlock.GetComponent<EditorBlock>().DestroyBlock();
            // Destroy(spawnedBlock);
        }
        else
        {
            spawnedBlock.GetComponent<EditorBlock>().setPos(pos[0], pos[1], pos[2]);
        }
            spawnedBlock = null;
    }

    /// <summary>
    /// The OnMouseDown
    /// </summary>
    internal void OnMouseDown()
    {
        if (spawnedBlock == null)
        {
            spawnedBlock = Instantiate(blockToSpawn, new Vector3(gameObject.transform.position.x, 1, gameObject.transform.position.z), blockToSpawn.transform.rotation);
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