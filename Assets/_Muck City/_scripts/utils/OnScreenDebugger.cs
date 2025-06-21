using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityUtils;


public struct LogMessage
{
    public string condition;
    public string stackTrace;
    public LogType type;

    public LogMessage(string _condition, string _stackTrace, LogType _type)
    {
        condition = _condition;
        stackTrace = _stackTrace;
        type = _type;
    }
}

public class OnScreenDebugger : MonoBehaviour
{
    public static OnScreenDebugger Instance { get; private set; }

    [TabGroup("Detect")]
    public ObjectDetector _detector;
    [TabGroup("Detect")]
    public LayerMask _detectLayer = new();

    [TabGroup("Console"), OnValueChanged("ToggleHud")]
    public bool _showConsoleLogs = true;
    [TabGroup("Console")]
    [SerializeField] GameObject _hud;
    [TabGroup("Console")]
    [SerializeField] TextMeshProUGUI _consoleText;
    [TabGroup("Console")]
    [SerializeField] TextMeshProUGUI _warningCountText;
    [TabGroup("Console")]
    [SerializeField] TextMeshProUGUI _errorCountText;
    [TabGroup("Console")]
    [SerializeField] TextMeshProUGUI _activeIndexText;
    [TabGroup("Console")]
    [SerializeField] TextMeshProUGUI _logsCountText;

    [TabGroup("Console")]
    public int _index = 0;

    [TabGroup("Crafting")]
    public CraftingArea _debugCraftingArea;

    [TabGroup("Crafting")]
    public Storage _activeStorage;
    [Tooltip("Spawn Position")]
    public Pos _debugSpawnPos;

    [TabGroup("Crafting")]
    [SerializeField] List<RawMaterialContainer> _debugItems = new();

    // [VerticalGroup]


    [SerializeField, TabGroup("ATTACK")] NpcCharacter _enemyPrefab;

    [SerializeField, TabGroup("ATTACK")] int _spawnCount;

    [SerializeField, TabGroup("ATTACK")] int _spawnSpread;

    [SerializeField, TabGroup("ATTACK")] List<NpcCharacter> _spawnedEnemies;

    [SerializeField, TabGroup("Quest")]
    Mission _mission = new();


    int _errorCount = 0;
    int _warningsCount = 0;
    int _logsCount = 0;
    List<LogMessage> _logs = new();

    LogType _activeLogType = LogType.Log;

    List<LogMessage> _activeList = new();


    void OnEnable()
    {
        Application.logMessageReceived += LogMessage;
    }

    void OnDisable()
    {
        Application.logMessageReceived -= LogMessage;
        if (_detector != null)
        {
            _detector = null;
        }
    }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            DeleteLogs();
            _detector = new(_detectLayer);
        }

        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        if (_debugItems.Count > 0)
        {
            foreach (RawMaterialContainer item in _debugItems)
            {
                item.SetData();
                Debug.Log("Spawned " + item._reference.name);
            }
        }
    }


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.RightBracket))
        {
            NextLog();
        }

        if (Input.GetKeyDown(KeyCode.LeftBracket))
        {
            PrevLog();
        }

        if (Input.GetKeyDown(KeyCode.Backslash))
        {
            ToggleHud();
        }

        if (Input.GetKeyDown(KeyCode.PageUp))
        {
            _activeLogType = _activeLogType == LogType.Log ? LogType.Error : LogType.Log;

            _consoleText.color = _activeLogType == LogType.Log ? Color.white : Color.red;

            _activeList = _logs.Filter(x => x.type == _activeLogType).ToList();

            _index = _activeList.Count - 1;
            // _activeListCountText.text = _activeList.Count.ToString();
            _consoleText.text = _activeList[_index].condition + "\n";
        }
    }

    [Button("Detect"), TabGroup("Detect")]
    void DetectObject()
    {
        DoorTrigger door = _detector.DetectObject<DoorTrigger>(_detector._position);
        Debug.Log("Detected" + door.name);
    }

    [Button("Reload Scene")]
    void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    #region Console
    void LogMessage(string condition, string stackTrace, LogType type)
    {
        switch (type)
        {
            case LogType.Error:
                _errorCount++;
                _errorCountText.text = _errorCount.ToString();
                _consoleText.color = Color.red;
                break;
            case LogType.Warning:
                _warningsCount++;
                _warningCountText.text = _warningsCount.ToString();
                break;
            case LogType.Log:
                _logsCount++;
                _logsCountText.text = _logsCount.ToString();
                _consoleText.color = Color.white;
                break;
            default:
                break;
        }

        _logs.Add(new LogMessage(condition, stackTrace, type));
        if (type != LogType.Warning)
        {
            _activeLogType = type;
        }
        _activeList = _logs.Filter(x => x.type == _activeLogType).ToList(); // Filter active logs by log type
        _consoleText.text = condition + "\n"; // Set active logs text to last log

        _index = _activeList.Count - 1; // Set  index to list count
        _activeIndexText.text = (_index + 1).ToString(); // Update active logs index text

        // _activeListCountText.text = _activeList.Count.ToString(); // Update active logs count



    }

    public void PrevLog()
    {
        if (_index == 0)
        {
            _index = _activeList.Count - 1;
        }
        else
        {
            _index--;
        }
        _consoleText.text = _activeList[_index].condition;
        _activeIndexText.text = (_index + 1).ToString();
    }


    public void NextLog()
    {
        if (_index == _activeList.Count - 1)
        {
            _index = 0;
        }
        else
        {
            _index++;
        }
        _consoleText.text = _activeList[_index].condition;
        _activeIndexText.text = (_index + 1).ToString();
    }



    public void Log(string log)
    {
        Debug.Log(log);
    }


    void ToggleHud()
    {
        _hud.SetActive(!_hud.activeSelf);
    }

    void DeleteLogs()
    {
        _consoleText.text = "";
    }

    #endregion

    #region Crafting
    [Button, TabGroup("Crafting")]
    public void SetActiveStorage()
    {
        GameObject storage = Instantiate(_activeStorage.gameObject, transform.position, Quaternion.identity);
        Player.Instance.EquipBackPack(storage.transform);
    }
    [Button, TabGroup("Crafting")]
    public void AddDebugItems()
    {
        foreach (RawMaterialContainer item in _debugItems)
        {
            Player.Instance.AddItemToInventory(item._reference);
        }
    }
    [Button, TabGroup("Crafting")]
    public void SpawnCraftingArea()
    {
        Instantiate(_debugCraftingArea, _debugSpawnPos.position, _debugSpawnPos.rotation == Vector3.zero ? Quaternion.identity : Quaternion.Euler(_debugSpawnPos.rotation));
    }
    [Button, TabGroup("Crafting")]
    public void ConvertMaterial()
    {
        Instantiate(_debugCraftingArea, _debugSpawnPos.position, _debugSpawnPos.rotation == Vector3.zero ? Quaternion.identity : Quaternion.Euler(_debugSpawnPos.rotation));
    }
    #endregion


    #region Quests
    [Button, TabGroup("Quest")]
    void SetObjective()
    {
        DomeManager.Instance.SetupMissionDisplay(_mission);
    }
    [Button, TabGroup("Detect")]
    void CreateDetector()
    {
        _detector = new(_detectLayer);
    }
    #endregion
    void OnDrawGizmosSelected()
    {
        if (_detector == null) return;
        Gizmos.DrawSphere(_detector._position, _detector._radius);
    }
}
