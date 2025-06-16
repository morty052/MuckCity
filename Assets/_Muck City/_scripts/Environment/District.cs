using System.Collections.Generic;
using System.Threading.Tasks;
using Invector.vCharacterController.AI;
using Sirenix.OdinInspector;
using UnityEngine;

public class District : MonoBehaviour, ILoadDataOnStart
{
    public Locations _districtID;
    [SerializeField] DistrictExit _exit;

    [SerializeField] bool _playerIsInCompound;

    [TabGroup("Security")]
    public GuardNpcSO _guardNpc = new();

    [TabGroup("Security")]
    public List<Pos> _guardSpawnPoints = new();

    [SerializeField] vWaypointArea _patrolPoints;

    void OnEnable()
    {
        GameEventsManager.OnGameLoadStartEvent += AddLoadingTaskToQueue;
    }

    void OnDisable()
    {
        GameEventsManager.OnGameLoadStartEvent -= AddLoadingTaskToQueue;
    }


    public void TogglePlayerPresence(bool state)
    {
        _playerIsInCompound = state;
        if (state)
        {
            GameEventsManager.Instance.OnDistrictEnter(this);
        }
        else
        {
            GameEventsManager.Instance.OnDistrictExit(this);
        }
    }

    public void OnDeliveryMarkerPlaced()
    {
        Debug.Log(" Player has placed delivery marker in district " + _districtID);
        GameEventsManager.OnDeliveryMarkerPlacedEvent(_districtID);
    }

    public Task OnLoadTask()
    {
        if (_patrolPoints == null || _guardSpawnPoints.Count == 0) return Task.CompletedTask;

        for (int i = 0; i < _guardSpawnPoints.Count; i++)
        {
            GuardNPC guardNPC = SpawnGuard(_guardSpawnPoints[i]);
            guardNPC.UpdateWayPoint(_patrolPoints);
            Debug.Log("Spawned guard " + " in district " + _districtID);
        }
        return Task.CompletedTask;
    }

    public void AddLoadingTaskToQueue()
    {
        GameEventsManager.Instance.AddGameStartTask(this);
    }

    GuardNPC SpawnGuard(Pos spawnPos)
    {
        GuardNPC guard = Instantiate(_guardNpc._npcPrefab, spawnPos.position, Quaternion.Euler(spawnPos.rotation)).GetComponent<GuardNPC>();
        return guard;
    }
}
