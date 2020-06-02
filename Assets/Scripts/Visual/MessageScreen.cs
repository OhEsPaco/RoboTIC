using UnityEngine;
using static MessageScreenButton;

public class MessageScreen : MonoBehaviour
{
    [SerializeField] private string screenName = "defaultName";
    private MessageScreenButton[] buttons;
    public string ScreenName { get => screenName; }

    // Start is called before the first frame update
    private void Start()
    {
        buttons = GetComponentsInChildren<MessageScreenButton>();
        foreach (MessageScreenButton b in buttons)
        {
            b.InformOnPressed = PressedButton;
        }
    }

    private void PressedButton(MessageScreenButtons button)
    {
    }
}