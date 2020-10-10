using UnityEngine;

public class MessageBase
{
    public object sender { get; private set; } // MonoBehaviour отправителя
    public ServiceShareData id { get; private set; } // id сообщения
    public object data { get; private set; } // данные
    public string tag { get; private set; } //тег сообщения

    public MessageBase(object sender, ServiceShareData id, string tag, object data = null)
    {
        this.sender = sender;
        this.id = id;
        this.tag = tag;
        this.data = data;
    }

    public static MessageBase Create(object sender, ServiceShareData id, string tag = null)
    {
        return new MessageBase(sender, id, tag);
    }
}