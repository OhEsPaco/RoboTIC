//https://www.c-sharpcorner.com/UploadFile/pranayamr/publisher-or-subscriber-pattern-with-event-or-delegate-and-e/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EventAggregator : MonoBehaviour
{
    private Dictionary<Type, IList> subscriber = new Dictionary<Type, IList>();

    private static EventAggregator eventAgregator;

    public static EventAggregator instance
    {
        get
        {
            if (!eventAgregator)
            {
                eventAgregator = FindObjectOfType(typeof(EventAggregator)) as EventAggregator;

                if (!eventAgregator)
                {
                    Debug.LogError("There needs to be one active EventAggregator script on a GameObject in your scene.");
                }
            }

            return eventAgregator;
        }
    }

    public void Publish<TMessageType>(TMessageType message)
    {
        Type t = typeof(TMessageType);
        IList actionlst;
        if (subscriber.ContainsKey(t))
        {
            actionlst = new List<Subscription<TMessageType>>(subscriber[t].Cast<Subscription<TMessageType>>());

            foreach (Subscription<TMessageType> a in actionlst)
            {
                a.Action(message);
            }
        }
    }

    public Subscription<TMessageType> Subscribe<TMessageType>(Action<TMessageType> action)
    {
        Type t = typeof(TMessageType);
        IList actionlst;
        var actiondetail = new Subscription<TMessageType>(action, this);

        if (!subscriber.TryGetValue(t, out actionlst))
        {
            actionlst = new List<Subscription<TMessageType>>();
            actionlst.Add(actiondetail);
            subscriber.Add(t, actionlst);
        }
        else
        {
            actionlst.Add(actiondetail);
        }

        return actiondetail;
    }

    public void Unsubscribe<TMessageType>(Subscription<TMessageType> subscription)
    {
        Type t = typeof(TMessageType);
        if (subscriber.ContainsKey(t))
        {
            subscriber[t].Remove(subscription);
        }
    }
}