using UnityEngine;

public class EditorBlock : MonoBehaviour
{
    private Vector3 mOffset;

    /// <summary>
    /// Defines the mZCoord
    /// </summary>
    private float mZCoord;

    // Start is called before the first frame update
    private void Start()
    {
        mZCoord = Camera.main.WorldToScreenPoint(transform.position).z;
        mOffset = transform.position - GetMouseWorldPos();
    }

    // Update is called once per frame
    private void Update()
    {
    }

    internal void OnMouseDrag()
    {
        Vector3 mousePos = GetMouseWorldPos() + mOffset;
        transform.position = mousePos;
    }

    internal void OnMouseUp()
    {
    }

    internal void OnMouseDown()
    {
        
    }

    private Vector3 GetMouseWorldPos()
    {
        Vector3 mousePoint = Input.mousePosition;
        mousePoint.z = mZCoord;
        return Camera.main.ScreenToWorldPoint(mousePoint);
    }
}