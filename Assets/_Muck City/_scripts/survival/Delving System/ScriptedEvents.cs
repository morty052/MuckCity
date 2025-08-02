using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public enum ScriptedEventLifecycle
{
    ON_ACCEPT = 0,
    ON_ENTER_REALM = 1,
    ON_RETRIEVE = 2,
    ON_COMPLETE = 3
}

[Serializable]
public abstract class ScriptedEvent
{
    public ScriptedEventLifecycle _eventLifecycle;
    public float _executionDelay = 0;
    public abstract void Execute(DelveManager delveManager, DelveSO delveSO);
    public abstract Task DelayedExecute(DelveManager delveManager, DelveSO delveSO);

}

public class SpawnNpc : ScriptedEvent
{
    public List<SpawnStruct> _spawnData;

    public override async Task DelayedExecute(DelveManager delveManager, DelveSO delveSO)
    {
        await Task.Delay((int)(_executionDelay * 1000));
        Execute(delveManager, delveSO);
    }

    public override void Execute(DelveManager delveManager, DelveSO delveSO)
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

public class InitPointOfInterest : ScriptedEvent
{
    public Pos _location;

    public override async Task DelayedExecute(DelveManager delveManager, DelveSO delveSO)
    {
        await Task.Delay((int)(_executionDelay * 1000));
        Execute(delveManager, delveSO);
    }

    public override void Execute(DelveManager delveManager, DelveSO delveSO)
    {
        Debug.Log("Initializing waypoint to point of interest");
        Waypoint.Instance.Init(_location.position);
    }
}

public class SpawnDelveItem : ScriptedEvent
{
    [SerializeField] DelveItem _delveItem;
    public Pos _location;

    public override void Execute(DelveManager delveManager, DelveSO delveSO)
    {
        Debug.Log("Initialized Contract Delve");
        DelveItem item = GameObject.Instantiate(_delveItem, _location.position, Quaternion.Euler(_location.rotation));
        item._id = delveSO._id;
    }

    public override async Task DelayedExecute(DelveManager delveManager, DelveSO delveSO)
    {
        await Task.Delay((int)(_executionDelay * 1000));
        Execute(delveManager, delveSO);
    }
}

public class InteractWithCompad : ScriptedEvent
{
    public PhoneTriggerType phoneTriggerType;

    public Chat _chat;

    public override void Execute(DelveManager delveManager, DelveSO delveSO)
    {
        if (phoneTriggerType == PhoneTriggerType.MESSAGE)
        {
            Phone.Instance.ReceiveInstantMessage(_chat);
        }
    }

    public override async Task DelayedExecute(DelveManager delveManager, DelveSO delveSO)
    {
        await Task.Delay((int)(_executionDelay * 1000));
        Execute(delveManager, delveSO);
    }
}
public class PlayTimeLine : ScriptedEvent
{
    public TimelinePlayer _timeLinePlayerPrefab;

    public Pos _spawnPos;

    public bool _playOnAwake;


    public override void Execute(DelveManager delveManager, DelveSO delveSO)
    {
        TimelinePlayer timelinePlayer = GameObject.Instantiate(_timeLinePlayerPrefab, _spawnPos.position, Quaternion.Euler(_spawnPos.rotation));
        timelinePlayer._enablePlayOnAwake = _playOnAwake;
    }

    public override async Task DelayedExecute(DelveManager delveManager, DelveSO delveSO)
    {
        await Task.Delay((int)(_executionDelay * 1000));
        Execute(delveManager, delveSO);
    }
}
