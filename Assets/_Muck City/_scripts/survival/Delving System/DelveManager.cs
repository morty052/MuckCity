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
        GameEventsManager.OnDepositDelveItemEvent += OnDepositDelveItem;
        SceneLoader.OnSceneGroupLoaded += OnSceneGroupLoaded;
    }

    void OnDisable()
    {
        GameEventsManager.OnAcceptBountyEvent -= OnAcceptBounty;
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
                    FireScriptedEventsOnEnterRealm(_contractsThatNeedInit.ElementAt(i));
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
        FireScriptedEventsOnAccept(contractSO);
    }
    #endregion

    #region"Scripted Events"
    void FireScriptedEventsOnAccept(DelveSO contractSO)
    {

        if (contractSO._OnAccept.Count == 0) return;
        for (int i = 0; i < contractSO._OnAccept.Count; i++)
        {
            ScriptedEvent scriptedEvent = contractSO._OnAccept[i];
            if (scriptedEvent._executionDelay == 0)
            {
                scriptedEvent.Execute(this, contractSO);
            }
            else
            {
                scriptedEvent.DelayedExecute(this, contractSO);
            }
        }
    }
    void FireScriptedEventsOnEnterRealm(DelveSO contractSO)
    {

        if (contractSO._OnEnterRealm.Count == 0) return;
        for (int i = 0; i < contractSO._OnEnterRealm.Count; i++)
        {
            ScriptedEvent scriptedEvent = contractSO._OnEnterRealm[i];
            if (scriptedEvent._executionDelay == 0)
            {
                scriptedEvent.Execute(this, contractSO);
            }
            else
            {
                scriptedEvent.DelayedExecute(this, contractSO);
            }
            // contractSO._OnAccept[i].Execute(this, contractSO);
        }
    }
    void FireScriptedEventsOnRetrieve(DelveSO contractSO)
    {
        if (contractSO._onRetrieveEvents.Count == 0) return;
        for (int i = 0; i < contractSO._onRetrieveEvents.Count; i++)
        {
            ScriptedEvent scriptedEvent = contractSO._onRetrieveEvents[i];
            if (scriptedEvent._executionDelay == 0)
            {
                scriptedEvent.Execute(this, contractSO);
            }
            else
            {
                scriptedEvent.DelayedExecute(this, contractSO);
            }
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
        for (int i = 0; i < contractSO._OnEnterRealm.Count; i++)
        {
            contractSO._OnEnterRealm[i].Execute(this, contractSO);
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
