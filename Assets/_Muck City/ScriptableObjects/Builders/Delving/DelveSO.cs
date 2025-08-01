using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using Unity.VisualScripting;
using UnityEngine;

public enum DelveType
{
    CONTRACT,
    BOUNTY
}

public class DelveSO : ScriptableObject
{
    public DelveType _delveType;
    public int _bounty;
    public string _name;
    public string _description;
    public Sprite _sprite;

    public RealmID _tiedRealm;
    [SerializeReference] public List<ScriptedEvent> _events;

    public string _id;


    [Button("Generate ID")]
    public void GenerateID()
    {
        _id = name[..3] + System.Guid.NewGuid().ToString()[..4];
    }

    public string GetCleanNameFromEnum()
    {
        string[] splitName = _tiedRealm.ToString().Split("_");
        if (splitName.Length > 1)
        {
            return string.Join(" ", splitName.Select(n => n.ToLower().FirstCharacterToUpper()));
        }
        else
        {
            return splitName[0].FirstCharacterToUpper();
        }
    }
}
