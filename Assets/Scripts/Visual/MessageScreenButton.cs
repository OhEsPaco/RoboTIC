using UnityEngine;
using static MessageScreenManager;

[RequireComponent(typeof(Collider))]
public class MessageScreenButton : MonoBehaviour
{
    private OnMessageScreenButtonPressed informOnPressed;
    [SerializeField] private string buttonType;
    public OnMessageScreenButtonPressed InformOnPressed { get => informOnPressed; set => informOnPressed += value; }
    public string ButtonType { get => buttonType; set => buttonType = value; }

 

    private void OnMouseDown()
    {
        if (informOnPressed != null)
        {
            informOnPressed();
        }
    }

    public void OnSelect()
    {
        if (informOnPressed != null)
        {
            informOnPressed();
        }
    }

    public void ResetDelegates()
    {
        informOnPressed = null;
    }
}