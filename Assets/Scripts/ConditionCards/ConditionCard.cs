using System.Collections;
using UnityEditor;
using UnityEngine;
using static Block;

public class ConditionCard : MonoBehaviour
{
    [SerializeField] private BlockProperties condition;
    [SerializeField] private bool checkFrontBlock = false;
    [SerializeField] private Transform endPosition;

    [Range(0.001f, 1000f)]
    [SerializeField] private float speed = 0.2f;

    public delegate void TappedCard(ConditionCard card);

    private TappedCard informOnTap;

    public BlockProperties Condition { get => condition; }

    public bool CheckFrontBlock { get => checkFrontBlock; }
    public bool ActionsFinished { get => actionsFinished; }
    public TappedCard InformOnTap { get => informOnTap; set => informOnTap += value; }

    private Transform childTransform;

    private void Awake()
    {
        ConditionCardFrame child = GetComponentInChildren<ConditionCardFrame>();
        if (child != null)
        {
            child.tappedFrameDelegate += TappedFrame;
            childTransform = child.transform;
        }
    }

    private bool actionsFinished = true;

    public void ShowCard()
    {
        StartCoroutine(ShowCardCrt());
    }

    public void HideCard()
    {
        StartCoroutine(HideCardCrt());
    }

    private void TappedFrame()
    {
        informOnTap(this);
    }

    private IEnumerator ShowCardCrt()
    {
        actionsFinished = false;
        if (childTransform != null)
        {
            Vector3 startPos = transform.position;
            Vector3 endPos = endPosition.position;
            float distance = Vector3.Distance(endPos, startPos);
            for (float i = 0; i <= 1;)
            {
                startPos = transform.position;
                endPos = endPosition.position;
                distance = Vector3.Distance(endPos, startPos);
                i += ((speed * Time.deltaTime) / distance);
                childTransform.transform.position = Vector3.Lerp(startPos, endPos, i);
                yield return null;
            }
            childTransform.transform.position = endPos;
        }
        actionsFinished = true;
    }

    private IEnumerator HideCardCrt()
    {
        actionsFinished = false;
        if (childTransform != null)
        {
            Vector3 startPos = endPosition.position;
            Vector3 endPos = transform.position;
            float distance = Vector3.Distance(endPos, startPos);
            for (float i = 0; i <= 1;)
            {
                startPos = endPosition.position;
                endPos = transform.position;
                distance = Vector3.Distance(endPos, startPos);
                i += ((speed * Time.deltaTime) / distance);
                childTransform.transform.position = Vector3.Lerp(startPos, endPos, i);
                yield return null;
            }
            childTransform.transform.position = endPos;
        }
        actionsFinished = true;
    }

    private void OnDrawGizmos()
    {
        if (endPosition != null)
        {
            Gizmos.color = Color.green;
            Handles.Label(endPosition.position, condition.ToString());
            Gizmos.DrawSphere(endPosition.position, 0.001f);
        }
    }
}