using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static RoadConstants;

public class RoadInput : MonoBehaviour
{
    private Color gizmoColor = Color.yellow;

    public Color GizmoColor { get => gizmoColor; set => gizmoColor = value; }

    [SerializeField] private IOType type = IOType.Generic;
    public IOType Type { get => type; }


    void OnDrawGizmos()
    {

            Gizmos.color = gizmoColor;
        
       
        Gizmos.DrawSphere(transform.position, 0.1f);
    }

    public Road GetParentRoad()
    {
        return transform.parent.GetComponent<Road>();
    }
}
