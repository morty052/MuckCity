using System;
using System.Collections.Generic;
using System.Linq;
using DialogueEditor;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;



public class MessengerHomeScreen : MonoBehaviour
{
    public Transform _messagesParent;
    public Message _messagePreviewPrefab;
    public List<InstantMessage> chats;

    public InstantMessage _activeChat;
    MessengerApp _messengerApp;

    void Awake()
    {
        _messengerApp = GetComponentInParent<MessengerApp>();
    }

    //! EDITOR EVENT FUNCTION
    [Button, TabGroup("Debug")]
    public void AddChatToList(Chat convo)
    {
        Conversation conversation = convo.GetSpeechNodes();
        Message messagePreview = Instantiate(_messagePreviewPrefab, _messagesParent);

        //* ACTIVATE DIVIDER UNDER MESSAGE FOR UI PURPOSE
        messagePreview.UseUnderLine();
        //* FIRST  MESSAGE IN CHAT ALWAYS AI MESSAGE 
        InstantMessage chat = new(convo._senderName, conversation.Root.Text, messagePreview);

        //* SET LATEST RECEIVED MESSAGE AS ACTIVE CHAT TO UPDATE WHEN ANY NEW MESSAGES COME IN
        _activeChat = chat;

        //* ADD INSTANT MESSAGE TO USER MESSAGES LIST
        chats.Add(chat);

        //* UPDATE ID OF INSTANT MESSAGE TO CURRENT LENGTH OF CHATS AFTER ADDING
        _activeChat.SetID(chats.Count - 1);

        //* MAKE MESSAGE PREFAB BUTTON OPEN INSTANT MESSAGE CORRESPONDING TO ID
        AddExpandFuncToButton(messagePreview, chats.Count - 1);
    }

    //! EDITOR EVENT FUNCTION
    public void UpdateChat(string latestMessage, bool isPlayerText)
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
        _messengerApp.OpenChat(instantMessage);
    }

}
