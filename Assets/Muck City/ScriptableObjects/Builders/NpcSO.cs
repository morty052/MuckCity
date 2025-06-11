using DialogueEditor;
using UnityEngine;
using System.Collections.Generic;
using DG.Tweening;



[CreateAssetMenu(fileName = "Npc", menuName = "ScriptableObjects/NewNpc/Npc", order = 1)]
public class NpcSO : ScriptableObject
{

    public SpecialCharacters _id;
    public string _name;
    public NpcCharacter _npcPrefab;
    public List<Role> _roles = new();
    public Locations _primaryLocation;
    [SerializeField] NPCConversation _defaultConversation;

    public Vector3 _spawnPosition;
    public Vector3 _spawnRotation;
    private void OnValidate()
    {
#if UNITY_EDITOR
        _name = this.name;
        UnityEditor.EditorUtility.SetDirty(this);
#endif
    }
}
