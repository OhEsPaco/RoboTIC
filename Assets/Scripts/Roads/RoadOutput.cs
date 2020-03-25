using UnityEngine;

public class RoadOutput : MonoBehaviour
{
    public RoadInput RoadInput { get; set; }

    void OnDrawGizmos()
    {

        Gizmos.color = Color.red;
        Gizmos.DrawSphere(transform.position, 0.2f);
    }
}