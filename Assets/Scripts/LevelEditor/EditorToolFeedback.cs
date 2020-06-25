using System.Collections;
using UnityEngine;

[RequireComponent(typeof(EditorTool))]
public class EditorToolFeedback : MonoBehaviour
{
    private Vector3 originalScale;
    private bool ready = true;
    [SerializeField] private float speed = 1f;

    [Range(0.001f, 0.999f)]
    [SerializeField] private float scalePercent = 0.5f;

    private void OnEnable()
    {
        transform.localScale = originalScale;
    }

    private void OnDisable()
    {
        ready = true;
    }

    private void Awake()
    {
        originalScale = transform.localScale;
    }

    public void OnSelect()
    {
        if (ready)
        {
            StartCoroutine(Animate());
        }
    }

    private IEnumerator Animate()
    {
        ready = false;
        Vector3 reducedScale = originalScale * scalePercent;
        transform.localScale = originalScale;
        float distance = Vector3.Distance(originalScale, reducedScale);
        for (float i = 0; i <= 1;)
        {
            i += ((speed * Time.deltaTime) / distance);
            transform.localScale = Vector3.Lerp(originalScale, reducedScale, i);
            yield return null;
        }
        transform.localScale = reducedScale;
        yield return null;
        distance = Vector3.Distance(originalScale, reducedScale);
        for (float i = 0; i <= 1;)
        {
            i += ((speed * Time.deltaTime) / distance);
            transform.localScale = Vector3.Lerp(reducedScale, originalScale, i);
            yield return null;
        }
        transform.localScale = originalScale;
        ready = true;
    }
}