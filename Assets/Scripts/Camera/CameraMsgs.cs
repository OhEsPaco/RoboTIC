using UnityEngine;

public class CameraMsgs : MonoBehaviour
{
    private void Awake()
    {
        EventAggregator.Instance.Subscribe<MsgGetMainCameraTransform>(ServeCameraTransform);
    }

    private void ServeCameraTransform(MsgGetMainCameraTransform msg)
    {
        EventAggregator.Instance.Publish<ResponseWrapper<MsgGetMainCameraTransform, Transform>>(new ResponseWrapper<MsgGetMainCameraTransform, Transform>(msg, transform));
    }
}