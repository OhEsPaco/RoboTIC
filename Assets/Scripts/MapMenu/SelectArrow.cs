using UnityEngine;

public class SelectArrow : MonoBehaviour
{
    public delegate void CallbackDelegate();

    private CallbackDelegate callbackDelegate;

    private void OnSelect()
    {
        if (callbackDelegate != null)
        {
            callbackDelegate();
        }
    }

    public void InformMe(CallbackDelegate action)
    {
        callbackDelegate += action;
    }
}