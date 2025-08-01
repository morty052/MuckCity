using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class DelveSO : ScriptableObject
{
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
}
