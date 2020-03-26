using UnityEngine;

public class RoadOutput : RoadIO
{
    public RoadInput RoadInput { get; set; }

    [SerializeField] private Color color = UnityEngine.Color.red;

    public override Color Color()
    {
        return color;
    }
}