using UnityEngine;
using static PathContainer;

public class NodeLoopIn : Road
{
    [SerializeField] private LoopCounter lCounter;
    [SerializeField] private RoadInput inputTop;
    [SerializeField] private RoadInput inputBottom;

    public override void ExecuteAction(in string[] args)
    {
        if (args.Length > 0)
        {
            switch (args[0])
            {
                case "lock":
                    lCounter.Lock();
                    Debug.Log("Counter locked.");
                    break;

                case "unlock":
                    lCounter.Unlock();
                    Debug.Log("Counter unlocked.");
                    break;

                default:
                    Debug.LogWarning("Undefined action: " + args[0]);
                    break;
            }
        }
    }

    public override bool GetPathAndOutput(in RoadInput input, out Path path, out RoadOutput output)
    {
        if (input == inputTop || input == inputBottom)
        {
            if (lCounter.ActualNumber() == 0)
            {
                //Mandarlos a outputno
                if (input == inputTop)
                {
                    GetPathByName("TopToNo", out path);
                    output = path.ioEnd;
                    return true;
                }
                if (input == inputBottom)
                {
                    GetPathByName("BottomToNo", out path);
                    output = path.ioEnd;
                    return true;
                }
            }
            else
            {
                //mandarlos a outputyes
                if (input == inputTop)
                {

                    GetPathByName("TopToYes", out path);
                    lCounter.SetNumber(lCounter.ActualNumber() - 1);
                    output = path.ioEnd;
                    return true;
                }

                if (input == inputBottom)
                {
                    GetPathByName("BottomToYes", out path);
                    lCounter.SetNumber(lCounter.ActualNumber() - 1);
                    output = path.ioEnd;
                    return true;
                }
            }
        }

        path = new Path();
        output = null;
        return false;
    }
}