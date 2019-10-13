using UnityEngine;

public class TextureScroller : MonoBehaviour
{
    public float ScrollX = 0.5f;
    public float ScrollY = 0.5f;
    public Material materialToOffset;

    private void Update()
    {
        float OffsetX = Time.time * ScrollX;
        float OffsetY = Time.time * ScrollY;
        materialToOffset.mainTextureOffset = new Vector2(OffsetX, OffsetY);
    }
}