using UnityEngine;

public class EditorSurfacePoint : MonoBehaviour
{
    private int[] mapPos = new int[3];
    private float blockLength;
    private Transform editorSurface;
    private BoxCollider box;
    private Vector3 surfacePoint;

    public void SetPosition(int x, int z)
    {
        mapPos[0] = x;
        mapPos[1] = 0;
        mapPos[2] = z;
    }

    public void OnSelect()
    {
        EventAggregator.Instance.Publish(new MsgEditorSurfaceTapped(this));
    }

    public void Up()
    {
        if (box != null)
        {
            box.center += new Vector3(0, blockLength / transform.localScale.y, 0);
            surfacePoint += new Vector3(0, blockLength, 0);
        }
        // transform.position += new Vector3(0, blockLength, 0);
        //return transform.position;
    }

    public void Down()
    {
        if (box != null)
        {
            box.center -= new Vector3(0, blockLength / transform.localScale.y, 0);
            surfacePoint -= new Vector3(0, blockLength, 0);
        }
        //transform.position -= new Vector3(0, blockLength, 0);
        //return transform.position;
    }

    public void ResetBox()
    {
        if (box != null)
        {
            box.center = new Vector3(0, 0, 0);
        }
    }

    public int SurfacePointPositionX
    {
        get
        {
            return mapPos[0];
        }
    }

    public int SurfacePointPositionZ
    {
        get
        {
            return mapPos[2];
        }
    }

    public float BlockLength { get => blockLength; set => blockLength = value; }
    public Transform EditorSurface { get => editorSurface; set => editorSurface = value; }
    public Vector3 SurfacePoint { get => surfacePoint; set => surfacePoint = value; }

    private void Start()
    {
        box = GetComponent<BoxCollider>();
        surfacePoint = transform.position;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(transform.position, 0.01f);
    }
}