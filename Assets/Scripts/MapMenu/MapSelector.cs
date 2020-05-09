using System.Collections;
using UnityEngine;

public class MapSelector : MonoBehaviour
{
    // Start is called before the first frame update
    private GameObject selectedObject;

    private Vector3 originalScale;
    private Vector3 bigScale;

    [Range(0.1f, 1000f)]
    [SerializeField] private float speed;

    [Range(1f, 1000f)]
    [SerializeField] private float scale;

    public GameObject SelectedObject
    {
        get { return selectedObject; }
        set
        {
            StopAllCoroutines();
            selectedObject = value;
            originalScale = selectedObject.transform.localScale;
            bigScale = originalScale * scale;
        }
    }

    private void Start()
    {
    }

    private void OnMouseEnter()
    {
        
        if (selectedObject != null)
        {
            StopAllCoroutines();
            StartCoroutine(Maximize());
        }
    }

    private void OnMouseExit()
    {
        if (selectedObject != null)
        {
            StopAllCoroutines();
            StartCoroutine(Minimize());
        }
    }

    private IEnumerator Maximize()
    {
        selectedObject.transform.localScale = originalScale;
        
        float distance = Vector3.Distance(originalScale, bigScale);
        for (float i = 0; i <= 1;)
        {
            i += ((speed * Time.deltaTime) / distance);
            selectedObject.transform.localScale = Vector3.Lerp(originalScale, bigScale, i);
            yield return null;
        }
        selectedObject.transform.localScale = bigScale;
    }

    private IEnumerator Minimize()
    {
        selectedObject.transform.localScale = bigScale;

        float distance = Vector3.Distance(originalScale, bigScale);
        for (float i = 0; i <= 1;)
        {
            i += ((speed * Time.deltaTime) / distance);
            selectedObject.transform.localScale = Vector3.Lerp(bigScale, originalScale, i);
            yield return null;
        }
        selectedObject.transform.localScale = originalScale;
    }

}