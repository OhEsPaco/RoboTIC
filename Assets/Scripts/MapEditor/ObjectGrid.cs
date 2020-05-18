using UnityEngine;

public class ObjectGrid : MonoBehaviour
{
    public int x = 1;
    private int y = 1;
    public int z = 1;
    public float blockLength = 1f;
    private Vector3[] points;

    // Start is called before the first frame update
    private void Start()
    {
        points = new Vector3[x * y * z];
        for(int ix=0; ix< x; ix++)
        {
            for (int iy = 0; iy < y; iy++)
            {
                for (int iz = 0; iz < z; iz++)
                {
                    Vector3 newPoint;
                    newPoint.x = transform.position.x + ix * blockLength + (blockLength / 2);
                    newPoint.y = transform.position.y + iy * blockLength + (blockLength / 2);
                    newPoint.z = transform.position.z + iz * blockLength + (blockLength / 2);

                    Set(ix, iy, iz, newPoint);
                }
            }
        }


    }


    public  Vector3 CalculateGridPoint(Vector3 blockPosition)
    {
        int xfactor = (int)((blockPosition.x - transform.position.x) / blockLength);
        int zfactor = (int)((blockPosition.z - transform.position.z) / blockLength);

        Vector3 newPos = new Vector3((transform.position.x + (blockLength / 2f) + xfactor * blockLength), transform.position.y, (transform.position.z + (blockLength / 2f) + zfactor * blockLength));
        return newPos;
    }

   

    private void OnDrawGizmos()
    {
        if (points != null)
        {
            Gizmos.color = Color.yellow;
            foreach(Vector3 p in points)
            {
                Gizmos.DrawSphere(p, 0.1f);
            }
        }
    }
    // Update is called once per frame
    private void Update()
    {
    }

    private bool Get(int bx, int by, int bz, out Vector3 point)
    {
        point = new Vector3();
        if (bx < 0 || bx >= x) return false;
        if (by < 0 || by >= y) return false;
        if (bz < 0 || bz >= z) return false;
        point = points[bx + bz * x + by * (x * z)];
        return true;
    }

    private bool Set(int bx, int by, int bz, in Vector3 point)
    {
        if (bx < 0 || bx >= x) return false;
        if (by < 0 || by >= y) return false;
        if (bz < 0 || bz >= z) return false;
        points[bx + bz * x + by * (x * z)] = point;
        return true;
    }
}