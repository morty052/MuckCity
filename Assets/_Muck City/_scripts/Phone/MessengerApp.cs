using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using DialogueEditor;
using Sirenix.OdinInspector;
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

public class MessengerApp : PhoneApp
{

    // public List<MessagePrefab> _messageObjects = new();
    // public List<Message> _savedMessages = new();
    // [SerializeField] GameObject _messagePrefab;
    // [SerializeField] GameObject _messagesHome;
    // [SerializeField] GameObject _fullChat;
    // [SerializeField] Transform _newMessagesParentTransform;

    // [SerializeField] private TextMeshProUGUI _activeMessageTitle;
    // [SerializeField] private TextMeshProUGUI _activeMessageBody;

    // MessagePrefab ActiveMessage => _messageObjects[_activeMessageIndex];
    [SerializeField, TabGroup("Components")] private GameObject _fullScreenMessageView;
    [SerializeField, TabGroup("Components")] private Image _contactPhoto;
    [TabGroup("Components")] public Transform _messagesParent;



    [TabGroup("Components")] public Message _newMessagePrefab;
    [TabGroup("Components")] public Message _responsePrefab;

    [SerializeField, TabGroup("Settings")] private int _maxMessagesOnScreen = 0;
    [TabGroup("Debug")] public Chat _activeConvo;

    [TabGroup("Debug"), SerializeField] private int _messagesOnScreen = 0;
    [TabGroup("Debug")] public bool _canRespond = false;

    //*
    public SpeechNode _rootNode;
    public ConversationNode _activeNode;
    private HashSet<OptionNode> _activeNodeOptions = new();
    private OptionNode _lastSelectedOption;

    Action OnAddMessageToScreen;

    public bool ShouldAutoSave { get => ShouldAutoSave; set => ShouldAutoSave = value; }
    public string SAVE_FILE_NAME { get => "Messages"; set => throw new System.NotImplementedException(); }

    void OnEnable()
    {
        OnAddMessageToScreen += AddMessageToScreen;
    }
    void OnDisable()
    {
        OnAddMessageToScreen -= AddMessageToScreen;
    }

    private void AddMessageToScreen()
    {
        //* INCREMENT MESSAGES ON SCREEN
        _messagesOnScreen++;
        if (_messagesOnScreen >= _maxMessagesOnScreen)
        {
            float messageHeight = _newMessagePrefab.GetComponent<RectTransform>().rect.height;
            _messagesParent.transform.DOLocalMoveY(_messagesParent.transform.localPosition.y + messageHeight, 1f);
        }
    }


    [Button, TabGroup("Debug")]
    void CalculateScreenSpace()
    {
        RectTransform parentTransform = _messagesParent.GetComponent<RectTransform>();
        RectTransform messageTransform = _newMessagePrefab.GetComponent<RectTransform>();

        float parentHeight = parentTransform.rect.height;

        float messageHeight = messageTransform.rect.height;

        int messagesFit = Mathf.FloorToInt(parentHeight / messageHeight);
        Debug.Log($"Parent Height: {parentHeight}, Message Height: {messageHeight}, Messages that can fit on Screen: {messagesFit}");

        _maxMessagesOnScreen = messagesFit;
    }


    [Button, TabGroup("Debug")]
    public void SetUpIm(Chat convo)
    {
        Conversation conversation = convo.GetSpeechNodes();

        _rootNode = conversation.Root;
        _activeNode = _rootNode;

        // GetActiveSpeechNode();
        if (_activeNode.Connections.Count > 0)
        {
            SetActiveOptions();
        }

    }

    public void SetActiveOptions()
    {
        //* CLEAR CURRENT OPTIONS IF ALREADY PRESENT
        if (_activeNodeOptions.Count > 0)
        {
            _activeNodeOptions.Clear();
        }

        //* LOOP THROUGH OPTIONS
        for (int i = 0; i < _activeNode.Connections.Count; i++)
        {
            OptionConnection connection = _activeNode.Connections[i] as OptionConnection;

            //* SET OPTIONS AS THE ACTIVE OPTIONS FOR ACTIVE SPEECH NODE
            _activeNodeOptions.Add(connection.OptionNode);
            // Debug.Log($"Option {i} {connection.OptionNode.Text}");
        }

    }

    public string DisplayActiveSpeechNode()
    {
        Message message = Instantiate(_newMessagePrefab, _messagesParent);
        message.gameObject.SetActive(true);
        message._content.text = _activeNode.Text;
        OnAddMessageToScreen?.Invoke();
        return _activeNode.Text;
    }

    bool SelectedOptionHasSpeech()
    {
        if (_lastSelectedOption.Connections.Count == 0) return false;
        return true;
    }
    bool ActiveSpeechHasOptions()
    {
        if (_activeNode.Connections.Count == 0) return false;
        return _activeNode.Connections.Any(x => x.ConnectionType == Connection.eConnectionType.Option);
    }

    public void SetActiveConvo(Chat convo)
    {
        _activeConvo = convo;
    }

    public void StartConvo()
    {
        Message message = Instantiate(_newMessagePrefab, _messagesParent);
        message.gameObject.SetActive(true);
        message._content.text = _activeNode.Text;
        OnAddMessageToScreen?.Invoke();
        //* ALLOW PLAYER TO RESPOND AFTER FIRST MESSAGE IS SHOWN
        _canRespond = true;
    }

    [Button, TabGroup("Debug")]
    public void ProgressConvo(int choice)
    {
        if (!_canRespond) return;
        // Debug.Log($"Selected {_activeNodeOptions.ElementAt(choice).Text}");

        ReplyMessage(_activeNodeOptions.ElementAt(choice).Text);

        //* Set LAST SELECTED OPTION TO OPTION AT INDEX OF CHOICE
        _lastSelectedOption = _activeNodeOptions.ElementAt(choice);

        //* STOP PLAYER FROM RESPONDING UNTIL NEXT MESSAGE IS SHOWN
        _canRespond = false;

        //* CHECK IF SELECTED OPTION HAS FURTHER DIALOGUE
        if (SelectedOptionHasSpeech())
        {
            Invoke(nameof(ShowNextMessage), 2f);
        }

        else
        {
            Invoke(nameof(EndConvo), 3f);
        }
    }
    [Button, TabGroup("Debug")]
    public void DebugScreenMovement()
    {
        float messageHeight = _newMessagePrefab.GetComponent<RectTransform>().rect.height;
        _messagesParent.transform.localPosition = new Vector3(_messagesParent.transform.localPosition.x, _messagesParent.transform.localPosition.y + messageHeight, _messagesParent.transform.localPosition.z);
        Debug.Log(messageHeight);
    }
    private void EndConvo()
    {
        // gameObject.SetActive(false);
        Debug.Log("Done");
    }

    void ShowNextMessage()
    {
        //* GET SPEECH FOR SELECTED 
        SpeechConnection speechConnection = _lastSelectedOption.Connections.First(x => x.ConnectionType == Connection.eConnectionType.Speech) as SpeechConnection;

        //*UPDATE THE ACTIVE NODE TO SELECTED OPTIONS CONNECTED SPEECH
        _activeNode = speechConnection.SpeechNode;

        //* DISPLAY THE NEW SPEECH
        DisplayActiveSpeechNode();



        //* IF SPEECH HAS OPTIONS DISPLAY THEM
        if (ActiveSpeechHasOptions())
        {
            SetActiveOptions();
        }

        //* ALLOW PLAYER TO RESPOND AFTER MESSAGE IS DISPLAYED
        _canRespond = true;
    }
    public void ReplyMessage(string text)
    {
        Message message = Instantiate(_responsePrefab, _messagesParent);
        message._content.text = text;

        message.gameObject.SetActive(true);

        OnAddMessageToScreen?.Invoke();
    }


    public void OpenChat()
    {
        _fullScreenMessageView.SetActive(true);
    }
    // private void AcceptMessageRequest()
    // {
    //     if (ActiveMessage._message._isRejected)
    //     {
    //         Debug.Log("message already rejected");
    //         return;
    //     }
    //     // ActiveMessage._message._request.StartRequest();
    // }

    // private void RejectMessageRequest()
    // {
    //     ActiveMessage._message._isRejected = true;
    //     // ActiveMessage._message._request.Reject();
    // }
    // public void HandleNewMessage(Message message)
    // {
    //     MessagePrefab messagePrefab = Instantiate(_messagePrefab, _newMessagesParentTransform).GetComponent<MessagePrefab>();
    //     messagePrefab.InitMessage(message);
    //     _messageObjects.Add(messagePrefab);
    //     _savedMessages.Add(message);
    //     // TriggerAutoSave();
    // }

    // void SelectNextMsg()
    // {
    //     _activeMessageIndex = (_activeMessageIndex + 1) % _messageObjects.Count;
    //     Debug.Log("active message index" + _activeMessageIndex);
    // }

    // void SelectPrevMsg()
    // {
    //     _activeMessageIndex = (_activeMessageIndex - 1) % _messageObjects.Count;
    //     Debug.Log("active message index" + _activeMessageIndex);
    // }


    // public void OpenFullChat()
    // {
    //     _activeMessageBody.text = ActiveMessage._bodyPreview.text;
    //     _activeMessageTitle.text = ActiveMessage._title.text;
    //     _messagesHome.SetActive(false);
    //     _fullChat.SetActive(true);
    // }
    // public void ExitFullChat()
    // {
    //     _fullChat.SetActive(false);
    //     _messagesHome.SetActive(true);

    // }

    // void ReloadSavedMessages()
    // {
    //     foreach (Message savedMessage in _savedMessages)
    //     {
    //         MessagePrefab messagePrefab = Instantiate(_messagePrefab, _newMessagesParentTransform).GetComponent<MessagePrefab>();
    //         messagePrefab.InitMessage(savedMessage);
    //         _messageObjects.Add(messagePrefab);
    //     }
    // }

    // public void TriggerAutoSave()
    // {
    //     PersistentMessageScreenData data = new(_savedMessages);
    //     Debug.Log("saved messages" + data._messages.Count);
    //     // ES3.Save(SAVE_FILE_NAME, data);
    // }

}
