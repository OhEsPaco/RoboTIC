using UnityEngine;

public class SelectArrow : MonoBehaviour
{
    public delegate void CallbackDelegate();

    private CallbackDelegate callbackDelegate;

    private void OnSelect()
    {
        Debug.Log("Pressed Arrow");
        callbackDelegate?.Invoke();
    }

    public void InformMeOfClickedArrow(CallbackDelegate action)
    {
        callbackDelegate += action;
    }
}