using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Defines the <see cref="LevelWatcher" />
/// </summary>
public class LevelWatcher : MonoBehaviour
{
    /// <summary>
    /// Defines the blocks
    /// </summary>
    private List<Block> blocks;

    // Start is called before the first frame update
    /// <summary>
    /// The Start
    /// </summary>
    internal void Start()
    {
        this.blocks = new List<Block>();
    }

    // Update is called once per frame
    /// <summary>
    /// The Update
    /// </summary>
    internal void Update()
    {
    }

    /// <summary>
    /// The addBlock
    /// </summary>
    /// <param name="position">The position<see cref="Vector3"/></param>
    /// <param name="type">The type<see cref="string"/></param>
    public void addBlock(Vector3 position, string type)
    {
        blocks.Add(new Block(position, type));
        Debug.Log(position + " " + type + "\n");
    }

    /// <summary>
    /// The deleteBlock
    /// </summary>
    /// <param name="position">The position<see cref="Vector3"/></param>
    /// <param name="type">The type<see cref="string"/></param>
    public void deleteBlock(Vector3 position, string type)
    {
    }

    /// <summary>
    /// The printBlocks
    /// </summary>
    public void printBlocks()
    {
        foreach (Block b in blocks)
        {
            Debug.Log(b.Position + " " + b.Type + "\n");
        }
    }


}
