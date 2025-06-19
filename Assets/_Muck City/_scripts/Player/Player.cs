using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DialogueEditor;
using ImprovedTimers;
using Invector;
using Invector.vCamera;
using Invector.vCharacterController;
using Invector.vItemManager;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityUtils;



public class Player : MonoBehaviour, IHavePersistentData
{
    public static Player Instance { get; private set; }
    public GenericInput _interactionInput = new("E", "Y", "Y");
    public GenericInput _showPhoneInput = new("C", "Y", "Y");
    public GenericInput _endConvoInput = new("C", "Y", "Y");
    public GenericInput _exitInput = new("C", "Y", "Y");
    public GenericInput _dialogueOneInput = new("C", "Y", "Y");
    public GenericInput _dialogueTwoInput = new("C", "Y", "Y");

    public float _interactionRange = 1f;

    [SerializeField] bool _isInDialogue;

    [SerializeField] Vehicle _currentVehicle;

    [SerializeField] private bool _isRunning = true;

    public vThirdPersonCameraListData CameraStateList;

    [SerializeField] LayerMask _interactionLayerMask = new();

    [SerializeField] NPCConversation _activeConversation;

    [SerializeField] Transform _backPackHolder;

    vThirdPersonController _vThirdPersonController;
    vThirdPersonInput _vThirdPersonInput;

    public vThirdPersonCamera _vThirdPersonCamera;

    vItemManager _inventory;
    string _lastBlendedState;

    [SerializeField] GameObject _phoneModel;
    [SerializeField] Camera _phoneCamera;
    [SerializeField] Camera _defaultCamera;
    [SerializeField] IInteractable _lastInteractable;
    [SerializeField] CraftingArea _activeCraftingArea;
    [SerializeField] Shop _activeShop;

    public BackPack _hotStorage;
    public Storage _activeStorage;

    public bool IsInVehicle => _currentVehicle != null;



    public Observer<bool> _isPhoneShowing = new(false);


    CancellationTokenSource cts = new();

    public bool ShouldAutoSave { get => AutoSaveManager.ShouldAutoSave(SaveAble.PLAYER); }

    public SaveAble SAVE_ID => SaveAble.PLAYER;

    private PlayerSaveData _playerSaveData;

    [SerializeField] List<IInteractable> _closestInteractables = new();

    CountdownTimer _detectionTimer;

    float _detectionRate = 0.2f;



    void OnEnable()
    {
        GameEventsManager.OnConversationStartedEvent += OnEnterConversation;
        GameEventsManager.OnConversationEndEvent += OnExitConversation;
        GameEventsManager.OnCutSceneStartEvent += OnCutSceneStart;
        GameEventsManager.OnCutSceneEndEvent += OnCutSceneEnd;
        GameEventsManager.OnCraftItemEvent += AddItemToInventory;
        AutoSaveManager.OnShouldAutoSave += AutoSave;
    }



    void OnDisable()
    {
        GameEventsManager.OnConversationStartedEvent -= OnEnterConversation;
        GameEventsManager.OnConversationEndEvent -= OnExitConversation;
        GameEventsManager.OnCutSceneStartEvent -= OnCutSceneStart;
        GameEventsManager.OnCutSceneEndEvent += OnCutSceneEnd;
        GameEventsManager.OnCraftItemEvent -= AddItemToInventory;
        AutoSaveManager.OnShouldAutoSave -= AutoSave;

        AutoSave();


        _isRunning = false;
        cts.Cancel();
        _vThirdPersonInput.onUpdate -= CheckForTriggerAction;

        if (_lastBlendedState != null)
        {
            CameraStateList.tpCameraStates.Remove(CameraStateList.tpCameraStates.Find(state => state.Name == _lastBlendedState));
        }
    }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            _vThirdPersonController = GetComponent<vThirdPersonController>();
            _vThirdPersonInput = GetComponent<vThirdPersonInput>();
            _inventory = GetComponent<vItemManager>();

            LoadPersistentData();
            // DontDestroyOnLoad(gameObject);
        }

        else
        {
            Destroy(gameObject);
        }


    }

    void Start()
    {
        _vThirdPersonCamera = FindFirstObjectByType<vThirdPersonCamera>();
        _defaultCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        UniversalAdditionalCameraData universalAdditionalCameraData = _defaultCamera.GetComponent<UniversalAdditionalCameraData>();
        universalAdditionalCameraData.cameraStack.Add(_phoneCamera);
        _vThirdPersonInput.onUpdate += CheckForTriggerAction;

        transform.SetPositionAndRotation(_playerSaveData._position.position, Quaternion.Euler(_playerSaveData._position.rotation));

        _detectionTimer = new(_detectionRate);
        _detectionTimer.OnTimerStop += () =>
        {
            EnvironmentInteraction();
            _detectionTimer.Start();
        };

        _detectionTimer.Start();
    }

    private void CheckForTriggerAction()
    {
        if (_showPhoneInput.GetButtonDown())
        {
            ShowPhone();
        }
        if (_interactionInput.GetButtonDown())
        {
            Interact();
        }
        if (_isInDialogue)
        {
            if (_endConvoInput.GetButtonDown())
            {
                ConversationManager.Instance.EndConversation();
            }

            if (_dialogueOneInput.GetButtonDown())
            {
                // Debug.Log("One");
                ConversationManager.Instance.m_currentSelectedIndex = 0;
                ConversationManager.Instance.PressSelectedOption();

            }
            else if (_dialogueTwoInput.GetButtonDown())
            {
                // Debug.Log("Two");
                ConversationManager.Instance.m_currentSelectedIndex = 1;
                ConversationManager.Instance.PressSelectedOption();
            }
        }

        if (_exitInput.GetButtonDown())
        {
            if (_activeCraftingArea != null)
            {
                _activeCraftingArea.Close();
                _activeCraftingArea = null;
                // UnlockPlayerControls();
                Invoke(nameof(UnlockPlayerControls), 0.1f);
            }

            else if (_activeShop != null)
            {
                _activeShop.ExitShop();
                _activeShop = null;
                // UnlockPlayerControls();
                Invoke(nameof(UnlockPlayerControls), 0.1f);
            }
        }

    }

    // void FixedUpdate()
    // {
    //     EnvironmentInteraction();
    // }


    #region SAVE/LOAD
    public void AutoSave()
    {
        AutoSaveManager.Autosave(SaveAble.PLAYER, new PlayerSaveData(this));
    }

    public void LoadPersistentData()
    {
        PlayerSaveData data = (PlayerSaveData)AutoSaveManager.Load(SAVE_ID);
        _playerSaveData = data;
    }

    public void TriggerAutoSave()
    {
        throw new NotImplementedException();
    }
    #endregion


    void Interact()
    {
        if (_lastInteractable != null)
        {
            _lastInteractable.Interact();
        }
    }
    void ShowPhone()
    {
        if (!_phoneCamera.gameObject.activeSelf)
        {
            _phoneCamera.gameObject.SetActive(true);
            _phoneModel.SetActive(true);
            _vThirdPersonInput.SetLockBasicInput(true);
        }

        else
        {
            _phoneCamera.gameObject.SetActive(false);
            _phoneModel.SetActive(false);
            _vThirdPersonInput.SetLockBasicInput(false);
        }
        _isPhoneShowing.Value = _phoneCamera.gameObject.activeSelf;
    }


    private void OnCutSceneStart(TimelinePlayer player)
    {

        _vThirdPersonInput.lockMoveInput = true;
        _vThirdPersonController.StopCharacter();
        // _vThirdPersonCamera.CameraStateList.tpCameraStates.Add(player._startingCamState);
        // _vThirdPersonInput.customlookAtPoint = player._startingCamState.lookPoints[0].pointName;
        // _vThirdPersonInput.ChangeCameraState(player._startingCamState.Name);
        // _vThirdPersonCamera.ChangePoint(player._startingCamState.lookPoints[0].pointName);


        // _vThirdPersonInput.ChangeCameraStateWithLerp(player._startingCamState.Name);
        // _vThirdPersonCamera.ChangePoint(player._startingCamState.lookPoints[0].pointName);
        // _vThirdPersonCamera.gameObject.SetActive(false);
    }

    public async Task PrepareForCutScene(TimelinePlayer player, float delay = 1f)
    {
        _vThirdPersonInput.lockMoveInput = true;
        _vThirdPersonController.StopCharacter();

        // _vThirdPersonCamera.CameraStateList.tpCameraStates.Add(player._startingCamState);

        // _vThirdPersonCamera.ChangeState(player._startingCamState.Name);

        // // _vThirdPersonCamera.ChangeState("INTRO_TO_HAZMAT_BILL");
        // Debug.Log("cam is in" + _vThirdPersonCamera.currentState.Name);


        // // _vThirdPersonCamera.ChangePoint(player._startingCamState.lookPoints[0].pointName);

        // _lastBlendedState = player._startingCamState.Name;

        await Task.Delay((int)(delay * 1000));
        _vThirdPersonCamera.gameObject.SetActive(false);
    }

    private void OnCutSceneEnd()
    {
        _vThirdPersonInput.lockMoveInput = false;
        // _vThirdPersonCamera.gameObject.SetActive(true);
        // _vThirdPersonCamera.ChangeStateList(DefaultCameraStateList);
        // _vThirdPersonCamera.ChangeState("Default");
        // CameraStateList.tpCameraStates.Remove(CameraStateList.tpCameraStates.Find(state => state.Name == _lastBlendedState));
        // _lastBlendedState = null;
    }

    private void OnExitConversation()
    {
        _vThirdPersonInput.lockMoveInput = false;
        _isInDialogue = false;
    }

    private void OnEnterConversation(NPCConversation conversation)
    {
        // OnScreenDebugger.Instance.Log("Conversation started with " + conversation);
        _vThirdPersonInput.lockMoveInput = true;
        _isInDialogue = true;
        _activeConversation = conversation;
        // _vThirdPersonCamera.ChangeStateList(CameraStateList);
    }


    [Button]
    public void LockPlayerControls()
    {
        // _vThirdPersonInput.lockInput = true;
        _vThirdPersonInput.SetLockAllInput(true);
        _vThirdPersonInput.ShowCursor(true);
        _vThirdPersonInput.LockCursor(true);
        _vThirdPersonCamera.LockCamera = true;
    }
    [Button]
    public void UnlockPlayerControls()
    {
        // _vThirdPersonInput.lockInput = true;
        _vThirdPersonInput.SetLockAllInput(false);
        _vThirdPersonInput.ShowCursor(false);
        _vThirdPersonInput.LockCursor(false);
        _vThirdPersonCamera.LockCamera = false;
    }


    public async void MoveToPosition(Vector3 targetPosition, bool rotateOnComplete = false, Action OnDestinationReached = null)
    {
        _vThirdPersonInput.lockMoveInput = true;
        _vThirdPersonController.MoveToPosition(targetPosition);
        await WatchForDestinationReached(targetPosition);
        _vThirdPersonInput.lockMoveInput = false;
        if (_isRunning)
        {
            if (rotateOnComplete)
            {
                Debug.Log("rotated on complete");
                _vThirdPersonController.RotateToDirection(transform.forward);
            }
        }
        OnDestinationReached?.Invoke();
    }
    public async void MoveToPosition(Transform targetPosition, bool rotateOnComplete = false, Action OnDestinationReached = null)
    {
        _vThirdPersonInput.lockMoveInput = true;
        _vThirdPersonController.MoveToPosition(targetPosition.transform.position);
        await WatchForDestinationReached(targetPosition.transform.position);
        _vThirdPersonInput.lockMoveInput = false;
        OnDestinationReached?.Invoke();
        if (rotateOnComplete)
        {
            _vThirdPersonController.RotateToDirection(targetPosition.forward);
        }
    }


    public void StopAutoMove()
    {
        _isRunning = false;
        Invoke(nameof(ToggleIsRunning), 0.1f);
    }

    void ToggleIsRunning()
    {
        _isRunning = !_isRunning;
    }
    public void ToggleInputLock()
    {
        _vThirdPersonInput.lockMoveInput = !_vThirdPersonInput.lockMoveInput;
    }

    async Task WatchForDestinationReached(Vector3 targetPosition)
    {

        gameObject.GetComponent<IInteractable>();
        while (_isRunning && (targetPosition - transform.position).magnitude > 0.5f)
        {
            if (cts.IsCancellationRequested)
            {
                break;
            }
            Debug.Log("Distance: " + (targetPosition - transform.position).magnitude);
            await Task.Yield();
        }
    }




    public void EnterVehicleMode(Vehicle vehicle)
    {
        Debug.Log("starting ride " + vehicle.name);
        _currentVehicle = vehicle;
        _vThirdPersonController.enabled = false;
        _vThirdPersonInput.enabled = false;
        _vThirdPersonCamera.gameObject.SetActive(false);
        gameObject.SetActive(false);
    }

    public void ExitVehicleMode()
    {
        gameObject.SetActive(true);
        _vThirdPersonController.enabled = true;
        _vThirdPersonInput.enabled = true;
        _vThirdPersonCamera.gameObject.SetActive(true);
        transform.position = _currentVehicle._exitPoint.transform.position;
        _currentVehicle = null;
    }



    // public void EnvironmentInteraction()
    // {

    //     Collider closestCollider = null;
    //     float closestDistance = _interactionRange + 10f;

    //     Collider[] hitColliders = Physics.OverlapSphere(transform.position + Vector3.up, _interactionRange, _interactionLayerMask);

    //     foreach (Collider hitCollider in hitColliders)
    //     {
    //         float distance = Vector3.Distance(transform.position, hitCollider.transform.position);
    //         if (distance < closestDistance)
    //         {
    //             // Debug.Log(hitCollider.name);
    //             closestDistance = distance;
    //             closestCollider = hitCollider;
    //         }
    //     }

    //     if (closestCollider != null)
    //     {
    //         if (closestCollider.gameObject.GetComponent<IInteractable>() != null)
    //         {
    //             Debug.Log(closestCollider.name + " is interactable");
    //             _lastInteractable = closestCollider.gameObject.GetComponent<IInteractable>();
    //             if (_lastInteractable != null && _lastInteractable.CanInteract)
    //             {
    //                 _lastInteractable.DrawAttention();
    //             }
    //         }
    //     }

    //     if (hitColliders.Length == 0)
    //     {
    //         // HudManager.Instance.HideInteractPrompt();
    //         _lastInteractable = null;
    //     }

    // }
    public void EnvironmentInteraction()
    {

        Collider[] hitColliders = Physics.OverlapSphere(transform.position + Vector3.up, _interactionRange, _interactionLayerMask);
        if (hitColliders.Length > 0)
        {
            foreach (Collider hitCollider in hitColliders)
            {
                IInteractable interactable = hitCollider.GetComponent<IInteractable>();
                if (!interactable.IsHighlighted)
                {
                    interactable.ToggleDrawAttention();
                    _closestInteractables.Add(interactable);
                    Debug.Log("added interactable: " + interactable.GameObject.name);
                }
            }

            // if (hitColliders.Length < _closestInteractables.Count)
            // {
            //     for (int i = _closestInteractables.Count - 1; i >= 0; i--)
            //     {
            //         IInteractable interactable = _closestInteractables[i];
            //         if (Vector3.Distance(transform.position, interactable.GameObject.transform.position) > _interactionRange)
            //         {
            //             interactable.ToggleDrawAttention();
            //             _closestInteractables.RemoveAt(i);
            //         }
            //     }
            // }
        }

        if (_closestInteractables.Count > 0)
        {
            for (int i = _closestInteractables.Count - 1; i >= 0; i--)
            {
                IInteractable interactable = _closestInteractables[i];
                if (Vector3.Distance(interactable.GameObject.transform.position, transform.position) > _interactionRange)
                {
                    interactable.ToggleDrawAttention();
                    _closestInteractables.RemoveAt(i);
                }
            }
        }

        if (hitColliders.Length == 0)
        {
            if (_closestInteractables.Count > 0)
            {
                for (int i = _closestInteractables.Count - 1; i >= 0; i--)
                {
                    IInteractable interactable = _closestInteractables[i];
                    interactable.ToggleDrawAttention();
                    _closestInteractables.RemoveAt(i);
                }
            }
        }

    }

    IEnumerator ClearInteractablesAfterDelay(IInteractable interactable)
    {
        Debug.Log("Deactivating Item" + interactable.GameObject.name);
        yield return new WaitForSeconds(1f);
        interactable.ToggleDrawAttention();
        _closestInteractables.Remove(interactable);
        yield return null;
    }


    // void OnDrawGizmos()
    // {
    //     Gizmos.color = Color.green;
    //     Gizmos.DrawWireSphere(transform.position + Vector3.up, _interactionRange);
    // }

    public void SwitchControlScheme()
    {
        Debug.Log("Switching control scheme");
        vShooterMeleeInput vShooterMeleeInput = GetComponent<vShooterMeleeInput>();
    }

    public void EnterCraftingArea(CraftingArea craftingArea)
    {
        _activeCraftingArea = craftingArea;
        LockPlayerControls();
    }
    public void EnterShop(Shop shop)
    {
        _activeShop = shop;
        LockPlayerControls();
    }


    #region Inventory Usage

    public void CheckIfItemInInventory()
    {
        // _inventory.ContainItem();

    }
    public void UseItem(vItem item)
    {
        switch (item.type)
        {
            case vItemType.Consumable:
                Debug.Log("Consumable item: " + item.name);
                Instantiate(item.dropObject, transform.position, Quaternion.identity);
                break;
            case vItemType.ShooterWeapon:
                break;
            default:
                Debug.LogWarning("Item type not handled: " + item.type);
                break;
        }
    }
    public void EquipItem(vEquipArea equipArea, vItem item)
    {
        // switch (item.type)
        // {
        //     case vItemType.Consumable:
        //         Debug.Log("Consumable item: " + item.name);
        //         Instantiate(item.dropObject, transform.position, Quaternion.identity);
        //         break;
        //     case vItemType.ShooterWeapon:
        //         break;
        //     default:
        //         Debug.LogWarning("Item type not handled: " + item.type);
        //         break;
        // }
    }

    public void EquipBackPack(Transform backPack)
    {
        backPack.SetParent(_backPackHolder);
        backPack.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
        BackPack b = backPack.GetComponent<BackPack>();
        _hotStorage = b;
        _activeStorage = b;

    }

    public void AddItemToInventory(ItemReference item)
    {
        _inventory.AddItem(item);
        if (_hotStorage != null)
        {
            _hotStorage.AddItem(item);
        }

    }

    public void SetInteractableObject(IInteractable interactable)
    {
        _lastInteractable = interactable;
    }

    public bool IsItemInInventory(int id)
    {
        if (_activeStorage == null)
        {
            return false;
        }
        bool hasItem = _activeStorage.IsItemInInventory(id);
        return hasItem;
    }



    #endregion

    void OnDrawGizmosSelected()
    {
        Gizmos.DrawSphere(transform.position + Vector3.up, _interactionRange);
    }
}
