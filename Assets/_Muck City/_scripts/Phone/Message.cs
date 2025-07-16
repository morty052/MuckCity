using System;
using System.Collections;
using System.Threading.Tasks;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.Pool;
using UnityUtils;


public class Message : MonoBehaviour
{

    public AudioClip _audioClip;

    public TextMeshProUGUI _title;
    public TextMeshProUGUI _content;
    public GameObject _underLine;

    public bool _isRejected = false;
    [SerializeField] bool _debug = false;
    [SerializeField] float _bottomPadding = 5;

    bool IsUsingUnderLine => _underLine != null && _underLine.activeSelf;


    //* MAIN FUNCTION IS TO DETERMINE WHICH POOL TO RELEASE MESSAGE TO
    // public bool _isPlayerMessage = false;

    public IObjectPool<Message> _pool;


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

    public void SetMessage(string content, Action<float> callback = null)
    {
        _content.text = content;
        DelayedSetHeight(false, callback);
    }
    public void SetMessage(string title, string content, Action<float> callback = null)
    {
        _title.text = title;
        _content.text = content;
        DelayedSetHeight(true, callback);
    }

    [Button]
    public float SetHeight(bool useTitle = false)
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
        return totalLineheight + _bottomPadding;
    }

    public float GetHeight()
    {
        float renderedHeight = GetComponent<RectTransform>().rect.height;
        return renderedHeight;
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

    public async void DelayedSetHeight(bool useTitle = false, Action<float> callback = null)
    {
        // await Task.Delay(delay * 100);
        await Task.Yield();
        float h = SetHeight(useTitle);
        callback?.Invoke(h);
    }

    public void Release()
    {
        Destroy(gameObject);
    }

    public void SetParent()
    {
        transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
        transform.localScale = Vector3.one;

    }
}
