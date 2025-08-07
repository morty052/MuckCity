using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Eflatun.SceneReference;
using Sirenix.OdinInspector;
using Systems.SceneManagement;
using TMPro;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

public class DelveGateConsole : Interactable, IBrowsable
{
    [SerializeField, TabGroup("Components")] private GameObject _hologramCurtain;
    [SerializeField, TabGroup("Components")] private Canvas _consoleUI;
    [SerializeField, TabGroup("Components")] private GameObject _sideBar;
    [SerializeField, TabGroup("Components")] private TextMeshProUGUI _realmNameText;
    [SerializeField, TabGroup("Components")] private TextMeshProUGUI _realmDescriptionText;
    [SerializeField, TabGroup("Components")] private GameObject _delverButtonPrefab;

    [TabGroup("Data")] public List<RealmDataSO> _realms;

    [TabGroup("Data")] public SceneData _lastSavedScene;
    [SerializeField, TabGroup("Asset Groups")] AssetLabelReference _realmsAssetGroup;

    AsyncOperationHandle<IList<RealmDataSO>> _realmsLoadHandle;
    HashSet<GameObject> _contractList = new();

    private int _selectedRealm;
    private bool _loadingRealms;



    void OnTriggerEnter(Collider other)
    {
        PrepareInteraction();
    }

    void OnTriggerExit(Collider other)
    {
        HideInteractionPrompt();
    }

    public override void Interact()
    {
        Player.Instance.UseAltControls(true, this);
        _hologramCurtain.SetActive(true);
        LoadAddressables();
        _consoleUI.gameObject.SetActive(true);
        HideInteractionPrompt();
    }




    public void OnButtonPress(Inputs button)
    {
        switch (button)
        {
            case Inputs.BACK:
                _hologramCurtain.SetActive(false);
                _consoleUI.gameObject.SetActive(false);
                Player.Instance.UseAltControls(false);
                break;
            case Inputs.SELECT:
                TravelToRealm();
                break;
        }
    }

    void LoadAddressables()
    {
        _realmsLoadHandle = Addressables.LoadAssetsAsync<RealmDataSO>(_realmsAssetGroup, null);
        _realmsLoadHandle.Completed += OnCompleteLoadRealms;
    }


    void OnCompleteLoadRealms(AsyncOperationHandle<IList<RealmDataSO>> handle)
    {
        for (int i = 0; i < handle.Result.Count; i++)
        {
            _realms.Add(handle.Result[i]);
            Debug.Log(_realms[i].name);
            CreateRealmButton(_realms[i], i);
        }
        handle.Completed -= OnCompleteLoadRealms;
        _loadingRealms = false;

        //* Start with the first realm
        DrawRealmData(0);
    }

    private void CreateRealmButton(RealmDataSO realmDataSO, int i)
    {
        GameObject button = Instantiate(_delverButtonPrefab, _sideBar.transform);
        button.GetComponentInChildren<TextMeshProUGUI>().text = $"{realmDataSO._realmID}SC";

        Button btn = button.GetComponent<Button>();
        btn.onClick.RemoveAllListeners();

        btn.onClick.AddListener(() => DrawRealmData(i));

        _contractList.Add(button);

        // button.SetActive(false);
    }

    private void DrawRealmData(int index)
    {
        RealmDataSO realmDataSO = _realms[index];
        _selectedRealm = index;
        _realmNameText.text = realmDataSO.GetCleanNameFromEnum();
        _realmDescriptionText.text = realmDataSO._realmDescription;
    }

    async void TravelToRealm()
    {
        DelveManager.Instance.OnStartTravelToRealm(_realms[_selectedRealm]._realmID);
        SceneData sceneData = _realms[_selectedRealm]._sceneData;
        SceneGroup sceneToLoad = new()
        {
            GroupName = sceneData.Name,
            Scenes = new() { sceneData }
        };
        Player.Instance.UseAltControls(false);
        _consoleUI.gameObject.SetActive(false);
        Player.Instance.SetInteractableObject(null);
        await SceneLoader.Instance.LoadSceneGroup(sceneToLoad, true, true);
    }
}
