using UnityEngine;

public class ConnectorDouble : Road
{
    [SerializeField] private RoadInput inputTop;
    [SerializeField] private RoadOutput outputTop;

    [SerializeField] private RoadInput inputBottom;
    [SerializeField] private RoadOutput outputBottom;

    public override RoadOutput GetNextOutput(RoadInput roadInput)
    {
        if (roadInput == inputTop)
        {
            return outputTop;
        }

        if (roadInput == inputBottom)
        {
            return outputBottom;
        }

        return null;
    }
}