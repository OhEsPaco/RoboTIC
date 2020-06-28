using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class MapController : MonoBehaviour
{
    [SerializeField] private Transform center;
    [SerializeField] private GameObject arrowR;
    [SerializeField] private GameObject arrowL;
    [SerializeField] private GameObject levelButtons;
    [SerializeField] private GameObject roadScaler;
    [SerializeField] private GameObject mapBounds;
    [SerializeField] private GameObject editorButtons;
    [SerializeField] private GameObject editorSurface;
    [SerializeField] private GameObject mapPlacer;
    [SerializeField] private bool drawCollidersGizmo;
    /* private Vector3 arrowRPos;
     private Vector3 arrowLPos;
     private Vector3 levelButtonsPos;
     private Vector3 roadScalerPos;*/

    private void Start()
    {
        /*arrowLPos = arrowL.transform.localPosition;
        arrowRPos = arrowR.transform.localPosition;
        levelButtonsPos = levelButtons.transform.localPosition;
        roadScalerPos = roadScaler.transform.localPosition;*/
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        if (drawCollidersGizmo)
        {
            BoxCollider r = GetComponent<BoxCollider>();
            if (r != null)
            {
                Gizmos.DrawWireCube(r.bounds.center, r.bounds.size);
            }

            BoxCollider[] childColliders = GetComponentsInChildren<BoxCollider>();
            foreach (BoxCollider box in childColliders)
            {
                Gizmos.DrawWireCube(box.bounds.center, box.bounds.size);
            }
        }

        Gizmos.DrawSphere(MapControllerCenter, 0.03f);
    }

   
    public void EnableGameControls()
    {
        /*levelButtons.transform.localPosition = levelButtonsPos;
        roadScaler.transform.localPosition = roadScalerPos;
        arrowR.transform.position = new Vector3(10000, 10000, 10000);
        arrowL.transform.position = new Vector3(10000, 10000, 10000);*/
        levelButtons.gameObject.SetActive(true);
        roadScaler.gameObject.SetActive(true);
        mapBounds.gameObject.SetActive(false);
        arrowR.gameObject.SetActive(false);
        arrowL.gameObject.SetActive(false);
        editorButtons.gameObject.SetActive(false);
        editorSurface.gameObject.SetActive(false);
        mapPlacer.gameObject.SetActive(true);
    }

    public void EnableMenuControls()
    {
        /*arrowR.transform.localPosition = arrowRPos;
        arrowL.transform.localPosition = arrowLPos;
        levelButtons.transform.position = new Vector3(10000, 10000, 10000);
        roadScaler.transform.position = new Vector3(10000, 10000, 10000);*/
        levelButtons.gameObject.SetActive(false);
        roadScaler.gameObject.SetActive(false);
        mapBounds.gameObject.SetActive(true);
        arrowR.gameObject.SetActive(true);
        arrowL.gameObject.SetActive(true);
        editorButtons.gameObject.SetActive(false);
        editorSurface.gameObject.SetActive(false);
        mapPlacer.gameObject.SetActive(true);
    }

    public void EnableMainMenuControls()
    {
        levelButtons.gameObject.SetActive(false);
        roadScaler.gameObject.SetActive(false);
        mapBounds.gameObject.SetActive(false);
        arrowR.gameObject.SetActive(false);
        arrowL.gameObject.SetActive(false);
        editorButtons.gameObject.SetActive(false);
        editorSurface.gameObject.SetActive(false);
        mapPlacer.gameObject.SetActive(false);
    }

    public void EnableEditorControls()
    {
        editorButtons.gameObject.SetActive(true);
        editorSurface.gameObject.SetActive(true);
        levelButtons.gameObject.SetActive(false);
        roadScaler.gameObject.SetActive(false);
        mapBounds.gameObject.SetActive(false);
        arrowR.gameObject.SetActive(false);
        arrowL.gameObject.SetActive(false);
        mapPlacer.gameObject.SetActive(true);
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