using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoadInput : MonoBehaviour
{
    private Color gizmoColor = Color.yellow;

    public Color GizmoColor { get => gizmoColor; set => gizmoColor = value; }

    void OnDrawGizmos()
    {

            Gizmos.color = gizmoColor;
        
       
        Gizmos.DrawSphere(transform.position, 0.1f);
    }
}
