using UnityEngine;

public class MessageBase
{
    public object sender { get; private set; } // MonoBehaviour отправителя
    public ServiceShareData id { get; private set; } // id сообщения
    public object data { get; private set; } // данные
    public string tag { get; private set; } //тег сообщения

    public MessageBase(object sender, ServiceShareData id, object data = null, string tag = "")
    {
        this.sender = sender;
        this.id = id;
        this.tag = tag;
        this.data = data;
    }
}