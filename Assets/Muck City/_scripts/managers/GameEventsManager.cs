using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DialogueEditor;
using Invector;
using Invector.vItemManager;
using UnityEngine;
using UnityEngine.SceneManagement;


public class WeaponSO : ScriptableObject
{
    // Placeholder for the WeaponSO class
}
public class GameEventsManager : MonoBehaviour
{
    public static GameEventsManager Instance { get; private set; }

    public QuestEvents _questEvents;

    vGameController _vGameController;

    List<ILoadDataOnStart> _onGameStartTasks = new();
    public int _tasksCount = 0;

    [SerializeField] SceneField _sceneToLoad;
    [SerializeField] SceneField _sceneToUnload;

    public Transform _sceneSpawnPoint;

    public static event Action<Scene> OnSceneChanged;
    public static Action OnGameLoadStartEvent;
    public static Action OnGameLoadEndEvent;

    public static Action OnAllNpcLoadedEvent;
    public static Action<Locations> OnSceneLoadStartEvent;
    public static Action OnSceneLoadEndEvent;
    public static Action<NPCConversation> OnConversationStartedEvent;
    public static Action OnConversationEndEvent;
    public static Action<TimelinePlayer> OnCutSceneStartEvent;
    public static Action OnCutSceneEndEvent;
    public static Action OnInGameHoursPassedEvent;
    public static Action<DeliveryData> OnDeliveryAddedEvent;
    public static Action<DeliveryData> OnDeliveryAcceptedEvent;
    public static Action<DeliveryData> OnDeliveryPointReachedEvent;
    public static Action<District> OnExitDistrictEvent;
    public static Action<District> OnEnterDistrictEvent;
    public static Action<Locations> OnDeliveryMarkerPlacedEvent;
    public static Action<ShopItemSO> OnPurchaseItem;
    public static Action<ItemReference> OnCraftItemEvent;
    public static Action OnSunDownEvent;
    public static Action OnSunUpEvent;
    public static Action<float, bool> OnContaminationUpdate;
    public static Action OnContaminationMaxedOut;

    public static Action OnShouldAutoSave;
    public static Action<Vector3> PlayerOverride;
    public static Action<Vector3> PlayerOverrideComplete;

    public static Action<BranchDecision> BranchDecisionStarted;
    public static Action<BranchDecisionManager.BranchDecisionType> BranchDecisionSelected;
    public static Action BranchDecisionEnded;
    public static Action<int, bool> OnSocialCreditUpdated;
    public static Action OnDisplayPhone;
    public static Action OnHidePhone;
    public static Action<Browsable> OnEnterBrowseMode;
    public static Action OnExitBrowseMode;

    public static Action<WeaponSO> OnAcquireWeapon;
    public static Action<Objective, DomeManager.ObjectiveState> OnObjectiveUpdated;

    // private void OnEnable()
    // {
    //     SceneManager.sceneLoaded += OnSceneLoaded;
    // }

    // private void OnDisable()
    // {
    //     SceneManager.sceneLoaded -= OnSceneLoaded;
    // }

    // private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    // {
    //     Debug.Log("Scene changed" + scene.name);
    //     OnSceneChanged?.Invoke(scene);
    // }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            _vGameController = GetComponent<vGameController>();
        }
        else
        {
            Destroy(gameObject);
        }

        _questEvents = new QuestEvents();
    }

    void Start()
    {
        // OnScreenDebugger.Instance.Log("GameEventsManager started");
        OnGameLoadStartEvent?.Invoke();
        Invoke(nameof(LoadGameStartTasks), 1f);
    }
    public void AddGameStartTask(ILoadDataOnStart task)
    {
        _onGameStartTasks.Add(task);
        _tasksCount++;
    }

    public async void LoadGameStartTasks()
    {
        var tasks = new Task[_onGameStartTasks.Count];
        for (int i = 0; i < _onGameStartTasks.Count; i++)
        {
            tasks[i] = _onGameStartTasks[i].OnLoadTask();
        }

        await Task.WhenAll(tasks);
        OnGameLoadEndEvent?.Invoke();
        _onGameStartTasks.Clear();
        // Debug.Log("All tasks completed");
    }

    public async Task LoadSceneChangeTasks()
    {
        var tasks = new Task[_onGameStartTasks.Count];
        for (int i = 0; i < _onGameStartTasks.Count; i++)
        {
            tasks[i] = _onGameStartTasks[i].OnLoadTask();
        }
        await Task.WhenAll(tasks);
        _onGameStartTasks.Clear();
    }

    public void OnPlayerSpawned()
    {
        Player.Instance.transform.SetParent(_vGameController.spawnPoint);
    }

    public async void OnSceneLoadStart(SceneField newScene, SceneField sceneToUnload)
    {
        _sceneToLoad = newScene;
        _sceneToUnload = sceneToUnload;
        OnSceneLoadStartEvent?.Invoke(SceneNameToLocation(newScene.SceneName));
        await SceneManager.UnloadSceneAsync(_sceneToUnload);
        await SceneManager.LoadSceneAsync(_sceneToLoad, LoadSceneMode.Additive);
        await LoadSceneChangeTasks();
        OnSceneLoadEnd();
    }

    public Locations SceneNameToLocation(string sceneName)
    {
        var location = sceneName switch
        {
            "OTHRO_BUNKER" => Locations.OTHRO_BUNKER,
            "MUCK_CITY" => Locations.MUCK_CITY,
            _ => Locations.NULL,
        };
        return location;
    }

    public void OnSceneLoadEnd()
    {
        Transform spawnPoint = GameObject.FindGameObjectWithTag("SpawnPoint").transform;
        if (spawnPoint != null)
        {
            _sceneSpawnPoint = spawnPoint;
            _vGameController.SetSpawnPoint(spawnPoint);
        }
        // _vGameController.SpawnPlayer(_vGameController.playerPrefab);
        OnSceneLoadEndEvent?.Invoke();
        OnScreenDebugger.Instance.Log($" Loaded scene ");
    }

    public void OnAllNpcLoaded()
    {
        OnAllNpcLoadedEvent?.Invoke();
    }
    public void OnConversationStarted(NPCConversation NPCConversation)
    {
        OnConversationStartedEvent?.Invoke(NPCConversation);
    }
    public void OnConversationEnded()
    {
        OnConversationEndEvent?.Invoke();
    }
    public void OnCutSceneStarted(TimelinePlayer timelinePlayer)
    {
        OnCutSceneStartEvent?.Invoke(timelinePlayer);
    }

    public void OnCutSceneEnded()
    {
        OnCutSceneEndEvent?.Invoke();
    }
    public void OnInGameHoursPassed()
    {
        OnInGameHoursPassedEvent?.Invoke();
    }
    public void OnDeliveryAdded(DeliveryData deliveryData)
    {
        OnDeliveryAddedEvent?.Invoke(deliveryData);
    }
    public void OnDeliveryAccepted(DeliveryData deliveryData)
    {
        OnDeliveryAcceptedEvent?.Invoke(deliveryData);
    }
    public void OnDeliveryPointReached(DeliveryData deliveryData)
    {
        OnDeliveryPointReachedEvent?.Invoke(deliveryData);
    }


    public void OnDistrictExit(District districtExit)
    {
        OnExitDistrictEvent?.Invoke(districtExit);
    }
    public void OnDistrictEnter(District districtExit)
    {
        OnEnterDistrictEvent?.Invoke(districtExit);
    }
    public void OnDeliveryMarkerPlaced(Locations ID)
    {
        OnDeliveryMarkerPlacedEvent?.Invoke(ID);
    }
    public void OnSunDown()
    {
        OnSunDownEvent?.Invoke();
    }
    public void OnSunUp()
    {
        OnSunUpEvent?.Invoke();
    }
    public void OnCraftItem(ItemReference itemReference)
    {
        OnCraftItemEvent?.Invoke(itemReference);
    }



    public void AutoSaveEvent()
    {
        OnShouldAutoSave?.Invoke();
    }
    public void OnPlayerOverride(Vector3 position)
    {
        PlayerOverride?.Invoke(position);
    }
    public void OnPlayerOverrideComplete(Vector3 position)
    {
        PlayerOverrideComplete?.Invoke(position);
        // Debug.Log("Player Override Complete");
    }

    public void StartBranchDecision(BranchDecision branchDecision)
    {
        BranchDecisionStarted?.Invoke(branchDecision);
        // Debug.Log("Branch Decision Started");

        //* Ends automatically after 5 seconds
        Invoke(nameof(EndBranchDecision), 5f);
    }

    public void OnBranchDecisionSelected(BranchDecisionManager.BranchDecisionType type)
    {
        BranchDecisionSelected?.Invoke(type);
    }
    private void EndBranchDecision()
    {
        BranchDecisionEnded?.Invoke();
    }

    public void DisplayPhoneEvent()
    {
        OnDisplayPhone?.Invoke();
    }
    public void HidePhoneEvent()
    {
        OnHidePhone?.Invoke();
    }
    public void OnEnterBrowseModeEvent(Browsable browsable)
    {
        OnEnterBrowseMode?.Invoke(browsable);
    }
    public void OnExitBrowseModeEvent()
    {
        OnExitBrowseMode?.Invoke();
    }

    public void OnObjectiveUpdatedEvent(Objective item, DomeManager.ObjectiveState status)
    {
        OnObjectiveUpdated?.Invoke(item, status);
    }

    public void OnAcquireWeaponEvent(WeaponSO item)
    {
        OnAcquireWeapon?.Invoke(item);
    }

    public void OnContaminationUpdateEvent(float value, bool isDeduction)
    {
        OnContaminationUpdate?.Invoke(value, isDeduction);
    }
    public void OnContaminationMaxedOutEvent()
    {
        OnContaminationMaxedOut?.Invoke();
    }


}
