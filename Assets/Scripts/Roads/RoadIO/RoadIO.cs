using UnityEditor;
using UnityEngine;

public abstract class RoadIO : MonoBehaviour
{
    public enum IODirection { Forward = 0, Back = 1, Left = 2, Right = 3, Undefined };

    public abstract Color Color();

    [SerializeField] private IODirection pointsTo = IODirection.Forward;
    public IODirection Direction { get => pointsTo; }
    public RoadIO connectedTo;

    [UniqueIdentifier,SerializeField] private string id;

    public string IOIdentifier
    {
        get
        {
            return id;
        }
    }

    public RoadIO ConnectedTo
    {
        get
        {
            return connectedTo;
        }

        set
        {
            connectedTo = value;
            value.connectedTo = this;
        }
    }

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
        //  Handles.Label(transform.position, gameObject.name);
        Handles.Label(transform.position, id);
    }

    public void MoveRoadTo(in Vector3 newPos)
    {
        transform.parent.position = MoveRoadFromPoint(transform.position, newPos, transform.parent.position);
    }

    private Vector3 MoveRoadFromPoint(in Vector3 point, in Vector3 newPositionOfPoint, in Vector3 roadPosition)
    {
        /*
         * Los struct (por ejemplo Vector3) se pasan siempre por valor, es decir que se copian.
         * No es necesario hacer "new" en los struct, de hecho es mucho más lento.
         * Para pasar por referencia y ahorrar el tiempo de copiarlos se pueden usar las palabras
         * in, ref y out.
         * ref -> requiere la variable inicializada.
         * out -> no hace falta inicializar la variable.
         * in -> requiere la variable inicializada pero no dejará que se ejecuten cambios sobre ella.
         *       (es decir, point.x=3 dará error).
         */
        Vector3 result;
        result.x = roadPosition.x + (newPositionOfPoint.x - point.x);
        result.y = roadPosition.y + (newPositionOfPoint.y - point.y);
        result.z = roadPosition.z + (newPositionOfPoint.z - point.z);
        return result;
    }

    public Road GetParentRoad()
    {
        return transform.parent.GetComponent<Road>();
    }
}