using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Invector.vCharacterController.AI;
using Invector.vCharacterController.AI.FSMBehaviour;
using Sirenix.OdinInspector;
using UnityEngine;



public class District : MonoBehaviour, ILoadDataOnStart
{
    [TabGroup("Details")]
    public Locations _districtID;

    [SerializeField] NpcSpawnData _npcList;


    [TabGroup("State")]
    [SerializeField] bool _playerIsInCompound;

    [TabGroup("Security")]
    public GuardNpcSO _guardNpc;

    [TabGroup("Security")]
    public List<Pos> _guardSpawnPoints = new();
    [TabGroup("Security")]
    public List<Pos> _exitGuards = new();

    [TabGroup("Security")]
    [SerializeField] vWaypointArea _patrolPoints;

    [TabGroup("Security")]

    [ShowInInspector, ReadOnly]
    public HashSet<GuardNPC> _guards = new();

    // void OnEnable()
    // {
    //     GameEventsManager.OnGameLoadStartEvent += AddLoadingTaskToQueue;
    // }

    // void OnDisable()
    // {
    //     GameEventsManager.OnGameLoadStartEvent -= AddLoadingTaskToQueue;
    // }


    [Button("Respond to threat"), TabGroup("Security")]
    void MobilizeGuardsToPoint(Vector3 point, int count = 2)
    {
        List<GuardNPC> guards = GetGuards(count, point);
        foreach (GuardNPC guard in guards)
        {
            guard.GetComponent<vFSMBehaviourController>().StopFSM();
            guard.GetComponent<vControlAI>().MoveTo(point, vAIMovementSpeed.Running);
        }
    }

    List<GuardNPC> GetGuards(int count, Vector3 point = default)
    {
        List<GuardNPC> guards = new();

        //find guards closest to point 
        if (point != default)
        {
            List<GuardNPC> closestGuards = _guards.OrderBy(x => Vector3.Distance(x.transform.position, point)).ToList();
            for (int i = 0; i < count; i++)
            {
                guards.Add(_guards.ElementAt(i));
            }
        }

        return guards;
    }


    /// <summary>
    /// *Toggles the player's presence in the district.
    /// 
    ///* When the player is present, the district's guards will be activated and the district's Enter event will be triggered.
    ///* When the player is not present, the district's guards will be deactivated and the district's Exit event will be triggered.
    /// </summary>
    /// <param name="state">The player's presence state.</param>
    public void TogglePlayerPresence(bool state)
    {
        _playerIsInCompound = state;
        if (state)
        {
            GameEventsManager.Instance.OnDistrictEnter(this);
            foreach (GuardNPC guard in _guards)
            {
                guard.GetComponent<vFSMBehaviourController>().StartFSM();
            }
        }
        else
        {
            GameEventsManager.Instance.OnDistrictExit(this);
            foreach (GuardNPC guard in _guards)
            {
                guard.GetComponent<vFSMBehaviourController>().StopFSM();
            }
        }
    }

    public Task LoadDistrictNpcs()
    {
        if (_npcList == null)
        {
            Debug.Log("No spawn data");
            return Task.CompletedTask;
        }
        foreach (SpawnStruct npcData in _npcList._data)
        {
            NpcCharacter npcCharacter = Instantiate(npcData._npc._npcPrefab, transform);
            npcCharacter.transform.SetLocalPositionAndRotation(npcData._location.position, Quaternion.Euler(npcData._location.rotation));
            NpcManager.Instance.AddNPC(npcCharacter);
        }

        GameEventsManager.Instance.OnAllNpcLoaded();
        return Task.CompletedTask;
    }

    public void OnDeliveryMarkerPlaced()
    {
        Debug.Log(" Player has placed delivery marker in district " + _districtID);
        GameEventsManager.OnDeliveryMarkerPlacedEvent(_districtID);
    }

    public Task LoadGuards()
    {
        if (_patrolPoints == null || _guardSpawnPoints.Count == 0) return Task.CompletedTask;

        for (int i = 0; i < _guardSpawnPoints.Count; i++)
        {
            GuardNPC guardNPC = SpawnGuard(_guardSpawnPoints[i]);
            guardNPC.UpdateWayPoint(_patrolPoints);
            _guards.Add(guardNPC);
        }
        for (int i = 0; i < _exitGuards.Count; i++)
        {
            GuardNPC guardNPC = SpawnGuard(_exitGuards[i]);
            // guardNPC.UpdateWayPoint(_patrolPoints);
            // guardNPC.GetComponent<vFSMBehaviourController>().StopFSM();
            _guards.Add(guardNPC);
            // Debug.Log("Spawned guard " + " in district " + _districtID);
        }
        return Task.CompletedTask;
    }
    public async Task OnLoadTask()
    {

        await LoadGuards();
        await LoadDistrictNpcs();

        // return Task.CompletedTask;
    }

    public void AddLoadingTaskToQueue()
    {
        GameEventsManager.Instance.AddGameStartTask(this);
    }

    GuardNPC SpawnGuard(Pos spawnPos)
    {
        GuardNPC guard = Instantiate(_guardNpc._npcPrefab, spawnPos.position, Quaternion.Euler(spawnPos.rotation), transform).GetComponent<GuardNPC>();
        return guard;
    }
}
