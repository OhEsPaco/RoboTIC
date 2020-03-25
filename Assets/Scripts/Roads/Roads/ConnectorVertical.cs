using UnityEngine;

public class ConnectorVertical : Road
{
    [SerializeField] private RoadInput input;
    [SerializeField] private RoadOutput output;

    public override RoadOutput GetNextOutput(RoadInput roadInput)
    {
        if (roadInput == input)
        {
            return output;
        }

        return null;
    }
}