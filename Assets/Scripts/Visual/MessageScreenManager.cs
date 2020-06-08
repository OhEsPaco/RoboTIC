using System;
using System.Collections.Generic;
using UnityEngine;
using static MessageScreenButton;

public class MessageScreenManager : MonoBehaviour
{
    private Dictionary<string, MessageScreen> messageScreensDic = new Dictionary<string, MessageScreen>();

    public delegate void OnMessageScreenButtonPressed();

    // Start is called before the first frame update
    private void Start()
    {
        MessageScreen[] messageScreens = GetComponentsInChildren<MessageScreen>();
        foreach (MessageScreen messageScreen in messageScreens)
        {
            if (messageScreensDic.ContainsKey(messageScreen.ScreenName))
            {
                Debug.LogError("Duplicate screen: " + messageScreen.ScreenName);
            }
            else
            {
                messageScreensDic.Add(messageScreen.ScreenName, messageScreen);
            }
            messageScreen.gameObject.SetActive(false);
        }
        EventAggregator.Instance.Subscribe<MsgShowScreen>(ShowScreen);
        EventAggregator.Instance.Subscribe<MsgHideAllScreens>(HideAllScreens);
    }

    private void HideAllScreens(MsgHideAllScreens msg)
    {
        foreach (KeyValuePair<string, MessageScreen> entry in messageScreensDic)
        {
            entry.Value.ResetAllButtons();
            entry.Value.gameObject.SetActive(false);
        }
    }

    private void ShowScreen(MsgShowScreen msg)
    {
        if (messageScreensDic.ContainsKey(msg.screenName))
        {
            MessageScreen msgScreen = messageScreensDic[msg.screenName];
            msgScreen.gameObject.SetActive(true);
            foreach (Tuple<MessageScreenButtons, OnMessageScreenButtonPressed> t in msg.listOfActions)
            {
                msgScreen.AddDelegateToButton(t.Item1, t.Item2);
            }
        }
    }
}