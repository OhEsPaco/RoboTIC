using System.Collections.Generic;

public class MsgRenderMapAndItems
{
    private List<int> mapAndItems;
    private List<int> levelSize;
    private List<int> goal;

    public MsgRenderMapAndItems(List<int> mapAndItems, List<int> levelSize, List<int> goal)
    {
        this.mapAndItems = mapAndItems;
        this.levelSize = levelSize;
        this.goal = goal;
    }

    public List<int> MapAndItems { get => mapAndItems; }
    public List<int> LevelSize { get => levelSize; }
    public List<int> Goal { get => goal; }
}