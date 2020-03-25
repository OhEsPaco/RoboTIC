using UnityEngine;

public abstract class Road : MonoBehaviour
{
    public abstract RoadOutput GetNextOutput(RoadInput roadInput);
}