using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Defines the <see cref="LevelWatcher" />
/// </summary>

public class LevelWatcher : MonoBehaviour
{
    public static readonly int OK = 0;
    public static readonly int OCCUPIED_SPACE = 1;
    public int maxBlocksX = 10;
    public int maxBlocksY = 4;
    public int maxBlocksZ = 10;
    private CubeArray cubes;

    // Start is called before the first frame update
    /// <summary>
    /// The Start
    /// </summary>
    internal void Start()
    {
        this.cubes = new CubeArray(maxBlocksX, maxBlocksY, maxBlocksZ);
    }

    // Update is called once per frame
    /// <summary>
    /// The Update
    /// </summary>
    internal void Update()
    {
    }

    public int Lowest(int x, int z)
    {
        return cubes.Lowest(x, z);
    }
    public bool isOccupied(int x, int y, int z)
    {
        Block tmp = cubes.get(x, y, z);

        if (tmp == null)
        {
            return false;
        }
        else
        {
            //   Debug.Log("OCUPPIED");
            return true;
        }
    }
    public int AddBlock(int x, int y, int z, Block block)
    {
        Block tmp = cubes.get(x, y, z);
        Debug.Log(x + " " + y + " " + z);
        if (tmp == null)
        {
            Debug.Log("OK");
            cubes.set(x, y, z, block);
            return OK;
        }
        else
        {
            Debug.Log("OCUPPIED");
            return OCCUPIED_SPACE;
        }
    }

    /// <summary>
    /// The deleteBlock
    /// </summary>
    /// <param name="position">The position<see cref="Vector3"/></param>
    /// <param name="type">The type<see cref="string"/></param>
    public void deleteBlock(int x, int y, int z)
    {
        cubes.set(x, y, z, null);
    }
}