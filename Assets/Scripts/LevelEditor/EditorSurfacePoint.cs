using UnityEngine;

public class EditorSurfacePoint : MonoBehaviour
{
    private int[] mapPos = new int[3];
    private float blockLength;
    public Material mA;
    public Material mB;

    public void SetPosition(int x, int z)
    {
        mapPos[0] = x;
        mapPos[1] = 0;
        mapPos[2] = z;
    }

    public void OnSelect()
    {
        transform.position += new Vector3(0, blockLength, 0);
        Renderer r = GetComponent<Renderer>();

        r.material = mB;
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

    public void GenerateBoxCollider()
    {
        BoxCollider box = gameObject.AddComponent<BoxCollider>();
        box.size = new Vector3(blockLength, blockLength, blockLength);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(transform.position, 0.01f);
    }
}