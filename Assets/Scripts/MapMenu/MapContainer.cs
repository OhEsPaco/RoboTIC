using System.Collections.Generic;
using UnityEngine;

public class MapContainer : MonoBehaviour
{
    private GameObject mapCenter;
    public Vector3 MapCenter { get => mapCenter.transform.position - transform.position; }

    private int[] mapSize;

    private void Start()
    {
        mapCenter = new GameObject();
        mapCenter.transform.parent = transform;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(transform.position + MapCenter, 0.01f);
    }

    public void MoveMapTo(in Vector3 mapCenterPos, float surfaceY, float blockLength)
    {
        Vector3 newPos;
        newPos.x = mapCenterPos.x - MapCenter.x;
        newPos.y = surfaceY + blockLength / 2;
        newPos.z = mapCenterPos.z - MapCenter.z;
        transform.position = newPos;
    }

    public void UpdateMapCenter(List<int> mapSize, float blockLength)
    {
        Vector3 mapCenter;
        mapCenter.x = ((mapSize[0] - 1) * blockLength) / 2;
        mapCenter.y = 0;
        mapCenter.z = ((mapSize[2] - 1) * blockLength) / 2;
        mapCenter += transform.position;

        this.mapCenter.transform.position = mapCenter;
    }
}