using System.Collections;
using UnityEngine;

public class ArrowAnim : MonoBehaviour
{
    private Vector3 originalScale;
    private Vector3 reducedScale;

    [Range(0.0f, 1.0f)]
    public float reducedPercent = 0.5f;

    [Range(0.0f, 100.0f)]
    public float speed = 10f;

    [Range(0.0f, 100f)]
    public float scaleMultiplier = 20f;

    private void Start()
    {
        originalScale = new Vector3(scaleMultiplier, scaleMultiplier, scaleMultiplier);
        gameObject.transform.localScale = originalScale;
        reducedScale = originalScale * reducedPercent;
        StartCoroutine(Minimize());
    }

    private void OnEnable()
    {
        originalScale = new Vector3(scaleMultiplier, scaleMultiplier, scaleMultiplier);
        gameObject.transform.localScale = originalScale;
        reducedScale = originalScale * reducedPercent;
        StartCoroutine(Minimize());
    }

    private IEnumerator Minimize()
    {
        float distance = Vector3.Distance(originalScale, reducedScale);
        for (float i = 0; i <= 1;)
        {
            i += ((speed * Time.deltaTime) / distance);
            transform.localScale = Vector3.Lerp(originalScale, reducedScale, i);
            yield return null;
        }
        transform.localScale = reducedScale;
        yield return null;
        StartCoroutine(Maximize());
    }

    private IEnumerator Maximize()
    {
        float distance = Vector3.Distance(originalScale, reducedScale);
        for (float i = 0; i <= 1;)
        {
            i += ((speed * Time.deltaTime) / distance);
            transform.localScale = Vector3.Lerp(reducedScale, originalScale, i);
            yield return null;
        }
        transform.localScale = originalScale;
        yield return null;
        StartCoroutine(Minimize());
    }
}