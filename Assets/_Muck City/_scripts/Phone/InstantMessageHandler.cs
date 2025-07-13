using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using DialogueEditor;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityUtils;




public class InstantMessageHandler : MonoBehaviour
{

    [TabGroup("Debug")] public Chat _activeConvo;

    [TabGroup("Debug")] public int _magnifiedMessagesOnScreen = 0;
    [TabGroup("Debug")] public bool _canRespond = false;


    //*
    public SpeechNode _rootNode;
    public ConversationNode _activeNode;
    private HashSet<OptionNode> _activeNodeOptions = new();
    private OptionNode _lastSelectedOption;

    [SerializeField, TabGroup("Debug")] private bool _debug;

    #region "Magnified Messages"
    Action OnAddMessageToLargeScreen;

    [TabGroup("Components")] public Transform _magnifiedMessagesParent;

    [TabGroup("Components")]
    [SerializeField, TabGroup("Components")] TextMeshProUGUI _optionsOneText;

    [SerializeField, TabGroup("Components")] TextMeshProUGUI _optionsTwoText;

    [TabGroup("Components")] public Message _newMagnifiedMessagePrefab;
    [TabGroup("Components")] public Message _magnifiedResponsePrefab;

    [TabGroup("Settings")] public int _maxMagnifiedMessagesOnScreen = 0;

    #endregion
    // void Awake()
    // {
    //     CalculateScreenSpace();
    // }

    // void OnEnable()
    // {
    //     OnAddMessageToLargeScreen += AddMessageToScreen;
    // }
    // void OnDisable()
    // {
    //     OnAddMessageToLargeScreen -= AddMessageToScreen;
    // }

    private void AddMessageToScreen()
    {
        //* INCREMENT MESSAGES ON SCREEN
        _magnifiedMessagesOnScreen++;
        if (_magnifiedMessagesOnScreen >= _maxMagnifiedMessagesOnScreen)
        {
            float messageHeight = _newMagnifiedMessagePrefab.GetComponent<RectTransform>().rect.height;
            _magnifiedMessagesParent.transform.DOLocalMoveY(_magnifiedMessagesParent.transform.localPosition.y + messageHeight, 1f);
        }
    }


    [Button, TabGroup("Debug")]
    void CalculateScreenSpace()
    {
        RectTransform parentTransform = _magnifiedMessagesParent.GetComponent<RectTransform>();
        RectTransform messageTransform = _newMagnifiedMessagePrefab.GetComponent<RectTransform>();

        float parentHeight = parentTransform.rect.height;

        float messageHeight = messageTransform.rect.height;

        int messagesFit = Mathf.FloorToInt(parentHeight / messageHeight);
        if (_debug)
        {
            Debug.Log($"Parent Height: {parentHeight}, Message Height: {messageHeight}, Messages that can fit on Screen: {messagesFit}");
        }

        _maxMagnifiedMessagesOnScreen = messagesFit;
    }

    //! EDITOR EVENT FUNCTION
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

        //* UPDATE OPTIONS TEXT
        _optionsOneText.text = _activeNodeOptions.ElementAt(0).Text;
        _optionsTwoText.text = _activeNodeOptions.ElementAt(1).Text;
    }

    public string DisplayActiveSpeechNode()
    {
        Message message = Instantiate(_newMagnifiedMessagePrefab, _magnifiedMessagesParent);
        message.gameObject.SetActive(true);
        message._content.text = _activeNode.Text;
        OnAddMessageToLargeScreen?.Invoke();
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

    //! EDITOR EVENT FUNCTION
    public void SetActiveConvo(Chat convo)
    {
        _activeConvo = convo;
    }

    //! EDITOR EVENT FUNCTION
    public void StartConvo()
    {
        Message message = Instantiate(_newMagnifiedMessagePrefab, _magnifiedMessagesParent);
        message.gameObject.SetActive(true);
        message._content.text = _activeNode.Text;
        OnAddMessageToLargeScreen?.Invoke();
        if (ActiveSpeechHasOptions())
        {
            _optionsOneText.text = _activeNodeOptions.ElementAt(0).Text;
            _optionsTwoText.text = _activeNodeOptions.ElementAt(1).Text;
        }

        //* ALLOW PLAYER TO RESPOND AFTER FIRST MESSAGE IS SHOWN
        _canRespond = true;

    }

    //! EDITOR EVENT FUNCTION
    [Button, TabGroup("Debug")]
    public void ProgressConvo(int choice)
    {
        if (!_canRespond) return;

        OptionNode chosenOption = _activeNodeOptions.ElementAt(choice);
        //* DISPLAY USERS CHOSEN OPTION IN CHAT AS NEW TEXT
        ReplyMessage(chosenOption.Text);

        //* BROADCAST USERS CHOSEN OPTION AS STRING
        // OnUpdateChat?.Invoke(chosenOption.Text, true);

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

        //* BROADCAST CONTENT OF SPEECH AS STRING
        // OnUpdateChat?.Invoke(_activeNode.Text, false);
    }


    public void ReplyMessage(string text)
    {
        Message message = Instantiate(_magnifiedResponsePrefab, _magnifiedMessagesParent);
        message._content.text = text;

        message.gameObject.SetActive(true);

        OnAddMessageToLargeScreen?.Invoke();
        _optionsOneText.text = "";
        _optionsTwoText.text = "";
    }
}
