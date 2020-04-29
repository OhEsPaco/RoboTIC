using UnityEngine;

public class CamCage : MonoBehaviour
{
    public CagePoint zPlus;
    public CagePoint zMinus;

    public CagePoint xPlus;
    public CagePoint xMinus;

    public CagePoint yPlus;
    public CagePoint yMinus;

    private Vector3 A;
    private Vector3 B;
    private Vector3 C;
    private Vector3 D;
    private Vector3 E;
    private Vector3 F;
    private Vector3 G;
    private Vector3 H;

    // Start is called before the first frame update
    private void Start()
    {
    }

    // Update is called once per frame
    private void Update()
    {
    }

    public Vector3 AdjustTranslation(Vector3 camPosition, Vector3 rotatedTranslation)
    {
        Vector3 output;
        output.x = rotatedTranslation.x;
        output.y = rotatedTranslation.y;
        output.z = rotatedTranslation.z;

        float xn = xMinus.transform.position.x;
        float xp = xPlus.transform.position.x;

        float yn = yMinus.transform.position.y;
        float yp = yPlus.transform.position.y;

        float zn = zMinus.transform.position.z;
        float zp = zPlus.transform.position.z;

        if (camPosition.x + rotatedTranslation.x > xp)
        {
            output.x = xp - camPosition.x;
        }

        if (camPosition.y + rotatedTranslation.y > yp)
        {
            output.y = yp - camPosition.y;
        }

        if (camPosition.z + rotatedTranslation.z > zp)
        {
            output.z = zp - camPosition.z;
        }

        if (camPosition.x + rotatedTranslation.x < xn)
        {
            output.x = xn - camPosition.x;
        }

        if (camPosition.y + rotatedTranslation.y < yn)
        {
            output.y = yn - camPosition.y;
        }

        if (camPosition.z + rotatedTranslation.z < zn)
        {
            output.z = zn - camPosition.z;
        }

        return output;
    }

    private void OnDrawGizmos()
    {
        float xn = xMinus.transform.position.x;
        float xp = xPlus.transform.position.x;

        float yn = yMinus.transform.position.y;
        float yp = yPlus.transform.position.y;

        float zn = zMinus.transform.position.z;
        float zp = zPlus.transform.position.z;

        A.x = xn;
        A.y = yn;
        A.z = zn;

        B.x = xp;
        B.y = yn;
        B.z = zn;

        C.x = xp;
        C.y = yp;
        C.z = zn;

        D.x = xn;
        D.y = yp;
        D.z = zn;

        E.x = xn;
        E.y = yp;
        E.z = zp;

        F.x = xn;
        F.y = yn;
        F.z = zp;

        G.x = xp;
        G.y = yn;
        G.z = zp;

        H.x = xp;
        H.y = yp;
        H.z = zp;

        Gizmos.color = Color.yellow;

        Gizmos.DrawLine(A, B);
        Gizmos.DrawLine(A, D);
        Gizmos.DrawLine(A, F);
        Gizmos.DrawLine(D, E);
        Gizmos.DrawLine(D, C);
        Gizmos.DrawLine(B, C);
        Gizmos.DrawLine(B, G);
        Gizmos.DrawLine(G, H);
        Gizmos.DrawLine(C, H);
        Gizmos.DrawLine(F, G);
        Gizmos.DrawLine(E, H);
        Gizmos.DrawLine(E, F);
    }
}