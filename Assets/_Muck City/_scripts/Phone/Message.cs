using UnityEngine;

[System.Serializable]
public class Message
{
    public string _title;
    public string _body;

    public string _senderId;

    public bool _isFromMind = false;

    public AudioClip _audioClip;


    public bool _read = false;
    public bool _isRejected = false;

    public Message() { }

    public class Builder
    {
        readonly Message message = new();

        public Builder(string title)
        {
            message._title = title;

        }
        public Builder WithCitizen(string sender)
        {
            message._senderId = sender;
            return this;
        }
        public Builder WithBody(string body)
        {
            message._body = body;
            return this;
        }

        public Message Build()
        {
            //* SEND MESSAGE
            // message.OnReceived(message);
            return message;
        }
    }

    // public void OnReceived(Message message)
    // {
    //     GameEventsManager.Instance.OnMessageReceivedEvent(message);
    // }

    public void PlayVoiceMessage()
    {
        if (_audioClip != null)
        {
            AudioSource.PlayClipAtPoint(_audioClip, Player.Instance.transform.position);
        }
    }
}
