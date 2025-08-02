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
    [TabGroup("Data")] public DelveType _delveType;
    [TabGroup("Data")] public int _bounty;
    [TabGroup("Data")] public string _name;
    [TabGroup("Data")] public string _description;
    [TabGroup("Data")] public Sprite _sprite;

    [TabGroup("Data")] public RealmID _tiedRealm;
    [TabGroup("Data")] public string _id;

    [SerializeReference, TabGroup("On Accept")] public List<ScriptedEvent> _OnAccept;
    [SerializeReference, TabGroup("On Enter Realm")] public List<ScriptedEvent> _OnEnterRealm;
    [SerializeReference, TabGroup("Retrieve")] public List<ScriptedEvent> _onRetrieveEvents;
    [SerializeReference, TabGroup("Deposit")] public List<ScriptedEvent> _onDepositEvents;





    [TabGroup("Funcs"), Button("Generate ID")]
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
