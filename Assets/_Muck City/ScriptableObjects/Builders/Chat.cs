using System;
using System.Collections.Generic;
using DialogueEditor;
using UnityEngine;



[CreateAssetMenu(fileName = "Chat", menuName = "ScriptableObjects/Phone/Chat")]
public class Chat : ScriptableObject
{
    public string _senderName;
    public NPCConversation _convo;

    public Action Oncomplete;

    [SerializeReference] public List<DecisionEffect> _effects;

    public void Complete()
    {
        Oncomplete?.Invoke();
    }

    public Conversation GetSpeechNodes()
    {
        return _convo.Deserialize();
    }

}
