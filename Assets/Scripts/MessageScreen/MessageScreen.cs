using System.Collections.Generic;
using UnityEngine;
using static MessageScreenManager;

public class MessageScreen : MonoBehaviour
{
    [SerializeField] private string screenName = "defaultName";
    private Dictionary<string, MessageScreenButton> buttons = new Dictionary<string, MessageScreenButton>();
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

    public void AddDelegateToButton(string bType, OnMessageScreenButtonPressed bDelegate)
    {
        if (buttons.ContainsKey(bType))
        {
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