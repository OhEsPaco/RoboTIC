using UnityEngine;
using static MessageScreenManager;

[RequireComponent(typeof(Collider))]
public class MessageScreenButton : MonoBehaviour
{
    private OnMessageScreenButtonPressed informOnPressed;
    [SerializeField] private MessageScreenButtons buttonType;
    public OnMessageScreenButtonPressed InformOnPressed { get => informOnPressed; set => informOnPressed += value; }
    public MessageScreenButtons ButtonType { get => buttonType; set => buttonType = value; }

    public enum MessageScreenButtons
    {
        NoAction,
        Yes,
        No
    }

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