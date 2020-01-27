using UnityEngine;

public class RoutePoint : MonoBehaviour
{
    [SerializeField] private GameObject nextPoint;
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Route point trigger triggered.");
        LevelManager.instance.RoadMovementLogic.FinishedAction(nextPoint.transform.position);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(transform.position, 0.1f);
    }
}