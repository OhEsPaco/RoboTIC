using UnityEditor;
using UnityEngine;

public class PathPoint : MonoBehaviour
{
    [SerializeField] public Color color = Color.yellow;

    private void OnDrawGizmos()
    {
        Gizmos.color = color;
        Gizmos.DrawWireSphere(transform.position, 0.002f);

       // Handles.Label(transform.position, gameObject.name);
    }
}