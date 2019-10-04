using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AABB2D
{
    private float  minX;
    private float minY;
    private float maxX;
    private float maxY;

    public float MinX
    {
        get { return minX; }
        set { minX = value; }
    }

    public float MinY
    {
        get { return minY; }
        set { minY = value; }
    }

    public float MaxX
    {
        get { return maxX; }
        set { maxX = value; }
    }

    public float MaxY
    {
        get { return maxY; }
        set { maxY = value; }
    }


    public AABB2D(float minX, float minY, float maxX,
            float maxY)
    {
        this.minX = minX;
        this.minY = minY;
        this.maxX = maxX;
        this.maxY = maxY;

    }

    public bool isCompletelyInside(AABB2D other)
    {
        return minX <= other.MinX && other.MaxX <= maxX && minY <= other.MinY
                && other.MaxY <= maxY;
    }

    public bool intersect(float minX, float minY,
                             float maxX, float maxY)
    {
        return this.MinX < maxX && this.MaxX > minX && this.MinY < maxY
                && this.MaxY > minY;
    }

    public bool intersect(AABB2D other)
    {
        return intersect(other.MinX, other.MinY, other.MaxX, other.MaxY);
    }

}
