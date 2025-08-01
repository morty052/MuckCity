using System;
using System.Collections;
using System.Collections.Generic;
using ImprovedTimers;
using TMPro;
using UnityEngine;
using DG.Tweening;
using Sirenix.OdinInspector;
#if UNITY_EDITOR
using UnityEditor;
#endif

public enum LocationType
{
    BUNKER,
    SHOP,
    DISTRICT
}

[System.Serializable]
public struct LocationData
{
    public Locations _name;
    public Vector3 _entrance;
    public Vector3 _activityTriggerPosition;

    public LocationType _type;

    public readonly bool HasActivityTrigger => _activityTriggerPosition != Vector3.zero;


    public LocationData(Locations name, Vector3 position, LocationType type, Vector3 activityTriggerPosition)
    {
        _name = name;
        _entrance = position;
        _type = type;
        _activityTriggerPosition = activityTriggerPosition;
    }

}

[System.Serializable]
public struct Objective
{
    public string _title;
    public Vector3 _wayPoint;

    public bool _isCompleted;

    public int _index;

    public readonly bool HasWayPoint => _wayPoint != Vector3.zero;

#if UNITY_EDITOR
    [Button("Snap")]
    void Snap()
    {
        _wayPoint = Selection.activeGameObject.transform.position;
    }
#endif

    public Objective(string title, Vector3 objectiveStartPosition, int index, bool isCompleted = false)
    {
        _title = title;
        _wayPoint = objectiveStartPosition;
        _isCompleted = isCompleted;
        _index = index;

    }
}

[System.Serializable]
public struct Mission
{
    public string _title;
    public Vector3 _missionStartPoint;

    public List<Objective> _objectives;
    public readonly bool HasMarker => _missionStartPoint != Vector3.zero;





    public Mission(string text, List<Objective> objectives, Vector3 missionTriggerPosition)
    {
        _title = text;
        _missionStartPoint = missionTriggerPosition;
        _objectives = objectives;
    }

}

public enum ObjectiveState
{
    STARTED,
    COMPLETED
}


public class DomeManager : MonoBehaviour
{
    public static DomeManager Instance { get; private set; }

    [TabGroup("Objective")]
    public ObjectiveRenderer _objectiveRenderer;

    [SerializeField] Waypoint _questMarker;


    [TabGroup("Locations")]
    public LocationData _othroBunkerLocationData;
    [TabGroup("Locations")]
    [SerializeField] List<LocationData> _shops = new();
    [TabGroup("Locations")]
    [SerializeField] List<LocationData> _districts = new();


    [SerializeField, TabGroup("Attack")] bool _canSpawn = true;
    [SerializeField, TabGroup("Attack")] int _spawnCount;

    [SerializeField, TabGroup("Attack")] List<Zombie> _spawnedEnemies;
    [SerializeField, TabGroup("Weather")] FogController _fogController;

    [SerializeField, TabGroup("Locations")] ScriptedLocationsSO _LocationsData;


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            _fogController = new();
        }
        else
        {
            Destroy(gameObject);
        }
    }


    void OnEnable()
    {
        GameEventsManager.OnExitDistrictEvent += HandleDistrictExit;
        GameEventsManager.OnEnterDistrictEvent += HandleDistrictEntry;
        TimeService.OnSunSet += HandleSunDown;
        TimeService.OnSunRise += HandleSunUp;

        // GameEventsManager.OnAcceptBountyEvent += OnAcceptBounty;
        // GameEventsManager.OnAcceptContractEvent += OnAcceptContract;
    }

    void OnDisable()
    {
        GameEventsManager.OnExitDistrictEvent -= HandleDistrictExit;
        GameEventsManager.OnEnterDistrictEvent -= HandleDistrictEntry;
        TimeService.OnSunSet -= HandleSunDown;
        TimeService.OnSunRise -= HandleSunUp;
        // GameEventsManager.OnAcceptBountyEvent -= OnAcceptBounty;
        // GameEventsManager.OnAcceptContractEvent -= OnAcceptContract;
    }

    // private void OnAcceptContract(ContractSO sO)
    // {
    //     ScriptedLocation scriptedLocation = _LocationsData.GetLocation(sO._keyLocation);
    //     Debug.Log(scriptedLocation.GetCleanName() + " Is a key location");
    //     Waypoint.Instance.Init(scriptedLocation._posData.position);
    // }

    // public ScriptedLocation UseApproximateLocation(Locations keyLocation, bool useWayPoint = true)
    // {
    //     ScriptedLocation scriptedLocation = _LocationsData.GetLocation(keyLocation);
    //     if (useWayPoint)
    //     {
    //         Waypoint.Instance.Init(scriptedLocation._posData.position);
    //     }
    //     return scriptedLocation;
    // }

    // private void OnAcceptBounty(BountySO sO)
    // {
    //     ScriptedLocation scriptedLocation = _LocationsData.GetLocation(sO._lastKnownPos);
    //     Debug.Log(scriptedLocation.GetCleanName() + " Is the last known location");
    //     Waypoint.Instance.Init(scriptedLocation._posData.position);
    // }

    [Button]
    private void HandleSunDown()
    {
        // InitFog();
        if (!_canSpawn || Player.Instance.IsUnderGround) return;
        Debug.Log("Sun is down, its zombie time!");
        for (int i = 0; i < _spawnCount; i++)
        {
            SpawnEnemy();
        }
    }
    private void HandleSunUp()
    {
        // Debug.Log("Sun is Up, Fry all zombies!");
        // ClearFog();
        for (int i = 0; i < _spawnedEnemies.Count; i++)
        {
            _spawnedEnemies[i].Die();
        }

    }

    private void HandleDistrictEntry(District exit)
    {
        Debug.Log("Player entered a district " + exit._districtID);
    }

    private void HandleDistrictExit(District exit)
    {
        Debug.Log("Player EXITED a district " + exit._districtID);
    }


    public void SetupMissionDisplay(Mission mission)
    {
        _objectiveRenderer.SetupMission(mission);
    }
    public void SetupMissionDisplay(Mission mission, int objectivesToDisplayOnStart)
    {
        _objectiveRenderer.SetupMission(mission, objectivesToDisplayOnStart);
    }
    public void UpdateMissionDisplay(int objectivesToAdd)
    {
        _objectiveRenderer.UpdateMissionDisplay(objectivesToAdd);
    }
    public void ClearMissionDisplay()
    {
        _objectiveRenderer.ClearMissionDisplay();
    }

    public void CompleteObjective(int index)
    {
        _objectiveRenderer.CompleteObjective(index);
    }


    public LocationData GetRandomLocation()
    {
        return _shops[0];
    }
    public LocationData GetLocationByName(Locations locationName, LocationType locationType)
    {
        LocationData location = _shops[0];
        switch (locationType)
        {
            case LocationType.SHOP:
                location = _shops.Find(x => x._name == locationName);
                break;
            case LocationType.DISTRICT:
                location = _districts.Find(x => x._name == locationName);
                break;
            default:
                break;
        }

        return location;
    }

    public LocationData GetOthroBunker()
    {

        return _othroBunkerLocationData;
    }


    #region "ATTACK"



    public void SpawnEnemy()
    {
        Zombie enemy = PoolManager.Instance.GetZombie();
        enemy.transform.position = GetRandomPointInCombatRange();
        enemy.transform.LookAt(Player.Instance.transform.position);
        _spawnedEnemies.Add(enemy);
    }

    Vector3 GetRandomPointInCombatRange()
    {
        Transform combatHelperSphere = Player.Instance._combatHelperSphere;
        float radius = combatHelperSphere.localScale.x / 2;
        Vector3 randomPosition = UnityEngine.Random.insideUnitSphere * radius;
        Vector3 spawnPoint = combatHelperSphere.position + randomPosition;

        // Debug.Log("Spawn Point: " + spawnPoint);
        return spawnPoint;
    }

    #endregion

    #region "Weather"

    [Button, TabGroup("Weather")]
    public void InitFog()
    {
        StartCoroutine(_fogController.FadeInFog());
    }
    [Button, TabGroup("Weather")]
    public void ClearFog()
    {
        StartCoroutine(_fogController.ClearFog());
    }
    #endregion
}



public class FogController
{
    public float targetDensity = 0.05f;
    public float fadeDuration = 5.0f;


    public IEnumerator FadeInFog()
    {
        float timePassed = 0f;
        while (timePassed <= fadeDuration)
        {
            float factor = timePassed / fadeDuration;
            RenderSettings.fogDensity = Mathf.Lerp(0, targetDensity, factor);
            timePassed += Time.deltaTime;
            yield return null;
        }
    }
    public IEnumerator ClearFog()
    {
        float timePassed = 0f;
        while (timePassed <= fadeDuration)
        {
            float factor = timePassed / fadeDuration;
            RenderSettings.fogDensity = Mathf.Lerp(targetDensity, 0, factor);
            timePassed += Time.deltaTime;
            yield return null;
        }
    }
}
