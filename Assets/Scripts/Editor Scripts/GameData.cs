using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData
{
    public int[] levelSize;
    public int[] playerStart;
    public int[] goal;
    public Instruction[] availableInstructions;
    public Item[,,] items;
    public Block[,,] map;
}