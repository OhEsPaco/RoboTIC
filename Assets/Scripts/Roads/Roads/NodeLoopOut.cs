using UnityEngine;
using static PathContainer;

public class NodeLoopOut : Road
{
    [SerializeField] private RoadInput inputTop;
    [SerializeField] private RoadInput inputYes;

    public override void ExecuteAction(in string[] args)
    {
    }

    public override bool GetPathAndOutput(in RoadInput input, out Path path, out RoadOutput output)
    {
        if (input == inputTop)
        {
            GetPathByName("TopToBottom", out path);

            output = path.ioEnd;
            return true;
        }

        if (input == inputYes)
        {
            GetPathByName("YesToTop", out path);

            output = path.ioEnd;
            return true;
        }

        path = new Path();
        output = null;
        return false;
    }
}