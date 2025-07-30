using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct ItemSpawnStruct
{
    public Pos _spawnPos;
    public GameObject _prefab;

    public ItemSpawnStruct(Pos spawnPos, GameObject prefab)
    {
        _spawnPos = spawnPos;
        _prefab = prefab;
    }
}

public enum OperationLifeCycle
{
    ON_AWAKE = 0,
    ON_START = 1,
    IN_QUEST = 2,
    ON_QUEST_END = 3
}
[Serializable]
public abstract class QuestOperation
{

    public OperationLifeCycle _operationLifeCycle;
    public bool _debug = false;
    public abstract void Execute(QuestStep questStep);

}

[Serializable]
public class InitQuestItems : QuestOperation
{
    [SerializeField] List<QuestItemStruct> _questItemsData = new();

    public override void Execute(QuestStep questStep)
    {
        for (int i = 0; i < _questItemsData.Count; i++)
        {
            questStep.AddQuestItem(_questItemsData[i]);
            if (_debug)
            {
                Debug.Log($"<color=cyan>Initializing Quest Item {_questItemsData[i]._name} </color>");
            }
        }
    }
}
[Serializable]
public class SpawnQuestItems : QuestOperation
{
    [SerializeField] List<ItemSpawnStruct> _spawns = new();

    public override void Execute(QuestStep questStep)
    {
        for (int i = 0; i < _spawns.Count; i++)
        {
            GameObject obj = GameObject.Instantiate(_spawns[i]._prefab, _spawns[i]._spawnPos.position, Quaternion.Euler(_spawns[i]._spawnPos.rotation), questStep.transform);
            obj.transform.SetLocalPositionAndRotation(_spawns[i]._spawnPos.position, Quaternion.Euler(_spawns[i]._spawnPos.rotation));
            if (_debug)
            {
                Debug.Log($"Spawned {_spawns[i]._prefab.name} at {_spawns[i]._spawnPos.position}");
            }
        }
    }
}

