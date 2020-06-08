using System.Collections.Generic;
using UnityEngine;
using static MessageScreenButton;
using static MessageScreenManager;

public class MessageScreen : MonoBehaviour
{
    [SerializeField] private string screenName = "defaultName";
    private Dictionary<MessageScreenButtons, MessageScreenButton> buttons = new Dictionary<MessageScreenButtons, MessageScreenButton>();
    private MessageScreenButton[] allButtons;
    public string ScreenName { get => screenName; }

    // Start is called before the first frame update
    private void Start()
    {
        allButtons = GetComponentsInChildren<MessageScreenButton>();
        foreach (MessageScreenButton b in allButtons)
        {
            Debug.Log(screenName + b.ButtonType);
            if (!buttons.ContainsKey(b.ButtonType))
            {
                buttons.Add(b.ButtonType, b);
            }
        }
    }

    private void OnEnable()
    {
        allButtons = GetComponentsInChildren<MessageScreenButton>();
        foreach (MessageScreenButton b in allButtons)
        {
            Debug.Log(screenName + b.ButtonType);
            if (!buttons.ContainsKey(b.ButtonType))
            {
                buttons.Add(b.ButtonType, b);
            }
        }
    }

    public void AddDelegateToButton(MessageScreenButtons bType, OnMessageScreenButtonPressed bDelegate)
    {
        if (buttons.ContainsKey(bType))
        {
            Debug.Log("BBBBBBBBB");
            buttons[bType].InformOnPressed = bDelegate;
        }
    }

    public void ResetAllButtons()
    {
        foreach (MessageScreenButton b in allButtons)
        {
            b.ResetDelegates();
        }
    }
}