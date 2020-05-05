using UnityEngine;
using static Block;

public class MsgUseItem
{
    public Block frontBlock;
    public EffectReaction reaction;
    public LevelObject replaceBlock;
    public Vector3 itemPos;
    public Item item;

    public MsgUseItem(Block frontBlock, EffectReaction reaction, LevelObject replaceBlock, Vector3 itemPos, Item item)
    {
        this.frontBlock = frontBlock;
        this.reaction = reaction;
        this.replaceBlock = replaceBlock;
        this.item = item;
        this.itemPos = itemPos;
    }
}