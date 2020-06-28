using System;
using static MessageScreenManager;

public class MsgShowScreen
{
    public string screenName;
    public Tuple<string, OnMessageScreenButtonPressed>[] listOfActions;
    public float seconds = -1;

    public MsgShowScreen(string screenName, Tuple<string, OnMessageScreenButtonPressed>[] listOfActions)
    {
        this.screenName = screenName;
        this.listOfActions = listOfActions;
    }

    public MsgShowScreen(string screenName, Tuple<string, OnMessageScreenButtonPressed>[] listOfActions, float seconds)
    {
        this.screenName = screenName;
        this.listOfActions = listOfActions;
        this.seconds = seconds;
    }

    public MsgShowScreen(string screenName)
    {
        this.screenName = screenName;
        this.listOfActions = new Tuple<string, OnMessageScreenButtonPressed>[0];
    }

    public MsgShowScreen(string screenName, float seconds)
    {
        this.screenName = screenName;
        this.listOfActions = new Tuple<string, OnMessageScreenButtonPressed>[0];
        this.seconds = seconds;
    }
}