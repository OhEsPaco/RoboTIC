using UnityEngine;
using static RoadConstants;

public class RoadOutput : RoadIO
{
    public override Color GetColor()
    {
        return Color.red;
    }

    public override InputOutput IsInputOrOutput()
    {
        return InputOutput.Output;
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Output trigger triggered.");
    }
}