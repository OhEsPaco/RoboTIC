using UnityEngine;

[RequireComponent(typeof(Collider))]
public class MessageScreenButton : MonoBehaviour
{
    public delegate void MessageScreenButtonPressed(MessageScreenButtons pressed);

    private MessageScreenButtonPressed informOnPressed;
    [SerializeField] private MessageScreenButtons buttonType;
    public MessageScreenButtonPressed InformOnPressed { get => informOnPressed; set => informOnPressed += value; }

    public enum MessageScreenButtons
    {
        NoAction,
        Yes,
        No
    }

    private void OnMouseDown()
    {
        informOnPressed(buttonType);
    }

    public void OnSelect()
    {
        informOnPressed(buttonType);
    }
}