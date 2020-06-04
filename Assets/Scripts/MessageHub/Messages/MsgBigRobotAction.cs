using UnityEngine;

public class MsgBigRobotAction
{
    public enum BigRobotActions
    {
        Jump,
        Move,
        TurnLeft,
        TurnRight,
        Win,
        Lose
    }

    private BigRobotActions action;
    private Vector3 target;

    public BigRobotActions Action { get => action; }
    public Vector3 Target { get => target; }

    public MsgBigRobotAction(BigRobotActions action, Vector3 target)
    {
        this.action = action;
        this.target = target;
    }
}