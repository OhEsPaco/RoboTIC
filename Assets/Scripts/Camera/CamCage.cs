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

    public void AdjustTranslation(Vector3 camPosition, Vector3 rotatedTranslation, out Vector3 output, out Vector3 correctedPos, out bool x, out bool y, out bool z) 
    {
        
        output.x = rotatedTranslation.x;
        output.y = rotatedTranslation.y;
        output.z = rotatedTranslation.z;

        correctedPos.x = camPosition.x;
        correctedPos.y = camPosition.y;
        correctedPos.z = camPosition.z;

        float xn = xMinus.transform.position.x;
        float xp = xPlus.transform.position.x;

        float yn = yMinus.transform.position.y;
        float yp = yPlus.transform.position.y;

        float zn = zMinus.transform.position.z;
        float zp = zPlus.transform.position.z;

        x = false;
        y = false;
        z = false;

        if (camPosition.x > xp && rotatedTranslation.x > 0)
        {
            output.x = 0;
            correctedPos.x = xp;
            x = true;
        }

        if (camPosition.x < xn && rotatedTranslation.x < 0)
        {
            output.x = 0;
            correctedPos.x = xn;
            x = true;
        }

        if (camPosition.y > yp && rotatedTranslation.y > 0)
        {
            output.y = 0;
            correctedPos.y = yp;
            y = true;
        }

        if (camPosition.y < yn && rotatedTranslation.y < 0)
        {
            output.y = 0;
            correctedPos.y = yn;
            y = true;
        }

        if (camPosition.z > zp && rotatedTranslation.z > 0)
        {
            output.z = 0;
            correctedPos.z = zp;
            z = true;
        }

        if (camPosition.z < zn && rotatedTranslation.z < 0)
        {
            output.z = 0;
            correctedPos.z = zn;
            z = true;
        }

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