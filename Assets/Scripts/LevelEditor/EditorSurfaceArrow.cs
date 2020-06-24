using UnityEngine;

public class EditorSurfaceArrow : MonoBehaviour
{
    private Vector3 mOffset;
    private float mZCoord;
    [SerializeField] private GameObject sphere;

    public void StartPlacingArrow(Vector3 closestSurfacePoint)
    {
        mZCoord = Camera.main.WorldToScreenPoint(gameObject.transform.position).z;
        mOffset = gameObject.transform.position - GetMouseWorldPos();
        sphere.transform.position = closestSurfacePoint;
        sphere.SetActive(true);
    }

    public void UpdateArrowPos(Vector3 closestSurfacePoint)
    {
        transform.position = GetMouseWorldPos() + mOffset;
        sphere.transform.position = closestSurfacePoint;
    }

    private Vector3 GetMouseWorldPos()
    {
        //Coordenadas en pixeles
        Vector3 mousePoint = Input.mousePosition;

        mousePoint.z = mZCoord;

        return Camera.main.ScreenToWorldPoint(mousePoint);
    }
}