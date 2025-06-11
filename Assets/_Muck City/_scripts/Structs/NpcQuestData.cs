using UnityEngine;
using DialogueEditor;

[System.Serializable]
public struct NpcQuestData
{

    public SpecialCharacters _characterID;
    public NPCConversation _conversationForQuest;

    public NpcSO _npcSO;

    public Pos _specialPosition;

    // Update is called once per frame
    public NpcQuestData(NPCConversation conversationForQuest, SpecialCharacters characterID, Pos specialPosition, NpcSO npcSO)
    {
        _conversationForQuest = conversationForQuest;
        _characterID = characterID;
        _specialPosition = specialPosition;
        _npcSO = npcSO;
    }
}
