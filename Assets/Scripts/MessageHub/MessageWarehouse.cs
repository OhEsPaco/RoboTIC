using System.Collections.Generic;
using UnityEngine;

public class MessageWarehouse
{
    //Manda un mensaje y espera un ResponseWrapper

    private EventAggregator eventAggregator;

    //Guarda el mensaje enviado y la respuesta
    private Dictionary<object, object> receivedMessages;

    //Guarda los tokens de subscripcion
    private Dictionary<object, object> subscriptionTokens;

    public MessageWarehouse(EventAggregator eventAggregator)
    {
        this.eventAggregator = eventAggregator;
        this.receivedMessages = new Dictionary<object, object>();
        this.subscriptionTokens = new Dictionary<object, object>();
    }

    public bool IsResponseReceived<MessageType, ResponseType>(in MessageType msg, out ResponseType response)
    {
        //Si hay alguna entrada de mensajes
        if (receivedMessages.ContainsKey(msg))
        {
            //Si se ha recibido algo
            if (receivedMessages[msg] != null)
            {
                //Si la respuesta es del tipo apropiado
                if (receivedMessages[msg] is ResponseType)
                {
                    response = (ResponseType)receivedMessages[msg];
                    receivedMessages.Remove(msg);
                    return true;
                }
            }
        }
        //Si no se ha recibido nada
        response = default(ResponseType);
        return false;
    }

    public void PublishMsgAndWaitForResponse<MessageType, ResponseType>(MessageType msg)
    {
        //Metemos el mensaje sin respuesta en el diccionario
        if (!receivedMessages.ContainsKey(msg))
        {
            receivedMessages.Add(msg, null);
            //Nos suscribimos
            Subscription<ResponseWrapper<MessageType, ResponseType>> subsToken = eventAggregator.Subscribe<ResponseWrapper<MessageType, ResponseType>>(this.EventReceiver<MessageType, ResponseType>);
            Debug.Log("Subscribed: " + msg.ToString());

            //Guardamos el token de suscripcion
            if (!subscriptionTokens.ContainsKey(msg))
            {
                subscriptionTokens.Add(msg, subsToken);
            }

            //Mandamos el mensaje
            Debug.Log("Published: " + msg.ToString());
            eventAggregator.Publish(msg);
        }
    }

    private void EventReceiver<MessageType, ResponseType>(ResponseWrapper<MessageType, ResponseType> msg)
    {
        //Si el diccionario de mensajes recibidos contiene una entrada para esta peticion seguimos
        if (receivedMessages.ContainsKey(msg.Petition))
        {
            //Si para esa peticion no se ha recibido nada, seguimos
            if (receivedMessages[msg.Petition] == null)
            {
                //Ponemos la respuesta para poder usarla cuando se requiera
                receivedMessages[msg.Petition] = msg.Response;
                Debug.Log("Received: " + msg.Petition.ToString());
                //Si hay token de subscripcion para este mensaje seguimos
                if (subscriptionTokens.ContainsKey(msg.Petition))
                {
                    //Comprobamos que no sea null
                    if (subscriptionTokens[msg.Petition] != null)
                    {
                        //Comprobamos que sea del tipo correcto
                        if (subscriptionTokens[msg.Petition] is Subscription<ResponseWrapper<MessageType, ResponseType>>)
                        {
                            //Nos desuscribimos
                            eventAggregator.Unsubscribe<ResponseWrapper<MessageType, ResponseType>>((Subscription<ResponseWrapper<MessageType, ResponseType>>)subscriptionTokens[msg.Petition]);

                            //Quitamos el token del diccionario
                            subscriptionTokens.Remove(msg.Petition);
                            Debug.Log("Unsubscribed: " + msg.Petition.ToString());
                        }
                    }
                }
            }
        }
    }
}