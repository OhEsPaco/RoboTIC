using UnityEngine;

public class RoadInput : MonoBehaviour
{
    public RoadOutput RoadOutput { get; set; }

    void OnDrawGizmos()
    {
        
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(transform.position, 0.2f);
    }
}