using System;
using System.Collections.Generic;
using DialogueEditor;
using Sirenix.OdinInspector;
using UnityEngine;



[CreateAssetMenu(fileName = "Chat", menuName = "ScriptableObjects/Phone/Chat")]
public class Chat : ScriptableObject
{
    public string _senderName;
    public NPCConversation _convo;

    public Action Oncomplete;

    [SerializeReference] public List<DecisionEffect> _effects;

    [ShowInInspector] private Conversation _conversation;

    void OnEnable()
    {
        if (_convo != null && _conversation == null)
        {
            _conversation = _convo.Deserialize();
        }
    }

    public void Complete()
    {
        Oncomplete?.Invoke();
    }

    public Conversation GetSpeechNodes()
    {
        return _conversation;
    }

    [Button]
    public void SetSpeechNodes()
    {
        _conversation = _convo.Deserialize();
    }

}
