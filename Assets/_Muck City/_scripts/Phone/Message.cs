using System.Collections;
using System.Threading.Tasks;
using Sirenix.OdinInspector;
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
    [SerializeField] bool _debug = true;
    [SerializeField] float _bottomPadding = 5;

    bool IsUsingUnderLine => _underLine != null && _underLine.activeSelf;


    //* MAIN FUNCTION IS TO DETERMINE WHICH POOL TO RELEASE MESSAGE TO
    public bool _isPlayerMessage = false;

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
        DelayedSetHeight();
    }
    public void SetMessage(string title, string content)
    {
        _title.text = title;
        _content.text = content;
        // DelayedSetHeight(true);
    }

    [Button]
    public void SetHeight(bool useTitle = false)
    {
        _content.ForceMeshUpdate();
        int totalLines = _content.textInfo.lineCount;

        if (useTitle) totalLines += 1;
        if (IsUsingUnderLine) totalLines += 1;

        float lineHeight = _content.textInfo.lineInfo[0].lineHeight;
        float totalLineheight = lineHeight * totalLines;

        if (_debug)
        {
            Debug.Log($"Line height: {totalLineheight}");
            Debug.Log($"Total lines: {totalLines}");
        }

        RectTransform rectTransform = GetComponent<RectTransform>();
        Vector2 sizeDelta = rectTransform.sizeDelta;
        sizeDelta.y = totalLineheight + _bottomPadding;
        rectTransform.sizeDelta = sizeDelta;
    }

    public void PlayVoiceMessage()
    {
        if (_audioClip != null)
        {
            AudioSource.PlayClipAtPoint(_audioClip, Player.Instance.transform.position);
        }
    }


    // private IEnumerator DelayedSetHeight()
    // {
    //     yield return null; // Wait for end of frame

    // }

    public async void DelayedSetHeight(bool useTitle = false)
    {
        // await Task.Delay(delay * 100);
        await Task.Yield();
        SetHeight(useTitle);
    }
}
