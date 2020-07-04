// ScrollTexture.cs
// Francisco Manuel García Sánchez - Belmonte
// 2020

using UnityEngine;

/// <summary>
/// Hace scroll continuo a una textura.
/// </summary>
public class ScrollTexture : MonoBehaviour
{
    /// <summary>
    /// Scroll en X.
    /// </summary>
    public float ScrollX = 0.5f;

    /// <summary>
    /// Scroll en Y.
    /// </summary>
    public float ScrollY = 0.5f;

    /// <summary>
    /// Update.
    /// </summary>
    private void Update()
    {
        float OffsetX = Time.time * ScrollX;
        float OffsetY = Time.time * ScrollY;
        GetComponent<Renderer>().material.mainTextureOffset = new Vector2(OffsetX, OffsetY);
    }
}