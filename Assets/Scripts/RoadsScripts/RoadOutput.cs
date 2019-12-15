using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoadOutput : MonoBehaviour
{
    private RoadInput roadInput;

    public RoadInput RoadInput { get => roadInput; set => roadInput = value; }

    void OnDrawGizmos()
    {
        // Draw a yellow sphere at the transform's position
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(transform.position, 0.1f);
    }
}
