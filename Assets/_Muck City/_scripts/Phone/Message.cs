using TMPro;
using UnityEngine;
using UnityUtils;


public class Message : MonoBehaviour
{

    public AudioClip _audioClip;

    public TextMeshProUGUI _title;
    public TextMeshProUGUI _content;
    public GameObject _underLine;

    public bool _isRejected = false;

    bool IsUsingUnderLine => _underLine.activeSelf;

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


    public void UseUnderLine()
    {
        _underLine.SetActive(true);
    }

    public void SetMessage(string content)
    {

        _content.text = content;
    }
    public void SetMessage(string title = null, string content = null)
    {
        if (title.IsNullOrEmpty())
        {
            _title.text = "Unknown Sender";
        }
        else
        {
            _title.text = title;
        }
        _content.text = content;

    }

    public void PlayVoiceMessage()
    {
        if (_audioClip != null)
        {
            AudioSource.PlayClipAtPoint(_audioClip, Player.Instance.transform.position);
        }
    }
}
