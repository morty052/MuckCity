using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityUtils;

[Serializable]
public struct NpcList
{
    public Locations _location;
    public List<NpcSO> _npcList;

    public NpcList(List<NpcSO> npcList, Locations location)
    {
        _npcList = npcList;
        _location = location;
    }
}

public class NpcLoader : MonoBehaviour
{
    [SerializeField] List<NpcList> _npcLists = new();

    public Task SpawnNpcList(Locations location, Transform spawnParent)
    {
        NpcList npcList = _npcLists.Find(x => x._location == location);

        foreach (NpcSO npc in npcList._npcList)
        {
            NpcCharacter npcCharacter = Instantiate(npc._npcPrefab, npc._spawnPosition, Quaternion.Euler(npc._spawnRotation), spawnParent);
            NpcManager.Instance.AddNPC(npcCharacter);
            // OnScreenDebugger.Instance.Log($"Spawned {npcCharacter.name}");
        }

        return Task.CompletedTask;
    }


}
