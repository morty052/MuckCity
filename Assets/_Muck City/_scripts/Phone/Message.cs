using TMPro;
using UnityEngine;


public class Message : MonoBehaviour
{
    public string _title;
    public string _body;

    public string _senderId;

    public bool _isFromMind = false;

    public AudioClip _audioClip;

    public TextMeshProUGUI _content;


    public bool _read = false;
    public bool _isRejected = false;

    // public Message() { }

    // public class Builder
    // {
    //     readonly Message message = new();

    //     public Builder(string title)
    //     {
    //         message._title = title;

    //     }
    //     public Builder WithCitizen(string sender)
    //     {
    //         message._senderId = sender;
    //         return this;
    //     }
    //     public Builder WithBody(string body)
    //     {
    //         message._body = body;
    //         return this;
    //     }

    //     public Message Build()
    //     {
    //         //* SEND MESSAGE
    //         // message.OnReceived(message);
    //         return message;
    //     }
    // }



    public void PlayVoiceMessage()
    {
        if (_audioClip != null)
        {
            AudioSource.PlayClipAtPoint(_audioClip, Player.Instance.transform.position);
        }
    }
}
