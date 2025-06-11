using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
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

    [FoldoutGroup("Console Debugging")]
    [SerializeField] GameObject _hud;
    [FoldoutGroup("Console Debugging")]
    [SerializeField] TextMeshProUGUI _consoleText;
    [FoldoutGroup("Console Debugging")]
    [SerializeField] TextMeshProUGUI _warningCountText;
    [FoldoutGroup("Console Debugging")]
    [SerializeField] TextMeshProUGUI _errorCountText;
    [FoldoutGroup("Console Debugging")]
    [SerializeField] TextMeshProUGUI _activeIndexText;
    [FoldoutGroup("Console Debugging")]
    [SerializeField] TextMeshProUGUI _logsCountText;

    [FoldoutGroup("Console Debugging")]
    public int _index = 0;

    [FoldoutGroup("Crafting Helpers")]
    public CraftingArea _debugCraftingArea;

    [FoldoutGroup("Crafting Helpers")]
    [Title("Storage")]
    public Storage _activeStorage;
    [Title("Spawn Position")]
    public Pos _debugSpawnPos;

    [FoldoutGroup("Crafting Helpers")]
    [Title("Spawn Items list")]
    [SerializeField] List<RawMaterialContainer> _debugItems = new();

    // [VerticalGroup]

    [Title("Crafting Buttons")]
    [FoldoutGroup("Crafting Helpers")]
    [BoxGroup("Crafting Helpers/Crafting Buttons")]
    [Button]
    public void SetActiveStorage()
    {
        GameObject storage = Instantiate(_activeStorage.gameObject, transform.position, Quaternion.identity);
        Player.Instance.EquipBackPack(storage.transform);
    }
    [FoldoutGroup("Crafting Helpers")]
    [BoxGroup("Crafting Helpers/Crafting Buttons")]
    [Button]
    public void AddDebugItems()
    {
        foreach (RawMaterialContainer item in _debugItems)
        {
            Player.Instance.AddItemToInventory(item._reference);
        }
    }
    [FoldoutGroup("Crafting Helpers")]
    [BoxGroup("Crafting Helpers/Crafting Buttons")]
    [Button]
    public void SpawnCraftingArea()
    {
        Instantiate(_debugCraftingArea, _debugSpawnPos.position, _debugSpawnPos.rotation == Vector3.zero ? Quaternion.identity : Quaternion.Euler(_debugSpawnPos.rotation));
    }
    [FoldoutGroup("Crafting Helpers")]
    [BoxGroup("Crafting Helpers/Crafting Buttons")]
    [Button]
    public void ConvertMaterial()
    {
        Instantiate(_debugCraftingArea, _debugSpawnPos.position, _debugSpawnPos.rotation == Vector3.zero ? Quaternion.identity : Quaternion.Euler(_debugSpawnPos.rotation));
    }


    [FoldoutGroup("ATTACK HELPERS")]
    [SerializeField] NpcCharacter _enemyPrefab;
    [FoldoutGroup("ATTACK HELPERS")]
    [SerializeField] int _spawnCount;
    [FoldoutGroup("ATTACK HELPERS")]
    [SerializeField] int _spawnSpread;

    [SerializeField] List<NpcCharacter> _spawnedEnemies;


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
    }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            DeleteLogs();

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
}
