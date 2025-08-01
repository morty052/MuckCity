using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using Systems.SceneManagement;
using UnityEngine;



public class DelveManager : MonoBehaviour
{
    public static DelveManager Instance { get; private set; }

    HashSet<BountySO> _activeBounties = new();
    HashSet<ContractSO> _activeContracts = new();
    HashSet<DelveSO> _activeRetrievals = new();
    public HashSet<DelveSO> Retrievals { get => _activeRetrievals; }
    // public HashSet<ContractSO> Contracts { get => _activeContracts; }
    // public HashSet<BountySO> Bounties { get => _activeBounties; }

    public DelveTicket _activeDelveTicket;

    HashSet<ContractSO> _contractsThatNeedInit = new();
    HashSet<BountySO> _bountiesThatNeedInit = new();




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
        SceneLoader.OnSceneGroupLoaded += OnSceneGroupLoaded;
    }

    void OnDisable()
    {
        GameEventsManager.OnAcceptBountyEvent -= OnAcceptBounty;
        GameEventsManager.OnAcceptContractEvent -= OnAcceptContract;
        GameEventsManager.OnDepositDelveItemEvent -= OnDepositDelveItem;
        SceneLoader.OnSceneGroupLoaded -= OnSceneGroupLoaded;
    }

    #region"Events driven funcs"

    private void OnSceneGroupLoaded(SceneGroup sceneGroup)
    {
        if (_contractsThatNeedInit.Count > 0)
        {

            for (int i = 0; i < _contractsThatNeedInit.Count; i++)
            {
                string realmScene = sceneGroup.FindSceneByName(SceneType.Realm);
                // Debug.Log("Delve manager noticed sceneGroup loaded with realm scene " + realmScene + " and contract " + _contractsThatNeedInit.ElementAt(i).GetCleanNameFromEnum());
                if (_contractsThatNeedInit.ElementAt(i).GetCleanNameFromEnum() == realmScene)
                {
                    FireScriptedEventsOnStart(_contractsThatNeedInit.ElementAt(i));
                    _contractsThatNeedInit.Remove(_contractsThatNeedInit.ElementAt(i));
                }
            }
        }
    }
    public void OnStartTravelToRealm(RealmID realmID)
    {
        ContractSO contract = ActiveRealmHasContract(realmID);
        BountySO bounty = ActiveRealmHasBounty(realmID);
        if (contract != null)
        {
            _contractsThatNeedInit.Add(contract);
        }
        if (bounty != null)
        {
            _bountiesThatNeedInit.Add(bounty);
        }
    }

    public void OnRetrieveDelveItem(string id)
    {
        DelveSO contractSO = _activeContracts.FirstOrDefault(x => x._id == id);
        _activeRetrievals.Add(contractSO);
        FireScriptedEventsOnRetrieve(contractSO);
        // GameEventsManager.OnRetrieveDelveItem?.Invoke(contractSO);
        Debug.Log("Retreived " + contractSO.name);
    }
    private void OnDepositDelveItem(DelveSO sO)
    {
        _activeRetrievals.Remove(sO);
    }

    public void OnAcceptBounty(BountySO bountySO)
    {
        Debug.Log("Accepted Bounty Delve");
        _activeBounties.Add(bountySO);
    }
    public void OnAcceptContract(ContractSO contractSO)
    {
        _activeContracts.Add(contractSO);
        // InitContract(contractSO);
    }
    #endregion

    #region"Scripted Events"
    void FireScriptedEventsOnStart(DelveSO contractSO)
    {
        List<ScriptedEvent> scriptedEvents = contractSO._events.FindAll(x => x._eventLifecycle == ScriptedEventLifecycle.ON_START);
        if (scriptedEvents.Count == 0) return;
        for (int i = 0; i < scriptedEvents.Count; i++)
        {
            scriptedEvents[i].SetUp(this, contractSO);
        }
    }
    void FireScriptedEventsOnRetrieve(DelveSO contractSO)
    {
        List<ScriptedEvent> scriptedEvents = contractSO._events.FindAll(x => x._eventLifecycle == ScriptedEventLifecycle.ON_RETRIEVE);
        if (scriptedEvents.Count == 0) return;
        for (int i = 0; i < scriptedEvents.Count; i++)
        {
            scriptedEvents[i].SetUp(this, contractSO);
        }
    }
    #endregion

    #region "Ticketing"
    public void IssueDelveTicket(DelveTicket delveTicket)
    {
        _activeDelveTicket = delveTicket;
        Debug.Log("Issued Delve Ticket" + delveTicket._ticketTier);
    }
    #endregion

    #region"Helpers"
    ContractSO ActiveRealmHasContract(RealmID realmID)
    {
        return _activeContracts.FirstOrDefault(x => x._tiedRealm == realmID);
    }
    BountySO ActiveRealmHasBounty(RealmID realmID)
    {
        return _activeBounties.FirstOrDefault(x => x._tiedRealm == realmID);
    }

    public bool PlayerHasDelveTicket()
    {
        return _activeDelveTicket != null;
    }
    #endregion

    private void InitBounty(BountySO bounty)
    {
        throw new NotImplementedException();
    }

    [Button("Save")]
    void SaveDelves()
    {
        ES3.Save("Bounties", _activeBounties);
        ES3.Save("Contracts", _activeContracts);
    }

    void InitContract(ContractSO contractSO)
    {
        for (int i = 0; i < contractSO._events.Count; i++)
        {
            contractSO._events[i].SetUp(this, contractSO);
        }

    }

    public void OnReturnToHomeRealm()
    {
        if (_activeRetrievals.Count > 0)
        {
            Debug.Log("DelveManager noticed you Returning to home realm with delve item ");
        }
        else
        {
            Debug.Log("DelveManager dint care about you Returning to home realm ");
        }

    }
}
