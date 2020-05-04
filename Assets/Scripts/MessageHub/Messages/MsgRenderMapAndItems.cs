using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MsgRenderMapAndItems
{
    private List<int> mapAndItems;
    private List<int> levelSize;

    public MsgRenderMapAndItems(List<int> mapAndItems, List<int> levelSize)
    {
        this.mapAndItems = mapAndItems;
        this.levelSize = levelSize;
    }

    public List<int> MapAndItems { get => mapAndItems; }
    public List<int> LevelSize { get => levelSize; }
}
