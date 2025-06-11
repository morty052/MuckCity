using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine;
using UnityEngine.SceneManagement;

[System.Serializable]
public class NpcManager : MonoBehaviour, ILoadDataOnStart
{
    public static NpcManager Instance { get; private set; }

    [SerializeField]
    private AssetReferenceGameObject _npcLoaderPrefab;

    [SerializeField] GameObject _npcLoaderPrefabDirty;

    private GameObject _npcLoaderInstance;
    [SerializeField] private Dictionary<SpecialCharacters, NpcCharacter> _specialCharacters = new();
    [SerializeField] private List<NpcCharacter> _npcList = new();
    [SerializeField] Locations _activeLocation;
    [SerializeField] Transform _activeSpawnParent;



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
        GameEventsManager.OnGameLoadStartEvent += AddLoadingTaskToQueue;
        GameEventsManager.OnSceneLoadStartEvent += OnSceneChanged;
        GameEventsManager.OnExitDistrictEvent += HandleDistrictExit;
        GameEventsManager.OnEnterDistrictEvent += HandleDistrictEntry;
    }

    void OnDisable()
    {
        GameEventsManager.OnGameLoadStartEvent -= AddLoadingTaskToQueue;
        GameEventsManager.OnSceneLoadStartEvent -= OnSceneChanged;
        GameEventsManager.OnExitDistrictEvent -= HandleDistrictExit;
        GameEventsManager.OnEnterDistrictEvent -= HandleDistrictEntry;
    }

    private void OnSceneChanged(Locations scene)
    {
        _activeLocation = scene;
        AddLoadingTaskToQueue();
    }



    private void HandleDistrictEntry(District district)
    {
        Debug.Log("Enabling npcs in " + district._districtID);
        EnableDistrictNpcs(district._districtID);
    }

    private void HandleDistrictExit(District district)
    {
        DisableDistrictNpcs(district._districtID);
        Debug.Log("disabling npcs in " + district._districtID);
    }


    void EnableDistrictNpcs(Locations location)
    {
        List<NpcCharacter> npcList = GetNPCListByDistrict(location);
        foreach (NpcCharacter npc in npcList)
        {
            npc.gameObject.SetActive(true);
        }
    }
    void DisableDistrictNpcs(Locations location)
    {
        List<NpcCharacter> npcList = GetNPCListByDistrict(location);
        foreach (NpcCharacter npc in npcList)
        {
            npc.gameObject.SetActive(false);
        }
    }

    public void AddNPC(NpcCharacter npc)
    {
        _specialCharacters[npc.ID] = npc; // Ensures fast insertion and prevents duplicate IDs
        _npcList.Add(npc);
    }

    public void RemoveNPC(SpecialCharacters id)
    {
        _specialCharacters.Remove(id);
    }

    public SpecialNPC GetSpecialCharacterByID(SpecialCharacters id)
    {
        _specialCharacters.TryGetValue(id, out NpcCharacter npc);
        // Debug.Log("loaded npcs" + _specialCharacters.Count);
        // Debug.Log("found" + npc.name);
        SpecialNPC specialNPC = npc.GetComponent<SpecialNPC>();
        return specialNPC;
    }

    public List<NpcCharacter> GetAllNPCs()
    {
        return _npcList;
    }
    public List<NpcCharacter> GetNPCByRole(Role role, Locations location = Locations.NULL)
    {
        var npcList = _npcList.FindAll(x => x._roles.Contains(role));
        if (location != Locations.NULL)
        {
            npcList = npcList.FindAll(x => x.Data._primaryLocation == location);
        }
        return npcList;
    }
    public List<NpcCharacter> GetNPCListByDistrict(Locations location, Role role = Role.NONE)
    {
        var npcList = _npcList.FindAll(x => x.Data._primaryLocation == location);
        if (role != Role.NONE)
        {
            var npcListInLocationByRole = npcList.FindAll(x => x._roles.Contains(role));
            npcList = npcListInLocationByRole;
        }
        return npcList;
    }
    public NpcCharacter GetNPCByName(string name)
    {
        NpcCharacter npc = _npcList.Find(x => x.Data._name == name);
        return npc;
    }

    public void AddLoadingTaskToQueue()
    {

        GameEventsManager.Instance.AddGameStartTask(this);
    }

    // public async Task OnLoadTask()
    // {
    //     LoadAddressable();
    //     await AwaitAddressable();
    //     NpcLoader npcLoader = _npcLoaderInstance.GetComponent<NpcLoader>();
    //     _activeSpawnParent = GameObject.FindGameObjectWithTag("SpawnPoint").transform;
    //     await npcLoader.SpawnNpcList(_activeLocation, _activeSpawnParent);
    //     Addressables.Release(_npcLoaderInstance);
    //     Destroy(_npcLoaderInstance);
    //     GameEventsManager.Instance.OnAllNpcLoaded();
    //     OnScreenDebugger.Instance.Log("loaded npcs " + _activeLocation);
    // }

    public async Task OnLoadTask()
    {
        NpcLoader npcLoader = Instantiate(_npcLoaderPrefabDirty, Vector3.zero, Quaternion.identity).GetComponent<NpcLoader>();
        _activeSpawnParent = GameObject.FindGameObjectWithTag("SpawnPoint").transform;
        // Scene secondScene = SceneManager.GetSceneAt(1);
        // _activeLocation = GameEventsManager.Instance.SceneNameToLocation(secondScene.name);
        // OnScreenDebugger.Instance.Log("loaded npcs for " + _activeLocation + " in scene " + SceneManager.GetSceneAt(1).name);

        await npcLoader.SpawnNpcList(Locations.BUNKER_HEIGHTS, _activeSpawnParent);
        // OnScreenDebugger.Instance.Log("loaded npcs for " + _activeLocation + " total: " + _specialCharacters.Count + " in scene " + SceneManager.GetSceneAt(1).name);
        GameEventsManager.Instance.OnAllNpcLoaded();
        Destroy(npcLoader.gameObject);
    }

    void LoadAddressable()
    {
        _npcLoaderPrefab.InstantiateAsync().Completed += OnAddressableLoaded;
    }

    public SpecialNPC SpawnAndMoveToPosition(NpcSO npc, Pos position)
    {
        Transform spawnParent = GameObject.FindGameObjectWithTag("SpawnPoint").transform;
        SpecialNPC specialNPC = Instantiate(npc._npcPrefab, spawnParent).GetComponent<SpecialNPC>();
        specialNPC.transform.SetPositionAndRotation(position.position, Quaternion.Euler(position.rotation.x, position.rotation.y, position.rotation.z));
        return specialNPC;
    }

    void SpawnAndMoveToPosition(string npc, Pos position)
    {

    }

    void OnAddressableLoaded(AsyncOperationHandle<GameObject> obj)
    {
        if (obj.Status == AsyncOperationStatus.Succeeded)
        {
            _npcLoaderInstance = obj.Result;
            _npcLoaderInstance.transform.parent = transform;
        }
    }

    async Task AwaitAddressable()
    {
        while (_npcLoaderInstance == null)
        {
            await Task.Yield();
        }
    }
}


