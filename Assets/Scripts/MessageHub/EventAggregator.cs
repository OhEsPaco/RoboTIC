//https://www.c-sharpcorner.com/UploadFile/pranayamr/publisher-or-subscriber-pattern-with-event-or-delegate-and-e/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Defines the <see cref="EventAggregator" />.
/// </summary>
public class EventAggregator : MonoBehaviour
{
    /// <summary>
    /// Defines the subscriber.
    /// </summary>
    private Dictionary<Type, IList> subscriber = new Dictionary<Type, IList>();

    /// <summary>
    /// Defines the eventAgregator.
    /// </summary>
    private static EventAggregator eventAgregator;

    /// <summary>
    /// Gets the Instance.
    /// </summary>
    public static EventAggregator Instance
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

    /// <summary>
    /// The Publish.
    /// </summary>
    /// <typeparam name="TMessageType">.</typeparam>
    /// <param name="message">The message<see cref="TMessageType"/>.</param>
    public void Publish<TMessageType>(TMessageType message)
    {
        Type t = typeof(TMessageType);
        IList actionlst;
        if (subscriber.ContainsKey(t))
        {
            actionlst = new List<Subscription<TMessageType>>(subscriber[t].Cast<Subscription<TMessageType>>());

            foreach (Subscription<TMessageType> a in actionlst)
            {
                try
                {
                    a.Action(message);
                }
                catch
                {
                    Unsubscribe(a);
                }
            }
        }
    }

    /// <summary>
    /// The Subscribe.
    /// </summary>
    /// <typeparam name="TMessageType">.</typeparam>
    /// <param name="action">The action<see cref="Action{TMessageType}"/>.</param>
    /// <returns>The <see cref="Subscription{TMessageType}"/>.</returns>
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

    /// <summary>
    /// The Unsubscribe.
    /// </summary>
    /// <typeparam name="TMessageType">.</typeparam>
    /// <param name="subscription">The subscription<see cref="Subscription{TMessageType}"/>.</param>
    public void Unsubscribe<TMessageType>(Subscription<TMessageType> subscription)
    {
        Type t = typeof(TMessageType);
        if (subscriber.ContainsKey(t))
        {
            subscriber[t].Remove(subscription);
        }
    }
}