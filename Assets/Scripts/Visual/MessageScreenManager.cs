using System;
using System.Collections.Generic;
using UnityEngine;

public class MessageScreenManager : MonoBehaviour
{
    private Dictionary<string, MessageScreen> messageScreensDic = new Dictionary<string, MessageScreen>();

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
    }

    private void ShowScreen(MsgShowScreen msg)
    {
        if (messageScreensDic.ContainsKey(msg.screenName))
        {
            messageScreensDic[msg.screenName].gameObject.SetActive(true);
        }
    }
}