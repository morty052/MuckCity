using System;
using System.Collections;
using System.Collections.Generic;
using ImprovedTimers;
using TMPro;
using UnityEngine;
using DG.Tweening;
using Sirenix.OdinInspector;

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
    public string _text;
    public Vector3 _objectiveTriggerPosition;
    public readonly bool HasMarker => _objectiveTriggerPosition != Vector3.zero;


    public Objective(string text, Vector3 objectiveTriggerPosition)
    {
        _text = text;
        _objectiveTriggerPosition = objectiveTriggerPosition;
    }

}


public class DomeManager : MonoBehaviour
{
    public static DomeManager Instance { get; private set; }
    public enum ObjectiveState
    {
        STARTED,
        COMPLETED
    }

    [Header("Locations")]
    public LocationData _othroBunkerLocationData;
    [SerializeField] List<LocationData> _shops = new();
    [SerializeField] List<LocationData> _districts = new();

    [SerializeField] GameObject _skyDome;
    [SerializeField] Material _skyTexture;
    [SerializeField] GameObject _sun;
    [SerializeField] GameObject _moon;


    [Header("Day Management")]
    CountdownTimer _inGameHoursTimer;
    [SerializeField] float _inGameHoursInterval = 10f;

    [SerializeField] float _nightTime = 0.4f;
    [SerializeField] float _dayTime = 0.1f;
    [SerializeField] float _timeOfDay = 0.1f;
    [SerializeField] float _offsetInterval = 0.1f;

    [SerializeField] int _inGameHours = 0;

    [SerializeField] Light _mainDirectionalLight;
    [SerializeField] Color _nightColor = new(0.1f, 0.1f, 0.1f, 1f);
    [SerializeField] Color _dayColor;

    [HideInInspector]
    public Objective _activeObjective;

    [SerializeField] Waypoint _questMarker;

    bool IsNightTime => _timeOfDay >= _nightTime;


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            _inGameHoursTimer = new(_inGameHoursInterval);
            _skyTexture = _skyDome.GetComponent<MeshRenderer>().material;
        }
        else
        {
            Destroy(gameObject);
        }
    }


    void OnEnable()
    {
        // GameEventsManager.OnPurchaseItem += DeliverItem;
        GameEventsManager.OnAcquireWeapon += NotifyPlayer;
        GameEventsManager.OnExitDistrictEvent += HandleDistrictExit;
        GameEventsManager.OnEnterDistrictEvent += HandleDistrictEntry;
        GameEventsManager.OnSunDownEvent += HandleSunDown;
        // GameEventsManager.OnObjectiveUpdated += OnObjectiveUpdated;
    }

    void OnDisable()
    {
        // GameEventsManager.OnPurchaseItem -= DeliverItem;
        GameEventsManager.OnAcquireWeapon -= NotifyPlayer;
        GameEventsManager.OnExitDistrictEvent -= HandleDistrictExit;
        GameEventsManager.OnEnterDistrictEvent -= HandleDistrictEntry;
        GameEventsManager.OnSunDownEvent -= HandleSunDown;
        // GameEventsManager.OnObjectiveUpdated -= OnObjectiveUpdated;
    }

    [Button]
    private void HandleSunDown()
    {
        Debug.Log("Sun is down");
        Color color = _mainDirectionalLight.color;
        _mainDirectionalLight.DOColor(_nightColor, 0.5f);
        _mainDirectionalLight.intensity = 0.97f;
    }

    private void HandleDistrictEntry(District exit)
    {
        Debug.Log("Player entered a district " + exit._districtID);
    }

    private void HandleDistrictExit(District exit)
    {
        Debug.Log("Player EXITED a district " + exit._districtID);
    }

    void Start()
    {
        _inGameHoursTimer.OnTimerStop += () =>
        {
            HandleInGameHoursEnd();
            StartCoroutine(OffSetTextureXFixedSpeed(_offsetInterval));
            _inGameHoursTimer.Start();
        };

        _inGameHoursTimer.Start();
    }


    private IEnumerator OffSetTextureXFixedSpeed()
    {
        float startOffset = 0.1f;
        float endOffset = 0.4f;
        float startTime = 0f;
        float endTime = 24f;
        float elapsedTime = 0f;
        float duration = endTime - startTime;

        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration;
            float offset = Mathf.Lerp(startOffset, endOffset, t);
            _skyTexture.mainTextureOffset = new Vector2(offset, 0);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }


    private IEnumerator OffSetTextureXFixedSpeed(float duration)
    {
        float offsetSpeed = 0.01f;
        float elapsedTime = 0;
        while (elapsedTime < duration)
        {
            _skyTexture.mainTextureOffset += new Vector2(offsetSpeed * Time.deltaTime, 0);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        _timeOfDay = _skyTexture.mainTextureOffset.x;

        bool moonShouldBeActive = _nightTime - _timeOfDay <= 0.1f;
        bool dayShouldBeActive = _dayTime - _timeOfDay <= 0.1f;
        if (moonShouldBeActive)
        {
            _sun.SetActive(false);
            _moon.SetActive(true);
            GameEventsManager.Instance.OnSunDown();
        }
        if (dayShouldBeActive)
        {
            _sun.SetActive(true);
            _moon.SetActive(false);
        }
    }
    private void HandleInGameHoursEnd()
    {
        _inGameHours++;
        GameEventsManager.Instance.OnInGameHoursPassed();
        // Debug.Log($"In game hours {_inGameHours}");
    }

    // private void OnObjectiveUpdated(Objective objective, ObjectiveState state)
    // {
    //     switch (state)
    //     {
    //         case ObjectiveState.STARTED:
    //             HandleNewObjective(objective);
    //             break;
    //         case ObjectiveState.COMPLETED:
    //             HandleEndObjective();
    //             break;
    //         default:
    //             break;
    //     }
    // }

    // private void DeliverItem(ShopItemSO sO)
    // {
    //     Debug.Log("Scheduling delivery for item");
    // }

    private void NotifyPlayer(WeaponSO sO)
    {
        Debug.Log("Player Acquired gun");
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

    public void InstantiateQuestMarker(Vector3 position, string eventTitle = null)
    {
        if (eventTitle != null)
        {
            _questMarker._eventTitle = eventTitle;
        }
        _questMarker.Init(position);
    }

    // void HandleNewObjective(Objective objective)
    // {
    //     _objectiveText.text = objective._text;
    //     _objectiveCanvas.SetActive(true);
    //     _activeObjective = objective;
    //     if (objective.HasMarker)
    //     {
    //         InstantiateQuestMarker(objective._objectiveTriggerPosition, objective._text);
    //     }
    // }
    // void HandleEndObjective()
    // {
    //     _objectiveCanvas.SetActive(false);
    // }
}
