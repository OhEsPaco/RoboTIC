using System;
using static MessageScreenButton;
using static MessageScreenManager;

public class MsgShowScreen
{
    public string screenName;
    public Tuple<MessageScreenButtons, OnMessageScreenButtonPressed>[] listOfActions;

    public MsgShowScreen(string screenName, Tuple<MessageScreenButtons, OnMessageScreenButtonPressed>[] listOfActions)
    {
        this.screenName = screenName;
        this.listOfActions = listOfActions;
    }

    public MsgShowScreen(string screenName)
    {
        this.screenName = screenName;
        this.listOfActions = new Tuple<MessageScreenButtons, OnMessageScreenButtonPressed>[0];
    }
}