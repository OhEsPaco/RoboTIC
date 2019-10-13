using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockArrayLinear : IBlockArray
{
    private Block[] blocks;
    private  int width;
    private  int height;
    private  int depth;
    private  int widthTimesDepth;

    public BlockArrayLinear(Block[] blocks, int width, int height, int depth)
    {
        this.blocks = blocks;
        this.width = width;
        this.height = height;
        this.depth = depth;
        this.widthTimesDepth = width * depth;

    }

    public BlockArrayLinear(int width, int height, int depth)
    {
        this.blocks = new Block[width * height * depth];
        for (int i = 0; i < blocks.GetLength(0); i++)
        {
            blocks[i] = null;
        }
 
        this.width = width;
        this.height = height;
        this.depth = depth;
        this.widthTimesDepth = width * depth;
    }
    public Block Get(int x, int y, int z)
    {
        if (x < 0 || x >= width) return null;
        if (y < 0 || y >= height) return null;
        if (z < 0 || z >= depth) return null;
        return blocks[x + z * width + y * widthTimesDepth];
    }

    public int GetDepth()
    {
        return depth;
    }

    public int GetHeight()
    {
        return height;
    }

    public int GetWidth()
    {
        return width;
    }



    public int Lowest(int x, int z)
    {
        int y = 0;
        for (; y < height; y++)
        {
            if (Get(x, y, z) == null)
            {
                break;
            }
        }
        return y;
    }

    public void Set(int x, int y, int z, Block block)
    {
        if (x < 0 || x >= width) return;
        if (y < 0 || y >= height) return;
        if (z < 0 || z >= depth) return;

        blocks[x + z * width + y * widthTimesDepth] = block;
    }
}
