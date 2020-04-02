using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static PathContainer;
public class ConnectorVertical : Road
{
    [SerializeField] private RoadInput rInput;

    public override void ExecuteAction(in string[] args)
    {

    }

    public override bool GetPathAndOutput(in RoadInput input, out Path path, out RoadOutput output)
    {
        if (input == rInput)
        {
            GetPathByName("Path", out path);
            output = path.ioEnd;
            return true;
        }

        path = new Path();
        output = null;
        return false;
    }
}
