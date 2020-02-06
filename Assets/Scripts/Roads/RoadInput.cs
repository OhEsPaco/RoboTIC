using UnityEngine;
using static RoadConstants;

public class RoadInput : RoadIO
{
    public override Color GetColor()
    {
        return Color.green;
    }

    public override InputOutput IsInputOrOutput()
    {
        return InputOutput.Input;
    }
}