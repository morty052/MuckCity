using System;
using System.Collections.Generic;
using UnityEngine;

public enum ScriptedEventLifecycle
{
    ON_START = 0,
    ON_RETRIEVE = 1,
    ON_COMPLETE = 2
}

[Serializable]
public abstract class ScriptedEvent
{
    public ScriptedEventLifecycle _eventLifecycle;
    public abstract void SetUp(DelveManager delveManager, DelveSO delveSO);
}

[Serializable]
public class SpawnNpc : ScriptedEvent
{
    public List<SpawnStruct> _spawnData;

    public override void SetUp(DelveManager delveManager, DelveSO delveSO)
    {
        Debug.Log("Initializing Npcs");
        for (int i = 0; i < _spawnData.Count; i++)
        {
            SpawnStruct spawnStruct = _spawnData[i];
            // Instantiate(spawnStruct._npc, spawnStruct._location.position, Quaternion.Euler(spawnStruct._location.rotation));
            NpcManager.Instance.SpawnNPC(spawnStruct._npc, spawnStruct._location);
        }
    }
}
[Serializable]
public class InitPointOfInterest : ScriptedEvent
{
    public Pos _location;

    public override void SetUp(DelveManager delveManager, DelveSO delveSO)
    {
        Debug.Log("Initializing waypoint to point of interest");
        Waypoint.Instance.Init(_location.position);
    }
}

public class SpawnDelveItem : ScriptedEvent
{
    [SerializeField] DelveItem _delveItem;
    public Pos _location;

    public override void SetUp(DelveManager delveManager, DelveSO delveSO)
    {
        Debug.Log("Initialized Contract Delve");
        DelveItem item = GameObject.Instantiate(_delveItem, _location.position, Quaternion.Euler(_location.rotation));
        item._id = delveSO._id;
    }
}
