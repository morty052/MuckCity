using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using DialogueEditor;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Pool;



public struct MessageContentStruct
{
    public string _content;
    public bool _playerSentMessage;

    public MessageContentStruct(string message, bool playerSentMessage)
    {
        _content = message;
        _playerSentMessage = playerSentMessage;
    }

}

[Serializable]
public class InstantMessage
{
    public string _senderName;
    public List<MessageContentStruct> _messages = new();

    public Message _messagePreview;

    public int ID;

    public bool _hasPendingMessages = true;

    public Chat _chat;
    public InstantMessage(Chat chat, string senderName, string content, Message messagePrefab, bool firstMessageIsPlayerMessage = false)
    {
        _chat = chat;
        _senderName = senderName;
        //! INITIAL MESSAGE IN LIST ALWAYS SET TO NOT PLAYER MESSAGE FOR NOW
        _messages.Add(new(content, false));
        _messagePreview = messagePrefab;
        _messagePreview.SetMessage(_messages[0]._content);

    }
    public void SetID(int id)
    {
        ID = id;
    }
    public void UpdateChat(string latestMessage, bool isPlayerText)
    {
        MessageContentStruct im = new(latestMessage, isPlayerText);
        _messages.Add(im);

        //* DISPLAY MESSAGE AS CURRENT CHAT PREVIEW MESSAGE
        _messagePreview.SetMessage(_senderName, latestMessage);
    }
}

public class MessengerApp : PhoneApp
{

    #region "inspector values"
    [SerializeField, TabGroup("Components")] private GameObject _fullScreenMessageView;
    [SerializeField, TabGroup("Components")] private Image _contactPhoto;
    [TabGroup("Components")] public Transform _messagesParent;

    [TabGroup("Components")] public Message _incomingMessagePrefab;
    [TabGroup("Components")] public Message _outgoingMessagePrefab;

    [SerializeField, TabGroup("Settings")] private int _maxMessagesOnScreen = 0;

    [SerializeField] private bool _isTexting;
    [SerializeField, TabGroup("Settings")] private bool _canEndChat;
    [TabGroup("Debug")] public Chat _activeConvo;

    [TabGroup("Debug"), SerializeField] private int _messagesOnScreen = 0;
    [TabGroup("Debug")] public bool _canRespond = false;
    [TabGroup("Debug")] public HashSet<EffectName> _chatEffects = new();
    Action OnAddMessageToScreen;
    #endregion

    #region "Non Inspector values"
    public SpeechNode _rootNode;
    public ConversationNode _activeNode;
    private HashSet<OptionNode> _activeNodeOptions = new();
    private OptionNode _lastSelectedOption;

    [SerializeField] private int _optionId = 0;
    #endregion

    #region "Magnified Messages"
    Action OnAddMessageToLargeScreen;

    [TabGroup("Magnified Components")] public Transform _magnifiedMessagesUi;
    [TabGroup("Magnified Components")] public Transform _magnifiedMessagesParent;

    [TabGroup("Magnified Components")]
    [SerializeField, TabGroup("Magnified Components")] TextMeshProUGUI _optionsOneText;

    [SerializeField, TabGroup("Magnified Components")] TextMeshProUGUI _optionsTwoText;
    [SerializeField, TabGroup("Magnified Components")] GameObject _endChatUi;
    [SerializeField, TabGroup("Magnified Components")] GameObject _optionsUi;

    [TabGroup("Magnified Components")] public Message _magnifiedIncomingMessagePrefab;
    [TabGroup("Magnified Components")] public Message _magnifiedOutgoingMessagePrefab;

    [TabGroup("Magnified Settings")] public int _maxMagnifiedMessagesOnScreen = 0;

    #endregion

    #region "message Preview"
    [SerializeField, TabGroup("Preview Components")] Transform _messagesPreviewParent;
    [SerializeField, TabGroup("Preview Components")] Message _messagePreviewPrefab;
    public List<InstantMessage> chats;

    public InstantMessage _activeChat;

    #endregion


    [TabGroup("Debug")] public int _magnifiedMessagesOnScreen = 0;

    public bool ShouldAutoSave { get => ShouldAutoSave; set => ShouldAutoSave = value; }
    bool IsViewingExpandedChat => _fullScreenMessageView.activeSelf;

    IObjectPool<Message> _incomingMessagePool;
    IObjectPool<Message> _outGoingMessagePool;
    IObjectPool<Message> _previewPool;
    IObjectPool<Message> _magnifiedIncomingMessagePool;
    IObjectPool<Message> _magnifiedOutgoingPool;
    [SerializeField] private ScrollRect _scrollRect;

    // Scroll down by 10% (0.1f)
    [Button("Scroll")]
    public void ScrollDownByAmount(float amount = 0.1f)
    {
        float newPos = Mathf.Clamp01(_scrollRect.verticalNormalizedPosition - amount);
        _scrollRect.verticalNormalizedPosition = newPos;
    }


    Message GetIncomingMessagePrefab()
    {
        return _incomingMessagePool.Get();
    }

    Message GetOutGoingMessagePrefab()
    {
        return _outGoingMessagePool.Get();
    }
    Message GetPreview()
    {
        return _previewPool.Get();
    }
    Message GetMagnifiedIncomingMessage()
    {
        return _magnifiedIncomingMessagePool.Get();
    }
    Message GetMagnifiedOutGoingMessage()
    {
        return _magnifiedOutgoingPool.Get();
    }


    void OnEnable()
    {
        OnAddMessageToScreen += AddMessageToScreen;
        OnAddMessageToLargeScreen += AddMessageToLargeScreen;
    }
    public override void OnDisablePhone()
    {
        OnAddMessageToScreen -= AddMessageToScreen;
        OnAddMessageToLargeScreen -= AddMessageToLargeScreen;
        if (_debug)
        {
            Debug.Log("Messenger App Disabled");
        }
    }

    private void AddMessageToScreen()
    {
        //* INCREMENT MESSAGES ON SCREEN
        _messagesOnScreen++;
        if (_messagesOnScreen >= _maxMessagesOnScreen)
        {
            float messageHeight = _incomingMessagePrefab.GetComponent<RectTransform>().rect.height;
            _messagesParent.transform.DOLocalMoveY(_messagesParent.transform.localPosition.y + messageHeight, 1f);
        }
    }

    private void AddMessageToLargeScreen()
    {
        _magnifiedMessagesOnScreen++;
        if (_magnifiedMessagesOnScreen >= _maxMagnifiedMessagesOnScreen)
        {
            float messageHeight = _magnifiedIncomingMessagePrefab.GetComponent<RectTransform>().rect.height;
            _magnifiedMessagesParent.transform.DOLocalMoveY(_magnifiedMessagesParent.transform.localPosition.y + messageHeight, 1f);
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

            // connection.OptionNode._effect = _optionId;
            // _optionId++;
        }
        //* UPDATE OPTIONS TEXT
        _optionsOneText.text = _activeNodeOptions.ElementAt(0).Text;
        _optionsTwoText.text = _activeNodeOptions.ElementAt(1).Text;

    }
    public string DisplayActiveSpeechNode()
    {
        // Message message = Instantiate(_newMessagePrefab, _messagesParent);

        Message message = GetIncomingMessagePrefab();
        message.transform.SetParent(_messagesParent);
        message.gameObject.SetActive(true);
        message.SetMessage(_activeNode.Text);
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

    private void OnCompleteChat()
    {
        Debug.Log("Completed chat");
    }

    private void EndConvo()
    {
        _optionsUi.SetActive(false);
        _endChatUi.SetActive(true);
        _canEndChat = true;
        _activeConvo.Complete();
        _activeConvo = null;
        _activeChat._hasPendingMessages = false;
        _optionId = 0;
        for (int i = 0; i < _chatEffects.Count; i++)
        {
            Debug.Log(_chatEffects.ElementAt(i));
        }
    }

    void ShowNextMessage()
    {

        //* GET SPEECH FOR SELECTED 
        SpeechConnection speechConnection = _lastSelectedOption.Connections.First(x => x.ConnectionType == Connection.eConnectionType.Speech) as SpeechConnection;

        //*UPDATE THE ACTIVE NODE TO SELECTED OPTIONS CONNECTED SPEECH
        _activeNode = speechConnection.SpeechNode;

        //* DISPLAY THE NEW SPEECH
        DisplayActiveSpeechNode();

        //* DISPLAY THE NEW MAGNIFIED SPEECH
        DisplayMagnifiedActiveSpeechNode();

        //* IF SPEECH HAS OPTIONS DISPLAY THEM
        if (ActiveSpeechHasOptions())
        {
            SetActiveOptions();
        }

        //* ALLOW PLAYER TO RESPOND AFTER MESSAGE IS DISPLAYED
        _canRespond = true;

        //* SET CONTENT OF SPEECH AS STRING FOR MESSAGE PREVIEW LATEST MESSAGE AS AI MESSAGE
        UpdateChatPreview(_activeNode.Text, false);
    }
    public void ReplyMessage(string text)
    {
        // Message message = Instantiate(_responsePrefab, _messagesParent);
        Message message = GetOutGoingMessagePrefab();
        message.gameObject.SetActive(true);
        message.SetMessage(text);


        OnAddMessageToScreen?.Invoke();
    }

    public void OpenChat(InstantMessage chat)
    {
        _activeChat = chat;
        _fullScreenMessageView.SetActive(true);
        if (chat._hasPendingMessages)
        {
            if (_debug)
            {

                Debug.Log($"<color=orange> Exposition not finished for selected convo {chat._messages[0]._content}</color> ");
            }
            SetUpOpenedMessage(chat._chat);
        }

        for (int i = 0; i < chat._messages.Count; i++)
        {
            MessageContentStruct message = chat._messages[i];
            Message messageItem;
            Message magnifiedMessage;
            if (!message._playerSentMessage)
            {
                messageItem = GetIncomingMessagePrefab();
                magnifiedMessage = GetMagnifiedIncomingMessage();

            }

            else
            {
                messageItem = GetOutGoingMessagePrefab();
                magnifiedMessage = GetMagnifiedOutGoingMessage();

                // OnAddMessageToScreen?.Invoke();
            }

            messageItem.SetMessage(message._content);
            magnifiedMessage.SetMessage(message._content);

            // if (_debug)
            // {
            //     Debug.Log($"Index: {i} IsPlayerMessage: {message._playerSentMessage} Content: {message._content}");
            // }

        }
        ShowChatUi();
    }

    public void ResetState()
    {
        Player.Instance._activeQuickAction = null;
    }
    private void ClearLatestExpandedChat()
    {
        for (int i = 0; i < _messagesParent.transform.childCount; i++)
        {
            Message message = _messagesParent.transform.GetChild(i).GetComponent<Message>();
            if (!message._isPlayerMessage)
            {
                _incomingMessagePool.Release(message);
            }
            else
            {
                _outGoingMessagePool.Release(message);
            }
        }
        for (int i = 0; i < _magnifiedMessagesParent.transform.childCount; i++)
        {
            Message message = _magnifiedMessagesParent.transform.GetChild(i).GetComponent<Message>();
            if (!message._isPlayerMessage)
            {
                _magnifiedIncomingMessagePool.Release(message);
            }
            else
            {
                _magnifiedOutgoingPool.Release(message);
            }
        }
        // _messagesParent.transform.localPosition = new(_messagesParent.transform.localPosition.x, 0, _messagesParent.transform.localPosition.z);
        _messagesParent.transform.localPosition = new Vector3(_messagesParent.transform.localPosition.x, 0f, _messagesParent.transform.localPosition.z);
        _messagesOnScreen = 0;
    }


    public void SetUpOpenedMessage(Chat convo)
    {
        Conversation conversation = convo.GetSpeechNodes();

        _rootNode = conversation.Root;
        _activeNode = _rootNode;

        _activeConvo = convo;


        if (_activeNode.Connections.Count > 0)
        {
            SetActiveOptions();
        }

        //* HANDLE START MAGNIFIED CONVO
        // StartMagnifiedConvo();

        //* ALLOW PLAYER TO RESPOND AFTER FIRST MESSAGE IS SHOWN
        _canRespond = true;

        _isTexting = true;

    }

    #region "Overrides"
    public override void OnBackPressed()
    {
        // This method can be overridden by derived classes to handle selection events
        Debug.Log("On Back pressed");
        if (!IsViewingExpandedChat)
        {
            Phone.Instance.GoToHomePage();
        }

        else
        {
            if (_canEndChat)
            {
                if (_magnifiedMessagesUi.gameObject.activeSelf)
                {
                    _magnifiedMessagesUi.gameObject.SetActive(false);
                }
                _fullScreenMessageView.SetActive(false);
                ClearLatestExpandedChat();
            }
        }

    }



    public override void OnAcceptPressed()
    {
        // This method can be overridden by derived classes to handle selection events
        Debug.Log("On Accept pressed");
        if (_isTexting)
        {
            ProgressConvo(0);
        }
    }
    public override void OnRejectPressed()
    {
        // This method can be overridden by derived classes to handle selection events
        Debug.Log("On Reject pressed");
        if (_isTexting)
        {
            ProgressConvo(1);
        }
    }

    public override void DoQuickAction()
    {
        _notificationSystem.HideNotification();
        StartConvo();
        ShowChatUi();
        Phone.Instance.SetApp(ID, true);
    }

    public override void OnInit()
    {
        _incomingMessagePool = new ObjectPool<Message>(
             () => Instantiate(_incomingMessagePrefab, _messagesParent),
             message =>
             {
                 message.gameObject.SetActive(true);
             },
             message => message.gameObject.SetActive(false),
             message => Destroy(message.gameObject),
             false, 10, 30
         );
        _outGoingMessagePool = new ObjectPool<Message>(
            () => Instantiate(_outgoingMessagePrefab, _messagesParent),
            message => message.gameObject.SetActive(true),
            message => message.gameObject.SetActive(false),
            message => Destroy(message.gameObject),
            false, 10, 30
        );

        _previewPool = new ObjectPool<Message>(
            () => Instantiate(_messagePreviewPrefab, _messagesPreviewParent),
            message => message.gameObject.SetActive(true),
            message => message.gameObject.SetActive(false),
            message => Destroy(message.gameObject),
            false, 10, 30
        );
        _magnifiedIncomingMessagePool = new ObjectPool<Message>(
            () => Instantiate(_magnifiedIncomingMessagePrefab, _magnifiedMessagesParent),
            message => message.gameObject.SetActive(true),
            message => message.gameObject.SetActive(false),
            message => Destroy(message.gameObject),
            false, 10, 30
        );
        _magnifiedOutgoingPool = new ObjectPool<Message>(
            () => Instantiate(_magnifiedOutgoingMessagePrefab, _magnifiedMessagesParent),
            message => message.gameObject.SetActive(true),
            message => message.gameObject.SetActive(false),
            message => Destroy(message.gameObject),
            false, 10, 30
        );

        // CalculateScreenSpace();
    }

    #endregion


    // [Button, TabGroup("Debug")]
    // void CalculateScreenSpace()
    // {
    //     RectTransform parentTransform = _messagesParent.GetComponent<RectTransform>();
    //     RectTransform messageTransform = _newMessagePrefab.GetComponent<RectTransform>();

    //     float parentHeight = parentTransform.rect.height;

    //     float messageHeight = messageTransform.rect.height;

    //     int messagesFit = Mathf.FloorToInt(parentHeight / messageHeight);
    //     Debug.Log($"Parent Height: {parentHeight}, Message Height: {messageHeight}, Messages that can fit on Screen: {messagesFit}");

    //     _maxMessagesOnScreen = messagesFit;
    // }

    //! EDITOR EVENT FUNCTION

    void CalculateScreenSpace()
    {
        RectTransform parentTransform = _magnifiedMessagesParent.GetComponent<RectTransform>();
        RectTransform messageTransform = _magnifiedIncomingMessagePrefab.GetComponent<RectTransform>();

        float parentHeight = parentTransform.rect.height;

        float messageHeight = messageTransform.rect.height;

        int messagesFit = Mathf.FloorToInt(parentHeight / messageHeight);
        if (_debug)
        {
            Debug.Log($"Parent Height: {parentHeight}, Message Height: {messageHeight}, Messages that can fit on Screen: {messagesFit}");
        }

        _maxMagnifiedMessagesOnScreen = messagesFit;
    }


    #region "Editor Event Functions"

    //! EDITOR EVENT FUNCTIONS
    public void ShowChatUi()
    {
        _fullScreenMessageView.SetActive(true);
        _magnifiedMessagesUi.gameObject.SetActive(true);
    }
    //! EDITOR EVENT FUNCTION
    public void SetActiveConvo(Chat convo)
    {
        _activeConvo = convo;
    }

    //! EDITOR EVENT FUNCTION
    [Button, TabGroup("Debug")]
    public void SetUpIm(Chat convo)
    {
        Conversation conversation = convo.GetSpeechNodes();

        _rootNode = conversation.Root;
        _activeNode = _rootNode;

        SetActiveConvo(convo);
        // GetActiveSpeechNode();
        if (_activeNode.Connections.Count > 0)
        {
            SetActiveOptions();
        }

        //* CREATE FIRST MESSAGE PREVIEW
        AddChatToPreviewList(convo);
        //* SHOW NOTIFICATION OF NEW CONVO
        UseQuickAction();
        _notificationSystem.ShowNotification(AppIcon.IconSprite, _activeNode.Text, "Reply", ResetState);
        // convo.Oncomplete += OnCompleteChat;
    }

    //! EDITOR EVENT FUNCTION
    public void StartConvo()
    {
        //* HANDLE START MAGNIFIED CONVO
        StartMagnifiedConvo();

        //* ADD FIRST MESSAGE TO REGULAR CHAT
        // Message message = Instantiate(_newMessagePrefab, _messagesParent);

        Message message = GetIncomingMessagePrefab();

        message.gameObject.SetActive(true);
        message.SetMessage(_activeNode.Text);
        OnAddMessageToScreen?.Invoke();
        //* ALLOW PLAYER TO RESPOND AFTER FIRST MESSAGE IS SHOWN
        _canRespond = true;

        //* SET ACTIVE APP ON PHONE TO MESSENGER APP
        Phone.Instance.SetApp(ID);

        _isTexting = true;
    }

    //! EDITOR EVENT FUNCTION
    [Button, TabGroup("Debug")]
    public void ProgressConvo(int choice)
    {
        if (!_canRespond) return;
        OptionNode chosenOption = _activeNodeOptions.ElementAt(choice);

        if (_debug)
        {
            Debug.Log($"Tied Effect id {chosenOption._effect} Option text {chosenOption.Text}");
        }
        // Debug.Log($"Selected {_activeNodeOptions.ElementAt(choice).Text}");

        _chatEffects.Add(chosenOption._effect);

        ReplyMessage(chosenOption.Text);
        ShowMagnifiedReply(chosenOption.Text);

        //* SET CONTENT OF USER REPLY AS STRING FOR MESSAGE PREVIEW LATEST MESSAGE AS USER MESSAGE
        UpdateChatPreview(chosenOption.Text, true);

        //* Set LAST SELECTED OPTION TO OPTION AT INDEX OF CHOICE
        _lastSelectedOption = chosenOption;

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

    #endregion

    #region "Debug"
    [Button, TabGroup("Debug")]
    public void DebugScreenMovement()
    {
        float messageHeight = _incomingMessagePrefab.GetComponent<RectTransform>().rect.height;
        _messagesParent.transform.localPosition = new Vector3(_messagesParent.transform.localPosition.x, _messagesParent.transform.localPosition.y + messageHeight, _messagesParent.transform.localPosition.z);
        Debug.Log(messageHeight);
    }

    #endregion

    #region "Magnified messages Handling"
    public void StartMagnifiedConvo()
    {
        Message message = GetMagnifiedIncomingMessage();
        message.SetMessage(_activeNode.Text);
        OnAddMessageToLargeScreen?.Invoke();
    }


    public string DisplayMagnifiedActiveSpeechNode()
    {
        // Message message = Instantiate(_newMagnifiedMessagePrefab, _magnifiedMessagesParent);
        Message message = GetMagnifiedIncomingMessage();
        // message.gameObject.SetActive(true);
        message.SetMessage(_activeNode.Text);
        OnAddMessageToLargeScreen?.Invoke();
        return _activeNode.Text;
    }
    public void ShowMagnifiedReply(string text)
    {
        // Message message = Instantiate(_magnifiedResponsePrefab, _magnifiedMessagesParent);

        Message message = GetMagnifiedOutGoingMessage();
        message.SetMessage(text);

        // message.gameObject.SetActive(true);

        OnAddMessageToLargeScreen?.Invoke();
        _optionsOneText.text = "";
        _optionsTwoText.text = "";
    }
    #endregion

    #region "MessagePreview Handling"

    [Button, TabGroup("Debug")]
    public void AddChatToPreviewList(Chat convo)
    {
        Conversation conversation = convo.GetSpeechNodes();
        // Message messagePreview = Instantiate(_messagePreviewPrefab, _messagesPreviewParent);

        Message messagePreview = GetPreview();

        //* ACTIVATE DIVIDER UNDER MESSAGE FOR UI PURPOSE
        messagePreview.UseUnderLine();
        //* FIRST  MESSAGE IN CHAT ALWAYS AI MESSAGE 
        InstantMessage chat = new(convo, convo._senderName, conversation.Root.Text, messagePreview);

        //* SET LATEST RECEIVED MESSAGE AS ACTIVE CHAT TO UPDATE WHEN ANY NEW MESSAGES COME IN
        _activeChat = chat;

        //* ADD INSTANT MESSAGE TO USER MESSAGES LIST
        chats.Add(chat);

        //* UPDATE ID OF INSTANT MESSAGE TO CURRENT LENGTH OF CHATS AFTER ADDING
        _activeChat.SetID(chats.Count - 1);

        //* MAKE MESSAGE PREFAB BUTTON OPEN INSTANT MESSAGE CORRESPONDING TO ID
        AddExpandFuncToButton(messagePreview, chats.Count - 1);
    }


    public void UpdateChatPreview(string latestMessage, bool isPlayerText)
    {
        _activeChat.UpdateChat(latestMessage, isPlayerText);
    }

    public bool ChatExists()
    {
        return false;
    }

    void AddExpandFuncToButton(Message message, int chatIndex)
    {
        message.GetComponent<Button>().onClick.AddListener(() =>
        {
            OpenChat(chatIndex);
        });
    }

    private void OpenChat(int chatIndex)
    {
        InstantMessage instantMessage = chats[chatIndex];
        OpenChat(instantMessage);
    }

    #endregion
}
