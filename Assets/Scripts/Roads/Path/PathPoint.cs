// PathPoint.cs
// Francisco Manuel García Sánchez - Belmonte
// 2020

using UnityEngine;

/// <summary>
/// Marca un punto en el camino del robot.
/// </summary>
public class PathPoint : MonoBehaviour
{
    /// <summary>
    /// Color del punto.
    /// </summary>
    [SerializeField] public Color color = Color.yellow;

    /// <summary>
    /// OnDrawGizmos.
    /// </summary>
    private void OnDrawGizmos()
    {
        Gizmos.color = color;
        Gizmos.DrawWireSphere(transform.position, 0.002f);
    }
}