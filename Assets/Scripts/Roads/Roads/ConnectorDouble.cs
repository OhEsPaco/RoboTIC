using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static PathContainer;
public class ConnectorDouble : Road
{
    [SerializeField] private RoadInput inputB;
    [SerializeField] private RoadInput inputA;
    public override void ExecuteAction(in string[] args)
    {

    }

    public override bool GetPathAndOutput(in RoadInput input, out Path path, out RoadOutput output)
    {

        if (input == inputA)
        {
            GetPathByName("A", out path);
            output = path.ioEnd;
            return true;
        }

        if (input == inputB)
        {
            GetPathByName("B", out path);
            output = path.ioEnd;
            return true;
        }


        path = new Path();
        output = null;
        return false;
    }
}
