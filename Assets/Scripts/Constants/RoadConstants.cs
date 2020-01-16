public abstract class RoadConstants
{
    public enum RoadType { Undetermined, CurveIn, CurveOut, Double, HorizontalIn, HorizontalOut, Vertical, IfIn, IfOut, LoopIn, LoopOut };

    public enum IOType { Generic, Top, Right, Bottom, No, Yes, If };

    public enum InputOutput { Input, Output };

    public enum Actions { Reset, GoToNo, GoToYes, SetCounter, SetButton };

    public enum PointingTo { Forward = 0, Back = 1, Left = 2, Right = 3 };
}