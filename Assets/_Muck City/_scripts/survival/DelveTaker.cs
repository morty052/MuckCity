using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class DelveTaker : Interactable, IBrowsable
{
    [SerializeField, TabGroup("Components")] GameObject _ui;
    [SerializeField, TabGroup("Components")] GameObject _noItemsScreen;
    [SerializeField, TabGroup("Components")] GameObject _screensParent;
    [SerializeField, TabGroup("Components")] GameObject _sideBar;
    [SerializeField, TabGroup("Components")] GameObject _delverButtonPrefab;
    [SerializeField, TabGroup("Text Components")] TextMeshProUGUI _navbarText;
    [SerializeField, TabGroup("Events")] UnityEvent OnLoadingComplete;
    [SerializeField, TabGroup("Events")] UnityEvent OnClose;

    [SerializeField, TabGroup("Events")] UnityEvent<ContractSO> OnClickContract;

    [SerializeField, TabGroup("Debug")] int _selectedItemIndex = 0;

    HashSet<GameObject> _contractList = new();

    ContractSO ActiveContract => DelveManager.Instance.Retrievals.ElementAt(_selectedItemIndex);
    GameObject ActiveButton => _contractList.ElementAt(_selectedItemIndex);
    bool HasContractToCollect => _contractList.Count > 0;
    public override void Interact()
    {
        Player.Instance.UseAltControls(true, this);
        _ui.SetActive(true);
        SetupData();
    }

    public void OnButtonPress(Inputs button)
    {
        switch (button)
        {
            case Inputs.LEFT:
                // if (_activeScreenIndex == 0)
                // {
                //     _activeScreenIndex = _screens.Count - 1;
                // }
                // else
                // {
                //     _activeScreenIndex--;
                // }

                // ShowPopup(_activeScreenIndex);
                break;
            case Inputs.RIGHT:
                // if (_activeScreenIndex == _screens.Count - 1)
                // {
                //     _activeScreenIndex = 0;
                // }
                // else
                // {
                //     _activeScreenIndex++;
                // }
                // ShowPopup(_activeScreenIndex);
                break;
            case Inputs.BACK:
                HandleExit();
                break;
            case Inputs.SELECT:
                RewardContract();
                break;
            default:
                break;
        }
    }

    /// <summary>
    ///* Deactivates the UI, resets player controls, clears the contract list, 
    ///* and invokes the OnClose event. Also, destroys all child objects of the sidebar.
    /// </summary>

    private void HandleExit()
    {
        _ui.SetActive(false);
        Player.Instance.UseAltControls(false);
        OnClose?.Invoke();
        _contractList.Clear();
        for (int i = 0; i < _sideBar.transform.childCount; i++)
        {
            Destroy(_sideBar.transform.GetChild(i).gameObject);
        }
    }

    /// <summary>
    ///* Sets up data for the delve by iterating through available retrievals 
    ///* and creating collection buttons for each contract. If no contracts 
    ///* are available to collect, it activates the no-items screen. Once 
    ///* setup is complete, invokes an optional loading complete event, displays 
    ///* the first contract, and draws all contract buttons.
    /// </summary>

    void SetupData()
    {
        for (int i = 0; i < DelveManager.Instance.Retrievals.Count; i++)
        {
            ContractSO contractSO = DelveManager.Instance.Retrievals.ElementAt(i);
            CreateCollectionButton(contractSO, i);
        }
        if (!HasContractToCollect)
        {
            _noItemsScreen.SetActive(true);
            return;
        }
        OnLoadingComplete?.Invoke();
        DrawContract(0);
        DrawContractButtons();
    }



    /// <summary>
    /// *Creates a button for a given contract and adds it to the sidebar UI.
    /// *The button's text displays the bounty value, and an event listener is attached
    /// *to handle contract drawing when clicked. The button is initially inactive 
    /// *and is stored in the contract list.
    /// </summary>
    /// <param name="bounty">The contract data used to configure the button.</param>
    /// <param name="index">The index of the contract, used to identify the button's action.</param>

    void CreateCollectionButton(ContractSO bounty, int index)
    {
        GameObject button = Instantiate(_delverButtonPrefab, _sideBar.transform);
        button.GetComponentInChildren<TextMeshProUGUI>().text = $"{bounty._bounty}SC";

        Button btn = button.GetComponent<Button>();
        btn.onClick.RemoveAllListeners();

        btn.onClick.AddListener(() => DrawContract(index));

        _contractList.Add(button);

        button.SetActive(false);
    }

    private void DrawContract(int index)
    {
        //* SET SELECTED ITEM TO INDEX ASSIGNED TO BUTTON 
        _selectedItemIndex = index;
        OnClickContract?.Invoke(ActiveContract);
    }

    void DrawContractButtons()
    {
        //* SET ALL BUTTONS STORED IN LIST TO ACTIVE
        for (int i = 0; i < _contractList.Count; i++)
        {
            _contractList.ElementAt(i).SetActive(true);
        }
    }

    void RewardContract()
    {
        Debug.Log("Collecting Reward for contract" + ActiveContract.name);
        ActiveButton.SetActive(false);
        _contractList.Remove(ActiveButton);
        GameEventsManager.OnDepositDelveItemEvent?.Invoke(ActiveContract);
        if (!HasContractToCollect)
        {
            _noItemsScreen.SetActive(true);
        }
    }
}
