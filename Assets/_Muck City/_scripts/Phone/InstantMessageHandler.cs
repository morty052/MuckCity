using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using DialogueEditor;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityUtils;

[System.Serializable]
public struct InstantMessage
{
    public string _message;
    public string _optionOne;
    public string _optionTwo;
    public InstantMessage(string message, string optionOne, string optionTwo)
    {
        _message = message;
        _optionOne = optionOne;
        _optionTwo = optionTwo;
    }
}
[System.Serializable]
public class IM
{
    public SpeechNode _rootNode;
    public ConversationNode _activeNode;
    private HashSet<OptionNode> _activeNodeOptions;
    private OptionNode _lastSelectedNode;
    public IM(SpeechNode message)
    {
        _rootNode = message;
        _activeNodeOptions = new();
    }

}

public class InstantMessageHandler : MonoBehaviour
{


    [TabGroup("Components")] public Transform _messagesParent;

    [TabGroup("Components")]
    [SerializeField, TabGroup("Components")] TextMeshProUGUI _optionsOneText;


    [SerializeField, TabGroup("Components")] TextMeshProUGUI _optionsTwoText;


    [TabGroup("Components")] public Message _newMessagePrefab;
    [TabGroup("Components")] public Message _responsePrefab;

    [TabGroup("Settings")] public int _maxMessagesOnScreen = 0;
    [TabGroup("Debug")] public Chat _activeConvo;

    [TabGroup("Debug")] public int _messagesOnScreen = 0;


    //*
    public SpeechNode _rootNode;
    public ConversationNode _activeNode;
    private HashSet<OptionNode> _activeNodeOptions = new();
    private OptionNode _lastSelectedOption;

    Action OnAddMessageToScreen;

    void Awake()
    {
        CalculateScreenSpace();
    }

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

        //* UPDATE OPTIONS TEXT
        _optionsOneText.text = _activeNodeOptions.ElementAt(0).Text;
        _optionsTwoText.text = _activeNodeOptions.ElementAt(1).Text;
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
        if (ActiveSpeechHasOptions())
        {
            _optionsOneText.text = _activeNodeOptions.ElementAt(0).Text;
            _optionsTwoText.text = _activeNodeOptions.ElementAt(1).Text;
        }

    }

    [Button, TabGroup("Debug")]
    public void ProgressConvo(int choice)
    {
        // Debug.Log($"Selected {_activeNodeOptions.ElementAt(choice).Text}");

        ReplyMessage(_activeNodeOptions.ElementAt(choice).Text);

        //* Set LAST SELECTED OPTION TO OPTION AT INDEX OF CHOICE
        _lastSelectedOption = _activeNodeOptions.ElementAt(choice);

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
    }
    public void ReplyMessage(string text)
    {
        Message message = Instantiate(_responsePrefab, _messagesParent);
        message._content.text = text;

        message.gameObject.SetActive(true);

        OnAddMessageToScreen?.Invoke();
        _optionsOneText.text = "";
        _optionsTwoText.text = "";
    }
}
