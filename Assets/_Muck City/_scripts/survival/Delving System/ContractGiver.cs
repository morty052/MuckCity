using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityUtils;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Sirenix.OdinInspector;
using UnityEngine.Events;
using TMPro;
using Unity.VisualScripting;
using UnityEngine.UI;
using System.Threading.Tasks;

public enum DelverScreenName
{
    CONTRACTS = 0,
    BOUNTY = 1,
    DEPOSIT = 2
}

[System.Serializable]
public struct DelverScreenStruct
{
    public GameObject _obj;
    public DelverScreenName _id;
    public DelverScreenStruct(GameObject obj, DelverScreenName delverScreenName)
    {
        _obj = obj;
        _id = delverScreenName;
    }
}

public class ContractGiver : Interactable, IBrowsable
{


    [SerializeField, TabGroup("Components")] NpcCharacter _tiedNpc;
    [SerializeField, TabGroup("Components")] GameObject _ui;
    [SerializeField, TabGroup("Components")] GameObject _screensParent;
    [SerializeField, TabGroup("Components")] GameObject _sideBar;
    [SerializeField, TabGroup("Components")] GameObject _delverButtonPrefab;
    [SerializeField, TabGroup("Text Components")] TextMeshProUGUI _navbarText;

    [SerializeField, TabGroup("Asset Groups")] AssetLabelReference _baseBounties;
    [SerializeField, TabGroup("Asset Groups")] AssetLabelReference _baseContracts;
    [SerializeField, TabGroup("Settings")] List<DelverScreenStruct> _screens = new();
    [SerializeField, TabGroup("Settings")] int _activeScreenIndex = 0;
    [SerializeField, TabGroup("Settings")] int _selectedItemIndex = 0;

    [SerializeField, TabGroup("Events")] UnityEvent OnLoadingComplete;
    [SerializeField, TabGroup("Events")] UnityEvent<BountySO> OnClickBounty;
    [SerializeField, TabGroup("Events")] UnityEvent<ContractSO> OnClickContract;
    [SerializeField, TabGroup("Events")] UnityEvent<ContractSO> OnSelectContract;
    [SerializeField, TabGroup("Events")] UnityEvent<BountySO> OnSelectBounty;
    [SerializeField, TabGroup("Events")] UnityEvent<DelverScreenName> OnConfirmDelve;
    [SerializeField, TabGroup("Events")] UnityEvent OnCancelConfirm;
    [SerializeField, TabGroup("Events")] UnityEvent OnClose;
    [SerializeField, TabGroup("Events")] UnityEvent<DelverScreenName> OnChangeScreen;
    [SerializeField, TabGroup("Events")] bool _debug;

    HashSet<BountySO> _bountySOlist = new();
    HashSet<ContractSO> _contractSOList = new();
    HashSet<GameObject> _bountyList = new();
    HashSet<GameObject> _contractList = new();

    private bool _isConfirmingDelve = false;
    private bool _loadingBounties = true;
    private bool _loadingContracts = true;
    private bool Loading => _loadingBounties && _loadingContracts;
    // HashSet<BountySO> _deposits = new();
    bool IsPopupActive => _screensParent.transform.Children().Any(x => x.gameObject.activeSelf);
    Transform ActivePopup => _screensParent.transform.Children().FirstOrDefault(x => x.gameObject.activeSelf);

    AsyncOperationHandle<IList<BountySO>> _bountyLoadHandle;
    AsyncOperationHandle<IList<ContractSO>> _contractLoadHandle;
    // public override void Start()
    // {
    //     base.Start();
    //     SetupData();
    // }

    async void SetupData()
    {
        LoadAddressables();
        await WaitUntilNotLoading();
        DrawContractButtons();
        OnClickBounty?.Invoke(_bountySOlist.ElementAt(0));
        OnClickContract?.Invoke(_contractSOList.ElementAt(0));
    }

    async Task WaitUntilNotLoading()
    {
        while (Loading)
        {
            await Task.Yield();
            if (_debug)
            {
                Debug.Log("Loading");
            }
        }
        if (_debug)
        {
            Debug.Log("Done Loading");
        }
        OnLoadingComplete?.Invoke();
    }

    public override void Interact()
    {
        Player.Instance.UseAltControls(true, this);
        _ui.SetActive(true);
        SetupData();
    }


    #region"ADDRESSABLE STUFF"
    public AsyncOperationHandle<T> FindAddressable<T>(string id) where T : class
    {
        return Addressables.LoadAssetAsync<T>(id);
    }


    void LoadAddressables()
    {
        _bountyLoadHandle = Addressables.LoadAssetsAsync<BountySO>(_baseBounties, null);
        _contractLoadHandle = Addressables.LoadAssetsAsync<ContractSO>(_baseContracts, null);
        _bountyLoadHandle.Completed += OnCompleteLoadBounty;
        _contractLoadHandle.Completed += OnCompleteLoadContract;

        // _bountyLoadHandle.Release();
        // _contractLoadHandle.Release();
    }


    void OnCompleteLoadBounty(AsyncOperationHandle<IList<BountySO>> handle)
    {
        for (int i = 0; i < handle.Result.Count; i++)
        {
            _bountySOlist.Add(handle.Result[i]);
            Debug.Log(_bountySOlist.ElementAt(i).name);
            CreateBountyButton(_bountySOlist.ElementAt(i), i);
        }
        handle.Completed -= OnCompleteLoadBounty;
        _loadingBounties = false;
    }
    void OnCompleteLoadContract(AsyncOperationHandle<IList<ContractSO>> handle)
    {
        for (int i = 0; i < handle.Result.Count; i++)
        {
            _contractSOList.Add(handle.Result[i]);
            Debug.Log(_contractSOList.ElementAt(i).name);

            CreateContractButton(_contractSOList.ElementAt(i), i);
        }
        handle.Completed -= OnCompleteLoadContract;
        _loadingContracts = false;
    }
    #endregion

    #region "UI STUFF

    public void ShowPopup(int parentIndex)
    {
        //* FIND SELECTED SCREEN
        GameObject obj = _screens.Find(x => x._id == (DelverScreenName)parentIndex)._obj;

        //* DO NOTHING IF SCREEN IS ALREADY SELECTED
        if (obj == null || obj.name == ActivePopup.gameObject.name) return;

        //* SCALE OUT ACTIVE SCREEN
        ABUtils.ScaleOut(ActivePopup.transform, () =>
       {
           //* DISABLE ACTIVE POP UP
           ActivePopup.gameObject.SetActive(false);
           //    obj.transform.localScale = Vector3.zero;
           //* ENABLE NEW POP UP
           obj.SetActive(true);
           //Scale in new pop up
           ABUtils.ScaleIn(obj.transform);
       });

        //* GET SELECTED SCREEN ENUM NAME
        DelverScreenName eumName = (DelverScreenName)parentIndex;

        //* SET NAVBAR TEXT
        string cleanEnumName = eumName.ToString().ToLower().Replace("_", " ").FirstCharacterToUpper();
        _navbarText.text = cleanEnumName;

        //*DEACTIVATE ACTIVE BUTTONS    
        _sideBar.transform.Children().ToList().ForEach(x => x.gameObject.SetActive(false));

        //* SHOW BUTTONS FOR SELECTED SCREEN
        switch (eumName)
        {
            case DelverScreenName.BOUNTY:
                DrawBountyButtons();
                break;
            case DelverScreenName.CONTRACTS:
                DrawContractButtons();
                break;
            default:
                break;
        }
    }

    void CreateBountyButton(BountySO bounty, int index)
    {
        GameObject button = Instantiate(_delverButtonPrefab, _sideBar.transform);
        button.GetComponentInChildren<TextMeshProUGUI>().text = $"{bounty._bounty}SC";

        Button btn = button.GetComponent<Button>();
        btn.onClick.RemoveAllListeners();

        btn.onClick.AddListener(() => DrawBounty(index));
        _bountyList.Add(button);

        button.SetActive(false);
    }
    void CreateContractButton(ContractSO bounty, int index)
    {
        GameObject button = Instantiate(_delverButtonPrefab, _sideBar.transform);
        button.GetComponentInChildren<TextMeshProUGUI>().text = $"{bounty._bounty}SC";

        Button btn = button.GetComponent<Button>();
        btn.onClick.RemoveAllListeners();

        btn.onClick.AddListener(() => DrawContract(index));

        _contractList.Add(button);

        button.SetActive(false);
    }


    void DrawBounty(int index)
    {

        //* SET SELECTED ITEM TO INDEX ASSIGNED TO BUTTON 
        _selectedItemIndex = index;
        OnClickBounty?.Invoke(_bountySOlist.ElementAt(index));

    }

    void DrawContract(int index)
    {
        //* SET SELECTED ITEM TO INDEX ASSIGNED TO BUTTON 
        _selectedItemIndex = index;

        //* FIRE EVENT TO OTHER UI COMPONENTS TO DRAW CONTRACT
        OnClickContract?.Invoke(_contractSOList.ElementAt(index));
    }

    void DrawBountyButtons()
    {
        for (int i = 0; i < _bountyList.Count; i++)
        {
            _bountyList.ElementAt(i).SetActive(true);
        }
    }

    void DrawContractButtons()
    {
        for (int i = 0; i < _contractList.Count; i++)
        {
            _contractList.ElementAt(i).SetActive(true);
        }
    }

    #endregion

    public void OnButtonPress(Inputs button)
    {
        switch (button)
        {
            case Inputs.LEFT:
                if (_activeScreenIndex == 0)
                {
                    _activeScreenIndex = _screens.Count - 1;
                }
                else
                {
                    _activeScreenIndex--;
                }

                ShowPopup(_activeScreenIndex);
                break;
            case Inputs.RIGHT:
                if (_activeScreenIndex == _screens.Count - 1)
                {
                    _activeScreenIndex = 0;
                }
                else
                {
                    _activeScreenIndex++;
                }
                ShowPopup(_activeScreenIndex);
                break;
            case Inputs.BACK:
                if (!_isConfirmingDelve)
                {
                    HandleExit();
                }
                else
                {
                    OnCancelConfirm?.Invoke();
                    _isConfirmingDelve = false;
                }
                break;
            case Inputs.SELECT:
                if (!_isConfirmingDelve)
                {
                    SelectItem();
                }
                else
                {
                    AcceptItem();
                }
                break;
            default:
                break;
        }
    }


    void SelectItem()
    {
        //* GET ACTIVE SCREEN ENUM NAME
        DelverScreenName eumName = (DelverScreenName)_activeScreenIndex;
        switch (eumName)
        {
            case DelverScreenName.BOUNTY:
                OnSelectBounty?.Invoke(_bountySOlist.ElementAt(_selectedItemIndex));
                break;
            case DelverScreenName.CONTRACTS:
                OnSelectContract?.Invoke(_contractSOList.ElementAt(_selectedItemIndex));
                break;
            default:
                break;
        }
        _isConfirmingDelve = true;
    }

    void AcceptItem()
    {
        OnConfirmDelve?.Invoke((DelverScreenName)_activeScreenIndex);
        _isConfirmingDelve = false;

        DelverScreenName eumName = (DelverScreenName)_activeScreenIndex;
        switch (eumName)
        {
            case DelverScreenName.BOUNTY:
                GameEventsManager.OnAcceptBountyEvent.Invoke(_bountySOlist.ElementAt(_selectedItemIndex));
                // DelveManager.Instance.OnAcceptBounty(_bountySOlist.ElementAt(_selectedItemIndex));
                break;
            case DelverScreenName.CONTRACTS:
                GameEventsManager.OnAcceptContractEvent.Invoke(_contractSOList.ElementAt(_selectedItemIndex));
                // DelveManager.Instance.OnAcceptContract(_contractSOList.ElementAt(_selectedItemIndex));
                break;
            default:
                break;
        }

    }

    void HandleExit()
    {
        _ui.SetActive(false);
        Player.Instance.UseAltControls(false);
        _bountyLoadHandle.Release();
        _contractLoadHandle.Release();
        _activeScreenIndex = 0;
        ShowPopup(_activeScreenIndex);
        _loadingBounties = true;
        _loadingContracts = true;
        _bountySOlist.Clear();
        _contractSOList.Clear();
        _contractList.Clear();
        _bountyList.Clear();
        for (int i = 0; i < _sideBar.transform.childCount; i++)
        {
            Destroy(_sideBar.transform.GetChild(i).gameObject);
        }
        OnClose?.Invoke();
    }
}
