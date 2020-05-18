using UnityEngine;

public class MapContainer : MonoBehaviour
{
    private Vector3 mapCenter;

    public Vector3 MapCenter { get => mapCenter; }

    // Start is called before the first frame update
    private void Start()
    {
    }

    // Update is called once per frame
    private void Update()
    {
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(transform.position + mapCenter, 0.03f);
    }

    public void MoveMapTo(in Vector3 newPos)
    {
        transform.position = MoveRoadFromPoint(transform.position + mapCenter, newPos, transform.position);
    }

    public void UpdateMapCenter()
    {
        Vector3 mapcenter = new Vector3(0, 0, 0);
        int count = 0;
        foreach (Transform child in transform)
        {
            mapcenter += child.position;
            count++;
        }
        if (count != 0)
        {
            mapcenter = mapcenter / count;
            mapCenter = mapcenter - transform.position;
        }
        else
        {
            mapCenter = new Vector3(0, 0, 0);
        }
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
}