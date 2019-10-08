using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class Item
{
    public string Type { get; set; }

    public Item(string type)
    {
        this.Type = type;
    }
}