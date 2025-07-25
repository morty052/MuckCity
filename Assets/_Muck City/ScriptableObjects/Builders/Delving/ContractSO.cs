using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Utilities.Editor;
using UnityEngine;

[CreateAssetMenu(fileName = "Contract", menuName = "ScriptableObjects/Delving/Contract", order = 1)]
public class ContractSO : ScriptableObject
{
    public int _bounty;
    public string _name;
    public string _description;
    public Sprite _sprite;
    public DelveItem _delveItem;

    public Locations _keyLocation;

    public Pos _itemSpawnPos;

    public string _id;
    [SerializeReference] public List<ScriptedEvent> _events;

    [Button("Generate ID")]
    public void GenerateID()
    {
        _id = name[..3] + System.Guid.NewGuid().ToString()[..4];
    }


}
