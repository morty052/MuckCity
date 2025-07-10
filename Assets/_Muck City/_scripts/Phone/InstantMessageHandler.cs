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

public class InstantMessageHandler : MonoBehaviour
{
    public Chat _activeConvo;
    public Transform _messagesParent;

    [SerializeField] TextMeshProUGUI _optionsOneText;
    [SerializeField] TextMeshProUGUI _optionsTwoText;
    public Message _newMessagePrefab;
    public Message _responsePrefab;

    public int _activeTextIndex = 0;


    public void SetActiveConvo(Chat convo)
    {
        _activeConvo = convo;
    }

    public void StartConvo()
    {
        Message message = Instantiate(_newMessagePrefab, _messagesParent);
        message.gameObject.SetActive(true);
        message._content.text = _activeConvo._dialogue[0]._message;
        _optionsOneText.text = _activeConvo._dialogue[0]._optionOne;
        _optionsTwoText.text = _activeConvo._dialogue[0]._optionTwo;

    }
    public void ReplyMessage(int choice)
    {
        Message message = Instantiate(_responsePrefab, _messagesParent);
        if (choice == 0)
        {
            message._content.text = _activeConvo._dialogue[_activeTextIndex]._optionOne;
        }

        else
        {
            message._content.text = _activeConvo._dialogue[_activeTextIndex]._optionTwo;
        }

        message.gameObject.SetActive(true);
        _optionsOneText.text = "";
        _optionsTwoText.text = "";
    }
}
