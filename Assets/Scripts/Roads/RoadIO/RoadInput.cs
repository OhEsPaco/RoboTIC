using UnityEngine;

public class RoadInput : RoadIO
{
    public RoadOutput RoadOutput { get; set; }

    [SerializeField] private Color color = UnityEngine.Color.green;

    public override Color Color()
    {
        return color;
    }
}