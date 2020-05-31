using System.Collections;
using UnityEngine;

public class FaceTarget : MonoBehaviour
{
    private Transform target;
    private MessageWarehouse msgWar;

    // Start is called before the first frame update
    private void Start()
    {
        msgWar = new MessageWarehouse(EventAggregator.Instance);
        StartCoroutine(GetCameraTransform());
    }

    private IEnumerator GetCameraTransform()
    {
        MsgGetMainCameraTransform msg = new MsgGetMainCameraTransform();
        msgWar.PublishMsgAndWaitForResponse<MsgGetMainCameraTransform, Transform>(msg);
        yield return new WaitUntil(() => msgWar.IsResponseReceived<MsgGetMainCameraTransform, Transform>(msg, out target));
    }

    // Update is called once per frame
    private void Update()
    {
        if (target != null)
        {
            transform.LookAt(target);
        }
    }
}