using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditorBlock : MonoBehaviour
{
    public BlockExploder exploder;
    private LevelWatcher lwatcher;
    private int x;
    private int y;
    private int z;


    /// <summary>
    /// Defines the mZCoord
    /// </summary>
    private float mZCoord;

    // Start is called before the first frame update
    private void Start()
    {
        lwatcher = LevelWatcher.instance;
    }

    // Update is called once per frame
    private void Update()
    {
    }

    internal void OnMouseDrag()
    {
        
    }

    internal void OnMouseUp()
    {
    }

    internal void OnMouseDown()
    {
        
    }
    public void setPos(int x,int y, int z)
    {
        this.x = x;
        this.y = y;
        this.z = z;

    }


    internal void OnMouseOver()
    {
        if(Input.GetMouseButtonDown(1)){
            
            lwatcher.DeleteBlock(x, y, z);
            DestroyBlock();
        }
    }

    public void DestroyBlock()
    {
        float seconds = exploder.particleDuration + 0.1f;
        transform.localScale = new Vector3(0, 0, 0);
        exploder.explode();
        StartCoroutine(DestroyAfterSeconds(seconds));
    }

    private IEnumerator DestroyAfterSeconds(float seconds)
    {

        yield return new WaitForSecondsRealtime(seconds);
        Destroy(this);
    }
}