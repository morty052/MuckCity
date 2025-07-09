using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public struct SpawnStruct
{
    public Pos _location;
    public NpcSO _npc;

    public SpawnStruct(NpcSO npcList, Pos location)
    {
        _npc = npcList;
        _location = location;
    }
}

[CreateAssetMenu(fileName = "NpcSpawnData", menuName = "ScriptableObjects/NpcSpawnData", order = 1)]
public class NpcSpawnData : ScriptableObject
{
    public List<SpawnStruct> _data;
}
