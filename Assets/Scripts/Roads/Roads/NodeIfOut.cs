using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static PathContainer;
public class NodeIfOut : Road
{
    [SerializeField] private RoadInput inputYes;
    [SerializeField] private RoadInput inputNo;
    public override void ExecuteAction(in string[] args)
    {

    }

    public override bool GetPathAndOutput(in RoadInput input, out Path path, out RoadOutput output)
    {
        if (input == inputYes)
        {
            GetPathByName("Yes", out path);
            output = path.ioEnd;
            return true;
        }

        if (input == inputNo)
        {
            GetPathByName("No", out path);
            output = path.ioEnd;
            return true;
        }

        path = new Path();
        output = null;
        return false;
    }
}
