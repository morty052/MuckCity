using System.Collections.Generic;
using DialogueEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "Chat", menuName = "ScriptableObjects/Phone/Chat")]
public class Chat : ScriptableObject
{
    public List<InstantMessage> _dialogue;

    public NPCConversation _convo;


    public Conversation GetSpeechNodes()
    {
        return _convo.Deserialize();
    }

}
