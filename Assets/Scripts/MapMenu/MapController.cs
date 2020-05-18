using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class MapController : MonoBehaviour
{
    // Start is called before the first frame update
    private void Start()
    {
    }

    // Update is called once per frame
    private void Update()
    {
    }

    private void OnDrawGizmos()
    {
        BoxCollider col = GetComponent<BoxCollider>();
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(transform.position + col.center, 0.03f);
    }

    public Vector3 MapControllerCenter
    {
        get
        {
            BoxCollider col = GetComponent<BoxCollider>();
            if (col != null)
            {
                return transform.position + col.center;
            }
            else
            {
                return transform.position;
            }
        }
    }
}