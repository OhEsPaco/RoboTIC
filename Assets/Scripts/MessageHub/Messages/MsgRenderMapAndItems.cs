using System.Collections.Generic;
using UnityEngine;

public class MsgRenderMapAndItems
{
    private List<int> mapAndItems;
    private List<int> levelSize;
    private List<int> goal;
    private GameObject mapParent;

    public MsgRenderMapAndItems(List<int> mapAndItems, List<int> levelSize, List<int> goal, GameObject mapParent)
    {
        this.mapAndItems = mapAndItems;
        this.levelSize = levelSize;
        this.goal = goal;
        this.mapParent = mapParent;
    }

    public List<int> MapAndItems { get => mapAndItems; }
    public List<int> LevelSize { get => levelSize; }
    public List<int> Goal { get => goal; }
    public GameObject MapParent { get => mapParent; }
}