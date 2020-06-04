using System.Collections.Generic;
using UnityEngine;
using static Block;

public class MsgUseItem
{
    public Block frontBlock;
    public EffectReaction reaction;
    public LevelObject replaceBlock;
    public Vector3 itemPos;
    public Item item;
    public Stack<Item> inventory;

    public MsgUseItem(Block frontBlock, EffectReaction reaction, LevelObject replaceBlock, Vector3 itemPos, Item item, Stack<Item> inventory)
    {
        this.frontBlock = frontBlock;
        this.reaction = reaction;
        this.replaceBlock = replaceBlock;
        this.item = item;
        this.itemPos = itemPos;
        this.inventory = new Stack<Item>();
        //Ya que las llamadas no son sincronas es importante llevar una copia del inventario y no una referencia
        Stack<Item> auxStack = new Stack<Item>();
        foreach (Item i in inventory)
        {
            auxStack.Push(i);
        }

        foreach (Item i in auxStack)
        {
            this.inventory.Push(i);
        }
        auxStack = null;
    }
}