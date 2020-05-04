public class MsgStartRoadMovement
{
    public RoadInput input;
    public RoadOutput output;

    public MsgStartRoadMovement(RoadInput input, RoadOutput output)
    {
        this.input = input;
        this.output = output;
    }
}