using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


struct PersistentMessageScreenData
{
    public List<Message> _messages;

    public PersistentMessageScreenData(List<Message> messages)
    {
        _messages = messages;
    }
}

public class MessagesScreen : MonoBehaviour
{

    public List<MessagePrefab> _messageObjects = new();
    public List<Message> _savedMessages = new();
    [SerializeField] GameObject _messagePrefab;
    [SerializeField] GameObject _messagesHome;
    [SerializeField] GameObject _fullChat;
    [SerializeField] Transform _newMessagesParentTransform;

    [SerializeField] private TextMeshProUGUI _activeMessageTitle;
    [SerializeField] private TextMeshProUGUI _activeMessageBody;
    [SerializeField] private Image _contactPhoto;

    int _activeMessageIndex = 0;

    MessagePrefab ActiveMessage => _messageObjects[_activeMessageIndex];

    public bool ShouldAutoSave { get => ShouldAutoSave; set => ShouldAutoSave = value; }
    public string SAVE_FILE_NAME { get => "Messages"; set => throw new System.NotImplementedException(); }


    void Awake()
    {
        // LoadPersistentData();
    }

    // void OnEnable()
    // {
    //     PhoneNavigation.OnUpPressed += SelectPrevMsg;
    //     PhoneNavigation.OnDownPressed += SelectNextMsg;
    //     PhoneNavigation.OnSelectPressed += OpenFullChat;
    //     PhoneNavigation.OnBackPressed += ExitFullChat;
    //     PhoneNavigation.OnAcceptPressed += AcceptMessageRequest;
    //     PhoneNavigation.OnRejectPressed += RejectMessageRequest;
    // }

    // void OnDisable()
    // {
    //     PhoneNavigation.OnUpPressed -= SelectPrevMsg;
    //     PhoneNavigation.OnDownPressed -= SelectNextMsg;
    //     PhoneNavigation.OnSelectPressed -= OpenFullChat;
    //     PhoneNavigation.OnBackPressed -= ExitFullChat;
    //     PhoneNavigation.OnAcceptPressed -= AcceptMessageRequest;
    //     PhoneNavigation.OnRejectPressed -= RejectMessageRequest;
    // }



    void Start()
    {
        // ReloadSavedMessages();
    }


    private void AcceptMessageRequest()
    {
        if (ActiveMessage._message._isRejected)
        {
            Debug.Log("message already rejected");
            return;
        }
        // ActiveMessage._message._request.StartRequest();
    }

    private void RejectMessageRequest()
    {
        ActiveMessage._message._isRejected = true;
        // ActiveMessage._message._request.Reject();
    }
    public void HandleNewMessage(Message message)
    {
        MessagePrefab messagePrefab = Instantiate(_messagePrefab, _newMessagesParentTransform).GetComponent<MessagePrefab>();
        messagePrefab.InitMessage(message);
        _messageObjects.Add(messagePrefab);
        _savedMessages.Add(message);
        // TriggerAutoSave();
    }

    void SelectNextMsg()
    {
        _activeMessageIndex = (_activeMessageIndex + 1) % _messageObjects.Count;
        Debug.Log("active message index" + _activeMessageIndex);
    }

    void SelectPrevMsg()
    {
        _activeMessageIndex = (_activeMessageIndex - 1) % _messageObjects.Count;
        Debug.Log("active message index" + _activeMessageIndex);
    }


    public void OpenFullChat()
    {
        _activeMessageBody.text = ActiveMessage._bodyPreview.text;
        _activeMessageTitle.text = ActiveMessage._title.text;
        _messagesHome.SetActive(false);
        _fullChat.SetActive(true);
    }
    public void ExitFullChat()
    {
        _fullChat.SetActive(false);
        _messagesHome.SetActive(true);

    }

    void ReloadSavedMessages()
    {
        foreach (Message savedMessage in _savedMessages)
        {
            MessagePrefab messagePrefab = Instantiate(_messagePrefab, _newMessagesParentTransform).GetComponent<MessagePrefab>();
            messagePrefab.InitMessage(savedMessage);
            _messageObjects.Add(messagePrefab);
        }
    }

    public void TriggerAutoSave()
    {
        PersistentMessageScreenData data = new(_savedMessages);
        Debug.Log("saved messages" + data._messages.Count);
        // ES3.Save(SAVE_FILE_NAME, data);
    }

    // public void LoadPersistentData()
    // {
    //     // if (!ES3.FileExists(SAVE_FILE_NAME)) return;
    //     PersistentMessageScreenData data = ES3.Load<PersistentMessageScreenData>(SAVE_FILE_NAME);
    //     Debug.Log("Loaded messages" + data._messages.Count);
    //     if (data._messages != null)
    //     {
    //         if (data._messages.Count == 0) return;
    //         _savedMessages = data._messages;
    //     }
    // }
}
