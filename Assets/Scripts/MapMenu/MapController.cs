using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class MapController : MonoBehaviour
{
    [SerializeField] private Transform center;
    [SerializeField] private GameObject arrowR;
    [SerializeField] private GameObject arrowL;
    [SerializeField] private GameObject levelButtons;
    [SerializeField] private GameObject roadScaler;
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(MapControllerCenter, 0.03f);
    }


    public void EnableGameControls()
    {
        levelButtons.SetActive(true);
        roadScaler.SetActive(true);
        arrowR.SetActive(false);
        arrowL.SetActive(false);
    }


    public void EnableMenuControls()
    {
        arrowR.SetActive(true);
        arrowL.SetActive(true);
        levelButtons.SetActive(false);
        roadScaler.SetActive(false);
    }

    public Vector3 MapControllerCenter
    {
        get
        {
            if (center != null)
            {
                return center.position;
            }
            else
            {
                return transform.position;
            }
        }
    }
}