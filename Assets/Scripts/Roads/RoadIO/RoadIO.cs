using UnityEditor;
using UnityEngine;

public abstract class RoadIO : MonoBehaviour
{
    public enum IODirection { Forward = 0, Back = 1, Left = 2, Right = 3, Undefined };

    public abstract Color Color();

    [SerializeField] private IODirection pointsTo = IODirection.Forward;
    public IODirection Direction { get => pointsTo; }

    public static IODirection GetOppositeDirection(IODirection direction)
    {
        switch (direction)
        {
            case IODirection.Forward:
                return IODirection.Back;

            case IODirection.Back:
                return IODirection.Forward;

            case IODirection.Left:
                return IODirection.Right;

            case IODirection.Right:
                return IODirection.Left;

            default:
                return IODirection.Undefined;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color();
        Gizmos.DrawSphere(transform.position, 0.1f);

        switch (pointsTo)
        {
            case IODirection.Forward:
                DrawArrow.ForGizmo(transform.position, Vector3.forward);
                break;

            case IODirection.Back:
                DrawArrow.ForGizmo(transform.position, Vector3.back);
                break;

            case IODirection.Left:
                DrawArrow.ForGizmo(transform.position, Vector3.left);
                break;

            case IODirection.Right:
                DrawArrow.ForGizmo(transform.position, Vector3.right);
                break;
        }
        Handles.Label(transform.position, gameObject.name);
    }
}