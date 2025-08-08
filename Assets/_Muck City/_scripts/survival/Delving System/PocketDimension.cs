using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ImprovedTimers;
using DynamicEnums;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Animations;
using UnityUtils;
using DG.Tweening;
using Systems.SceneManagement;
using UnityEngine.SceneManagement;
using System.Collections;

public enum PocketDimensionDeviceID
{
    DIMENSION_GATE = 0,
}



[Serializable]
public abstract class PocketDimensionDevice
{

    public PocketDimensionDeviceID _deviceType;
    public string _interactionPrompt = "Interact";
    public Pos _actionTextPos;
    [HideInInspector] public ObjectDetector _objectDetector;
    public PocketDimension _pocketDimension;



    public abstract void Init(PocketDimension pocketDimension);
    public abstract void Interact(PocketDimension pocketDimension);
    public abstract void PrepareInteraction(PocketDimension pocketDimension);
    public abstract void HideInteractionPrompt();


    public void ShowActionText(string text = null)
    {
        if (text.IsNullOrEmpty())
        {
            _pocketDimension._actionText.SetText(_interactionPrompt);
        }
        else
        {
            _pocketDimension._actionText.SetText(text);
        }
        _pocketDimension._actionText.transform.position = _pocketDimension.transform.TransformPoint(_actionTextPos.position);
        _pocketDimension._actionText.gameObject.SetActive(true);
    }

    public void GetItem(QuestItemStruct itemData)
    {
        IFindable item = _objectDetector.DetectFindable<IFindable>(itemData._position, itemData._radius);
    }
}

public class PocketDimensionGate : PocketDimensionDevice
{
    public SceneData _pocketDimensionScene;
    public override void HideInteractionPrompt()
    {

    }

    public override void Init(PocketDimension pocketDimension)
    {
        Debug.Log("Init" + _deviceType);
        _pocketDimension = pocketDimension;
    }
    public override void Interact(PocketDimension pocketDimension)
    {

        PodDoor door = _pocketDimension._itemFinder.GetItem<PodItemName>(PodItemName.PocketDoor).transform.GetComponent<PodDoor>();


        OpenPocketDimension(() =>
        {
            _pocketDimension.SnapDimensionToPod();
            door._isLocked = false;
            door.Open();
            Debug.Log($"<color=green> Pocket Dimension Ready</color>");
        });
    }


    public override void PrepareInteraction(PocketDimension pocketDimension)
    {
        ShowActionText("Open Pocket");
    }
    async void OpenPocketDimension(Action OnComplete = null)
    {

        SceneGroup sceneToLoad = new()
        {
            GroupName = _pocketDimensionScene.Name,
            Scenes = new() { _pocketDimensionScene }
        };
        Player.Instance.SetInteractableObject(null);
        await SceneLoader.Instance.LoadSceneGroup(sceneToLoad);
        OnComplete?.Invoke();
    }
}

public struct PocketDimensionData
{
    public bool _expanded;

    public bool _playerIsInPocketDimension;
    public PocketDimensionData(bool expanded, bool playerIsInDimension)
    {
        _expanded = expanded;
        _playerIsInPocketDimension = playerIsInDimension;
    }
}

public class PocketDimension : Interactable
{
    public GameObject _pocketDimensionMiniaturePrefab;
    public GameObject _pocketDimensionMiniature;

    [SerializeField] ParentConstraint _parentConstraint;

    [SerializeField] Pos _miniaturePos;

    [SerializeField] Transform _outerStructure;

    [SerializeField] GameObject _landingAreaHelper;


    public Pos _dimensionSnappingPoint;

    public bool _lockedInPlace = false;

    public bool _canExpand = false;
    public bool _expanded = false;

    public bool _playerIsInPocketDimension = true;

    [SerializeReference] public List<PocketDimensionDevice> _devices;

    public ChildrenItemFinder _itemFinder;

    PocketDimensionDevice _currentDevice;


    // [SerializeField, HideInInspector] Material _previewMat;
    // [SerializeField, HideInInspector] List<Material> _childrenMat;
    // [SerializeField, HideInInspector] List<Transform> _outerChildren;

    void Awake()
    {
        _pocketDimensionMiniature = GameObject.Instantiate(_pocketDimensionMiniaturePrefab);


        _itemFinder = GetComponent<ChildrenItemFinder>();
        _itemFinder.SetEnumType<PodItemName>();
        _itemFinder.SearchChildrenIterative(transform);
        ParentToMiniature();
        LoadPersistentData();
        if (_playerIsInPocketDimension)
        {
            return;
        }
        //TODO WHEN YOU ADD SAVING REFACTOR TO ONLY SET INACTIVE IF PLYER IS NOT IN POCKET DIMENSION
        _outerStructure.gameObject.SetActive(false);

        //* DISABLE LANDING AREA HELPER
        _landingAreaHelper.SetActive(false);

        //* DISABLE POCKET DIMENSION
        gameObject.SetActive(false);


    }



    void OnDisable()
    {
        AutoSave();
    }

    public override void Start()
    {
        base.Start();
        //* INITIALIZE DEVICES
        foreach (PocketDimensionDevice device in _devices)
        {
            device.Init(this);
        }

    }

    [Button]
    void AutoSave()
    {
        PocketDimensionData data = new(_expanded, _playerIsInPocketDimension);

        ES3.Save("POCKET_DIMENSION_DATA", data);
        Debug.Log("Saved data expanded " + data._expanded + " is in dimension " + data._playerIsInPocketDimension);
    }

    void LoadPersistentData()
    {
        if (!ES3.KeyExists("POCKET_DIMENSION_DATA")) return;
        PocketDimensionData data = (PocketDimensionData)ES3.Load("POCKET_DIMENSION_DATA");
        _expanded = data._expanded;
        _playerIsInPocketDimension = data._playerIsInPocketDimension;
        Debug.Log("Loaded data expanded " + data._expanded + " is in dimension " + data._playerIsInPocketDimension);
    }


    PocketDimensionDevice GetDevice(string deviceString)
    {
        PocketDimensionDeviceID deviceType = (PocketDimensionDeviceID)System.Enum.Parse(typeof(PocketDimensionDeviceID), deviceString);
        return _devices.Find(d => d._deviceType == deviceType);
    }

    public override void Interact()
    {
        _currentDevice.Interact(this);
    }

    public void PrepareDeviceInteraction(string deviceString)
    {

        PocketDimensionDevice device = GetDevice(deviceString);
        _currentDevice = device;
        Player.Instance.SetInteractableObject(this);
        device.PrepareInteraction(this);
    }

    public override void HideInteractionPrompt()
    {
        base.HideInteractionPrompt();
        _currentDevice.HideInteractionPrompt();
        Player.Instance.SetInteractableObject(null);
    }

    // void OnTriggerEnter(Collider other)
    // {
    //     if (!_lockedInPlace)
    //     {
    //         HudManager.Instance.UseStatusText("Exit Spawn Area", Color.red, false);
    //     }
    // }

    // void OnTriggerExit(Collider other)
    // {
    //     if (!_lockedInPlace)
    //     {
    //         HudManager.Instance.HideStatusText();
    //     }
    // }

    public void Expand()
    {
        if (_canExpand && _lockedInPlace)
        {
            _outerStructure.gameObject.SetActive(true);
            _landingAreaHelper.SetActive(false);
            _pocketDimensionMiniature.SetActive(false);
            _expanded = true;
        }

        else
        {
            Debug.Log($"Either not locked in place or cannot expand, Can Expand {_canExpand}, Locked in place {_lockedInPlace}");
        }

    }

    public void Shrink()
    {
        _lockedInPlace = false;
        _expanded = false;
        gameObject.SetActive(false);
    }

    public void ToggleExpansionWarning(bool show)
    {
        if (show)
        {
            HudManager.Instance.UseStatusText("Exit Spawn Area", Color.red, false);
            _canExpand = false;
        }

        else
        {
            HudManager.Instance.HideStatusText();
            _canExpand = true;
        }
    }
    public void TogglePlayerPresence(bool state)
    {
        _playerIsInPocketDimension = state;
        AutoSave();
    }

    public void HandlePlaceMent()
    {
        //* ENABLE POCKET DIMENSION MINIATURE
        _pocketDimensionMiniature.SetActive(true);

        //* DISABLE OUTER STRUCTURE
        _outerStructure.gameObject.SetActive(false);

        //* Enable Self
        gameObject.SetActive(true);

        //* Enable Landing Area
        _landingAreaHelper.SetActive(true);

        //* Locked in place
        _lockedInPlace = true;
        Debug.Log("Locked in place");
    }
    [Button]
    void ParentToMiniature()
    {
        if (_parentConstraint != null && _pocketDimensionMiniature != null)
        {
            //* POSITION MINIATURE EXACTLY IN THE CENTER OF THE PARENT OBJECT
            _pocketDimensionMiniature.transform.position = transform.TransformPoint(_miniaturePos.position);
            ConstraintSource newSource = new()
            {
                sourceTransform = _pocketDimensionMiniature.transform,
                weight = 1.0f
            };

            int sourceIndex = _parentConstraint.sourceCount;

            _parentConstraint.AddSource(newSource);
            _parentConstraint.SetSource(sourceIndex, newSource);

            //* Maintain the relative position and rotation of the parent object   
            _parentConstraint.SetTranslationOffset(sourceIndex, _parentConstraint.transform.position - _pocketDimensionMiniature.transform.position);
            _parentConstraint.SetRotationOffset(sourceIndex, (_parentConstraint.transform.rotation * Quaternion.Inverse(_pocketDimensionMiniature.transform.rotation)).eulerAngles);


            // Optionally, activate the constraint
            _parentConstraint.constraintActive = true;
        }
    }


    [Button]
    public virtual void SnapDimensionToPod()
    {
        PocketDimensionManager pocketDimensionManager = GameObject.FindFirstObjectByType<PocketDimensionManager>();
        pocketDimensionManager.transform.position = transform.TransformPoint(_dimensionSnappingPoint.position);
    }

    // [Button]
    // void GetChildrenMat()
    // {
    //     //Get ALL Outer Children
    //     for (int i = 0; i < _outerStructureParent.childCount; i++)
    //     {
    //         _outerChildren.Add(_outerStructureParent.transform.GetChild(i));
    //         if (_outerChildren[i].childCount > 0)
    //         {
    //             for (int b = 0; b < _outerChildren[i].childCount; b++)
    //             {
    //                 _outerChildren.Add(_outerChildren[i].transform.GetChild(b));
    //             }
    //         }
    //     }
    //     for (int i = 0; i < _outerChildren.Count; i++)
    //     {
    //         _childrenMat.Add(_outerChildren[i].GetComponent<MeshRenderer>().sharedMaterial);
    //     }
    // }
    // [Button]
    // public void ToggleOuterStructMat(bool showPreview)
    // {
    //     if (_childrenMat.Count == 0)
    //     {
    //         GetChildrenMat();
    //     }
    //     if (showPreview)
    //     {
    //         for (int i = 0; i < _outerChildren.Count; i++)
    //         {
    //             _outerChildren[i].GetComponent<MeshRenderer>().material = _previewMat;
    //         }
    //     }

    //     else
    //     {
    //         for (int i = 0; i < _outerChildren.Count; i++)
    //         {
    //             _outerChildren[i].GetComponent<MeshRenderer>().material = _childrenMat[i];
    //         }
    //     }

    // }

    public void OnAllChildrenFound()
    {
        Debug.Log($"<color=green> All Children Found </color>");
    }
}



public static class SceneAdder
{
    /// <summary>
    /// Loads a scene additively and invokes a callback when done.
    /// </summary>
    /// <param name="sceneName">Scene name in Build Settings</param>
    /// <param name="onLoaded">Callback after scene finishes loading</param>
    public static void LoadSceneAdditive(string sceneName, Action onLoaded = null)
    {
        CoroutineRunner.Instance.StartCoroutine(LoadSceneRoutine(sceneName, onLoaded));
    }

    private static IEnumerator LoadSceneRoutine(string sceneName, Action onLoaded)
    {
        AsyncOperation op = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);

        if (op == null)
        {
            Debug.LogError($"SceneLoader: Could not load scene '{sceneName}'. Check Build Settings.");
            yield break;
        }

        // Wait until loading finishes
        while (!op.isDone)
            yield return null;

        onLoaded?.Invoke();
    }
}
