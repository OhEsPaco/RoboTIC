using UnityEngine;
using static RoadConstants;

public static class RoadMethods
{
    public static Vector3 MoveRoadFromPoint(in Vector3 point, in Vector3 newPositionOfPoint, in Vector3 roadPosition)
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

    public static PointingTo OppositeDirection(in PointingTo direction)
    {
        switch (direction)
        {
            case PointingTo.Forward:
                return PointingTo.Back;

            case PointingTo.Back:
                return PointingTo.Forward;

            case PointingTo.Left:
                return PointingTo.Right;

            case PointingTo.Right:
                return PointingTo.Left;

            default:
                return PointingTo.Forward;
        }
    }
}