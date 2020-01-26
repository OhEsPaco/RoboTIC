using UnityEditor;
using UnityEngine;
using static RoadConstants;

public abstract class RoadIO : MonoBehaviour
{
    [SerializeField] private IOType ioType = IOType.Generic;

    public PointingTo PointsTo { get => pointsTo; }
    public IOType IoType { get => ioType; set => ioType = value; }

    [SerializeField] private PointingTo pointsTo = PointingTo.Forward;

    [SerializeField] private RoadIO connectedTo;

    private void OnDrawGizmos()
    {
        Gizmos.color = GetColor();
        Gizmos.DrawSphere(transform.position, 0.1f);

        switch (pointsTo)
        {
            case PointingTo.Forward:
                DrawArrow.ForGizmo(transform.position, Vector3.forward);
                break;

            case PointingTo.Back:
                DrawArrow.ForGizmo(transform.position, Vector3.back);
                break;

            case PointingTo.Left:
                DrawArrow.ForGizmo(transform.position, Vector3.left);
                break;

            case PointingTo.Right:
                DrawArrow.ForGizmo(transform.position, Vector3.right);
                break;
        }
        Handles.Label(transform.position, ioType.ToString("g"));
    }

    public abstract Color GetColor();

    public bool ConnectTo(in RoadIO connectToThisIo)
    {
        if (connectToThisIo.IsInputOrOutput() == IsInputOrOutput())
        {
            Debug.Log("Can't connect a " + IsInputOrOutput().ToString("g") + " to another " + IsInputOrOutput().ToString("g"));
            return false;
        }
        else
        {
            if (connectedTo != connectToThisIo)
            {
                connectedTo = connectToThisIo;
                connectToThisIo.ConnectTo(this);
               
            }
          
            return true;
        }
    }

    public RoadIO ConnectedTo()
    {
        return connectedTo;
    }

    public abstract InputOutput IsInputOrOutput();

    public Road GetParentRoad()
    {
        return transform.parent.GetComponent<Road>();
    }

    public void MoveRoadTo(in Vector3 newPos)
    {
        transform.parent.position = RoadMethods.MoveRoadFromPoint(transform.position, newPos, transform.parent.position);
    }
}