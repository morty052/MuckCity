using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

[Serializable]
public abstract class ScriptedEvent
{
    public abstract void SetUp();
}

[Serializable]
public class SpawnNpc : ScriptedEvent
{
    public List<SpawnStruct> _spawnData;

    public override void SetUp()
    {
        Debug.Log("Initializing Waypoint");
        for (int i = 0; i < _spawnData.Count; i++)
        {
            SpawnStruct spawnStruct = _spawnData[i];
            // Instantiate(spawnStruct._npc, spawnStruct._location.position, Quaternion.Euler(spawnStruct._location.rotation));
            NpcManager.Instance.SpawnNPC(spawnStruct._npc, spawnStruct._location);
        }
    }
}

public class DelveManager : MonoBehaviour
{
    public static DelveManager Instance { get; private set; }

    HashSet<BountySO> _activeBounties = new();
    HashSet<ContractSO> _activeContracts = new();
    HashSet<ContractSO> _activeRetrievals = new();
    public HashSet<ContractSO> Retrievals { get => _activeRetrievals; }
    public HashSet<ContractSO> Contracts { get => _activeContracts; }
    public HashSet<BountySO> Bounties { get => _activeBounties; }

    public DelveTicket _activeDelveTicket;


    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void OnEnable()
    {
        GameEventsManager.OnAcceptBountyEvent += OnAcceptBounty;
        GameEventsManager.OnAcceptContractEvent += OnAcceptContract;
        GameEventsManager.OnDepositDelveItemEvent += OnDepositDelveItem;
    }

    void OnDisable()
    {
        GameEventsManager.OnAcceptBountyEvent -= OnAcceptBounty;
        GameEventsManager.OnAcceptContractEvent -= OnAcceptContract;
        GameEventsManager.OnDepositDelveItemEvent -= OnDepositDelveItem;
    }

    private void OnDepositDelveItem(ContractSO sO)
    {
        _activeRetrievals.Remove(sO);
    }

    public bool PlayerHasDelveTicket()
    {
        return _activeDelveTicket != null;
    }

    [Button("Save")]
    void SaveDelves()
    {
        ES3.Save("Bounties", _activeBounties);
        ES3.Save("Contracts", _activeContracts);
    }


    public void OnAcceptBounty(BountySO bountySO)
    {
        Debug.Log("Accepted Bounty Delve");
        _activeBounties.Add(bountySO);
    }
    public void OnAcceptContract(ContractSO contractSO)
    {
        _activeContracts.Add(contractSO);
        InitContract(contractSO);
    }

    void InitContract(ContractSO contractSO)
    {
        Debug.Log("Initialized Contract Delve");
        DelveItem item = Instantiate(contractSO._delveItem, contractSO._itemSpawnPos.position, Quaternion.Euler(contractSO._itemSpawnPos.rotation));
        item._id = contractSO._id;
        for (int i = 0; i < contractSO._events.Count; i++)
        {
            contractSO._events[i].SetUp();
        }
    }

    public void OnRetrieveDelveItem(string id)
    {
        ContractSO contractSO = _activeContracts.FirstOrDefault(x => x._id == id);
        _activeRetrievals.Add(contractSO);
        // GameEventsManager.OnRetrieveDelveItem?.Invoke(contractSO);
        Debug.Log("Retreived " + contractSO.name);
    }
}
