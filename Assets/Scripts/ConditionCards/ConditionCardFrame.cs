using UnityEngine;

public class ConditionCardFrame : MonoBehaviour
{
    public delegate void TappedFrame();

    public TappedFrame tappedFrameDelegate;

    private void OnSelect()
    {
        tappedFrameDelegate();
    }
}