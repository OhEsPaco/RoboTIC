﻿using System.Collections.Generic;

public class MsgRenderBlock
{
    public LevelObject[] objectReferences;
    public List<int> levelSize;
    public int blockToSpawn;
    public int x;
    public int y;
    public int z;

    public MsgRenderBlock(LevelObject[] objectReferences, List<int> levelSize, int blockToSpawn, int x, int y, int z)
    {
        this.objectReferences = objectReferences;
        this.levelSize = levelSize;
        this.blockToSpawn = blockToSpawn;
        this.x = x;
        this.y = y;
        this.z = z;
    }
}