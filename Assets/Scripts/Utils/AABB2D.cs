public class AABB2D
{
    /// <summary>
    /// Gets or sets the MinX
    /// </summary>
    public float MinX { get; set; }

    /// <summary>
    /// Gets or sets the MinY
    /// </summary>
    public float MinY { get; set; }

    /// <summary>
    /// Gets or sets the MaxX
    /// </summary>
    public float MaxX { get; set; }

    /// <summary>
    /// Gets or sets the MaxY
    /// </summary>
    public float MaxY { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="AABB2D"/> class.
    /// AABB stands for Axis-Aligned Bounding Box.
    /// </summary>
    /// <param name="minX">The minX<see cref="float"/></param>
    /// <param name="minY">The minY<see cref="float"/></param>
    /// <param name="maxX">The maxX<see cref="float"/></param>
    /// <param name="maxY">The maxY<see cref="float"/></param>
    public AABB2D(float minX, float minY, float maxX,
            float maxY)
    {
        MinX = minX;
        MinY = minY;
        MaxX = maxX;
        MaxY = maxY;
    }

    /// <summary>
    /// Checks if the other AABB2D is completely inside of this AABB2D.
    /// </summary>
    /// <param name="other">The other<see cref="AABB2D"/></param>
    /// <returns>The <see cref="bool"/></returns>
    public bool isCompletelyInside(AABB2D other)
    {
        return MinX <= other.MinX && other.MaxX <= MaxX && MinY <= other.MinY
                && other.MaxY <= MaxY;
    }

    /// <summary>
    /// Checks if the other AABB2D intersects this AABB2D.
    /// </summary>
    /// <param name="minX">The minX<see cref="float"/></param>
    /// <param name="minY">The minY<see cref="float"/></param>
    /// <param name="maxX">The maxX<see cref="float"/></param>
    /// <param name="maxY">The maxY<see cref="float"/></param>
    /// <returns>The <see cref="bool"/></returns>
    public bool intersect(float minX, float minY,
                             float maxX, float maxY)
    {
        return this.MinX < maxX && this.MaxX > minX && this.MinY < maxY
                && this.MaxY > minY;
    }

    /// <summary>
    /// Checks if the other AABB2D intersects this AABB2D.
    /// </summary>
    /// <param name="other">The other<see cref="AABB2D"/></param>
    /// <returns>The <see cref="bool"/></returns>
    public bool intersect(AABB2D other)
    {
        return intersect(other.MinX, other.MinY, other.MaxX, other.MaxY);
    }
}