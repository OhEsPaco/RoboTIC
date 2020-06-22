using System.Collections;
using UnityEngine;

public class FindingSpaceSign : MonoBehaviour
{
    [SerializeField] private GameObject[] points = new GameObject[0];
    [SerializeField] private float time = 1f;

    private IEnumerator findingSpace = null;

    private void Awake()
    {
        EventAggregator.Instance.Subscribe<MsgFindingSpace>(FindingSpace);
    }

    private void FindingSpace(MsgFindingSpace msg)
    {
        if (!msg.isFindingSpace)
        {
            if (findingSpace != null)
            {
                StopCoroutine(findingSpace);
                findingSpace = null;
            }
            foreach (Transform child in transform)
            {
                child.gameObject.SetActive(false);
            }
        }
        else
        {
            if (findingSpace == null)
            {
                foreach (Transform child in transform)
                {
                    child.gameObject.SetActive(true);
                }

                findingSpace = FindingSpaceCrt();
                StartCoroutine(findingSpace);
            }
        }
    }

    private IEnumerator FindingSpaceCrt()
    {
        bool[] activePoints = new bool[points.Length];
        for (int i = 0; i < activePoints.Length; i++)
        {
            activePoints[i] = false;
        }
        yield return new WaitForSeconds(time);
        int index = 0;
        while (true)
        {
            if (index < activePoints.Length)
            {
                activePoints[index] = true;
                index++;
            }
            else
            {
                index = 0;
                for (int i = 0; i < activePoints.Length; i++)
                {
                    activePoints[i] = false;
                }
            }

            EnableOrDisablePoints(activePoints, points);
            yield return new WaitForSeconds(time);
        }
    }

    private void EnableOrDisablePoints(bool[] activePoints, GameObject[] points)
    {
        for (int i = 0; i < activePoints.Length; i++)
        {
            if (activePoints[i])
            {
                points[i].SetActive(true);
            }
            else
            {
                points[i].SetActive(false);
            }
        }
    }

    // Update is called once per frame
    private void Update()
    {
    }
}