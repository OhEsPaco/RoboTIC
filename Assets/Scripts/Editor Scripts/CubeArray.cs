using UnityEngine;

public class CubeArray
{
    private int width;
    private int height;
    private int depth;
    private Block[,,] voxels;
    public int Lowest(int x, int z)
    {
        int y = 0;
        for (; y < height; y++)
        {
            if (get(x, y, z) == null)
            {
                break;
            }
        }
       // Debug.Log(y);
        return y;
    }

    public CubeArray(Block[,,] voxels)
    {
        this.voxels = voxels;
        width = voxels.GetLength(0);
        height = voxels.GetLength(1);
        depth = voxels.GetLength(2);
    }

    public CubeArray(int width, int height, int depth)
    {
        this.voxels = new Block[height,width,depth];

        for (int x = 0; x < voxels.GetLength(0); x += 1)
        {
            for (int y = 0; y < voxels.GetLength(1); y += 1)
            {
                for (int z = 0; z < voxels.GetLength(2); z += 1)
                {
                    voxels[x, y, z] = null;
                }
            }
        }


        this.width = width;
        this.height = height;
        this.depth = depth;
        
    }


    public Block get(int x, int y, int z)
    {
        if (x < 0 || x >= width) return null;
        if (y < 0 || y >= height) return null;
        if (z < 0 || z >= depth) return null;
        return voxels[y,x,z];
    }



    public void set(int x, int y, int z, Block voxel)
    {
        if (x < 0 || x >= width) return;
        if (y < 0 || y >= height) return;
        if (z < 0 || z >= depth) return;

        voxels[y,x,z] = voxel;
    }


    public int getWidth()
    {
        return width;
    }

    public int getHeight()
    {
        return height;
    }



    public int getDepth()
    {
        return depth;
    }
}