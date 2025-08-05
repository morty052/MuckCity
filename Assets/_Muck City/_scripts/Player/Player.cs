using System;
using System.Threading.Tasks;
using DialogueEditor;
using Invector;
using Invector.vCamera;
using Invector.vCharacterController;
using Invector.vCharacterController.vActions;
using Invector.vItemManager;
using Invector.vShooter;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;


public interface IDoQuickAction
{
    public void DoQuickAction();
}

public class Player : MonoBehaviour, IHavePersistentData
{
    public static Player Instance { get; private set; }

    vThirdPersonController _vThirdPersonController;
    [HideInInspector] public vShooterManager _vShooterManager;
    public vThirdPersonInput _vThirdPersonInput;
    public vShooterMeleeInput _vShooterMeleeInput;

    vGenericAnimation _vGenericAnimation;


    vItemManager _itemManager;

    Vehicle _currentVehicle;

    private PlayerSaveData _playerSaveData;



    // CancellationTokenSource cts = new();

    string _lastBlendedState;


    [TabGroup("Inputs")] public GenericInput _interactionInput = new("E", "Y", "Y");

    [TabGroup("Inputs")] public GenericInput _acceptInput = new("Y", "Y", "Y");

    [TabGroup("Inputs")] public GenericInput _showPhoneInput = new("C", "Y", "Y");
    [TabGroup("Inputs")] public GenericInput _endConvoInput = new("C", "Y", "Y");
    [TabGroup("Inputs")] public GenericInput _exitInput = new("C", "Y", "Y");
    [TabGroup("Inputs")] public GenericInput _dialogueOneInput = new("C", "Y", "Y");
    [TabGroup("Inputs")] public GenericInput _dialogueTwoInput = new("C", "Y", "Y");
    [TabGroup("Inputs")] public bool _isUsingAltInput = false;
    [SerializeField, TabGroup("Inputs")] InputActionAsset _inputAsset;
    [TabGroup("Inputs")] public AltInput _altInput;
    [TabGroup("Interaction")] public float _interactionRange = 1f;
    [SerializeField, TabGroup("Interaction")] LayerMask _interactionLayerMask = new();
    [SerializeField, TabGroup("Interaction")] LayerMask _defaultLayerMask = new();
    [SerializeField, TabGroup("Interaction")] float _detectionRate = 0.2f;
    [SerializeField, TabGroup("Interaction")] IInteractable _lastInteractable;
    [SerializeField, TabGroup("Interaction")] InteractionSystem _interactionSystem;

    [SerializeField, TabGroup("State")] private bool _isRunning = true;
    [SerializeField, TabGroup("State")] bool _isInDialogue;
    [SerializeField, TabGroup("State")] NPCConversation _activeConversation;
    [SerializeField, TabGroup("State")] CraftingArea _activeCraftingArea;
    [SerializeField, TabGroup("State")] Shop _activeShop;
    [TabGroup("Components")] public vThirdPersonCameraListData CameraStateList;
    [TabGroup("Components")] public Transform _combatHelperSphere;
    [TabGroup("Components")] public vFootStep _vFootStep;
    [TabGroup("Components")] public vThirdPersonCamera _vThirdPersonCamera;
    [SerializeField, TabGroup("Components")] Camera _defaultCamera;
    [TabGroup("Body Snaps")] public Transform _backPackHolder;
    [TabGroup("Body Snaps")] public Transform _headHolder;
    [TabGroup("Body Snaps")] public Transform _delveBuddySlot;
    [SerializeField, TabGroup("Phone")] GameObject _phoneModel;
    [SerializeField, TabGroup("Phone")] Camera _phoneCamera;
    [TabGroup("Phone")] public Observer<bool> _isPhoneShowing = new(false);

    [TabGroup("Settings")] public SaveAble SAVE_ID => SaveAble.PLAYER;
    [SerializeField, TabGroup("Settings")] bool _useLastSavedPosition = false;
    [SerializeField, TabGroup("Settings")] float _underGroundThreshold = 0;
    [SerializeField, TabGroup("Effects")] PostProcessManager _postProcessManager;

    public bool ShouldAutoSave { get => AutoSaveManager.ShouldAutoSave(SaveAble.PLAYER); }
    public bool IsInVehicle => _currentVehicle != null;

    public bool IsUnderGround => transform.position.y < _underGroundThreshold;
    private bool _isSubscribedToThirdPersonInputs = false;

    public IDoQuickAction _activeQuickAction;
    void OnEnable()
    {
        GameEventsManager.OnConversationStartedEvent += OnEnterConversation;
        GameEventsManager.OnConversationEndEvent += OnExitConversation;
        GameEventsManager.OnCutSceneStartEvent += OnCutSceneStart;
        GameEventsManager.OnCutSceneEndEvent += OnCutSceneEnd;

        AltInput.OnToggleEquipmentWheel += OnToggleEquipmentWheel;
        AutoSaveManager.OnShouldAutoSave += AutoSave;

        if (!_isSubscribedToThirdPersonInputs)
        {
            _vThirdPersonInput.onUpdate += CheckForTriggerAction;
        }
    }

    void OnDisable()
    {
        GameEventsManager.OnConversationStartedEvent -= OnEnterConversation;
        GameEventsManager.OnConversationEndEvent -= OnExitConversation;
        GameEventsManager.OnCutSceneStartEvent -= OnCutSceneStart;
        GameEventsManager.OnCutSceneEndEvent -= OnCutSceneEnd;
        AutoSaveManager.OnShouldAutoSave -= AutoSave;

        AltInput.OnToggleEquipmentWheel -= OnToggleEquipmentWheel;

        AutoSave();


        _isRunning = false;
        // cts.Cancel();
        _vThirdPersonInput.onUpdate -= CheckForTriggerAction;
        _isSubscribedToThirdPersonInputs = false;

        if (_lastBlendedState != null)
        {
            CameraStateList.tpCameraStates.Remove(CameraStateList.tpCameraStates.Find(state => state.Name == _lastBlendedState));
        }

        _interactionSystem.Dispose();
    }



    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            _vThirdPersonController = GetComponent<vThirdPersonController>();
            _vThirdPersonInput = GetComponent<vThirdPersonInput>();
            _vShooterManager = GetComponent<vShooterManager>();
            _vFootStep = GetComponent<vFootStep>();
            _vGenericAnimation = GetComponent<vGenericAnimation>();
            _vShooterMeleeInput = GetComponent<vShooterMeleeInput>();
            _altInput = GetComponent<AltInput>();


            LoadPersistentData();


            _interactionSystem = new InteractionSystem(_interactionRange, _detectionRate, transform, _interactionLayerMask, _defaultLayerMask);

            _postProcessManager = new(GetComponentInChildren<Volume>());
        }

        else
        {
            Destroy(gameObject);
        }

    }

    void Start()
    {
        _vThirdPersonCamera = FindFirstObjectByType<vThirdPersonCamera>();
        // _vThirdPersonInput.onUpdate += CheckForTriggerAction;
        // _isSubscribedToThirdPersonInputs = true;

        // Debug.Log($" Special Equipments : {_playerSaveData._specialEquipments.Count}");
        if (_useLastSavedPosition)
        {

            transform.SetPositionAndRotation(_playerSaveData._position.position, Quaternion.Euler(_playerSaveData._position.rotation));
        }

        SpecialEquipmentManager.Instance.SetSpecialEquipments();

    }

    private void CheckForTriggerAction()
    {
        if (_showPhoneInput.GetButtonDown())
        {
            TogglePhone();
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

        if (_acceptInput.GetButtonDown())
        {
            // if (Phone.Instance._canQuickAcceptReject)
            // {
            //     Phone.Instance.QuickInspectDelivery();
            // }
            _activeQuickAction?.DoQuickAction();
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

    private void OnToggleEquipmentWheel()
    {
        if (_vThirdPersonCamera.LockCamera == false)
        {
            LockAllInput(true);
            _vThirdPersonCamera.LockCamera = true;
            _vThirdPersonInput.ShowCursor(true);
            _vThirdPersonInput.LockCursor(true);
        }

        else
        {
            LockAllInput(false);
            _vThirdPersonCamera.LockCamera = false;
            _vThirdPersonInput.ShowCursor(false);
            _vThirdPersonInput.LockCursor(false);
        }
    }
    #region SAVE/LOAD
    public void AutoSave()
    {
        AutoSaveManager.Autosave(SaveAble.PLAYER, new PlayerSaveData(this));
    }

    public void LoadPersistentData()
    {
        if (!ES3.KeyExists(SAVE_ID.ToString())) return;
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
        // Debug.Log("Interact Pressed");
        if (_lastInteractable != null)
        {
            // Debug.Log("Interacting with " + _lastInteractable.GameObject.name);
            _lastInteractable.Interact();
        }
    }
    public void TogglePhone(bool useBlur = true)
    {
        if (!_phoneCamera.gameObject.activeSelf)
        {
            _phoneCamera.gameObject.SetActive(true);
            _phoneModel.SetActive(true);
            UseAltControls(true, Phone.Instance);
            _vThirdPersonCamera.LockCamera = true;
            _vThirdPersonInput.ShowCursor(true);
            _vThirdPersonInput.LockCursor(true);
        }

        else
        {
            _phoneCamera.gameObject.SetActive(false);
            _phoneModel.SetActive(false);
            _vThirdPersonInput.SetLockBasicInput(false);
            UseAltControls(false);
            _vThirdPersonCamera.LockCamera = false;
            _vThirdPersonInput.ShowCursor(false);
            _vThirdPersonInput.LockCursor(false);
        }

        if (useBlur)
        {
            _postProcessManager.ToggleBlur();
        }
    }

    public void UseAltControls(bool state, IBrowsable browsable = null)
    {
        _isUsingAltInput = state;
        _altInput.ToggleUseInput(state);
        LockAllInput(state);
        _vThirdPersonCamera.LockCamera = state;
        _vThirdPersonInput.ShowCursor(state);
        _vThirdPersonInput.LockCursor(state);
        if (browsable != null)
        {
            _altInput._activeBrowsable = browsable;
        }
        else
        {
            _altInput._activeBrowsable = null;
        }
    }
    public void TogglePlayerInput(bool state)
    {
        _isUsingAltInput = state;
        _altInput.ToggleUseInput(state);
        LockAllInput(state);
        _vThirdPersonCamera.LockCamera = state;
        _vThirdPersonInput.ShowCursor(state);
        _vThirdPersonInput.LockCursor(state);
    }
    private void OnCutSceneStart(TimelinePlayer player)
    {

        _vThirdPersonInput.lockMoveInput = true;
        _vThirdPersonController.StopCharacter();
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


    [Button, TabGroup("Debug")]
    public void LockPlayerControls()
    {
        // _vThirdPersonInput.lockInput = true;
        _vThirdPersonInput.SetLockAllInput(true);
        _vThirdPersonInput.ShowCursor(true);
        _vThirdPersonInput.LockCursor(true);
        _vThirdPersonCamera.LockCamera = true;
    }
    [Button, TabGroup("Debug")]
    public void UnlockPlayerControls()
    {
        // _vThirdPersonInput.lockInput = true;
        _vThirdPersonInput.SetLockAllInput(false);
        _vThirdPersonInput.ShowCursor(false);
        _vThirdPersonInput.LockCursor(false);
        _vThirdPersonCamera.LockCamera = false;
    }



    public void LockAllInput(bool value)
    {
        _vThirdPersonInput.SetLockAllInput(value);
        InventoryManager.Instance._inventory.lockInventoryInput = value;
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



    public void SetInteractableObject(IInteractable interactable)
    {
        _lastInteractable = interactable;
    }

    public void UpdatePhone(Phone phone)
    {
        _phoneCamera = phone._phoneCamera;
        _phoneModel = phone._phoneModel;
        _defaultCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        UniversalAdditionalCameraData universalAdditionalCameraData = _defaultCamera.GetComponent<UniversalAdditionalCameraData>();
        universalAdditionalCameraData.cameraStack.Add(_phoneCamera);
    }



    public async Task PrepareForCutScene(TimelinePlayer player, float delay = 1f)
    {
        _vThirdPersonInput.lockMoveInput = true;
        _vThirdPersonController.StopCharacter();


        await Task.Delay((int)(delay * 1000));
        _vThirdPersonCamera.gameObject.SetActive(false);
    }

    async Task WatchForDestinationReached(Vector3 targetPosition)
    {

        gameObject.GetComponent<IInteractable>();
        while (_isRunning && (targetPosition - transform.position).magnitude > 0.5f)
        {
            // if (cts.IsCancellationRequested)
            // {
            //     break;
            // }
            Debug.Log("Distance: " + (targetPosition - transform.position).magnitude);
            await Task.Yield();
        }
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



    // void ToggleIsRunning()
    // {
    //     _isRunning = !_isRunning;
    // }
    // public void ToggleInputLock()
    // {
    //     _vThirdPersonInput.lockMoveInput = !_vThirdPersonInput.lockMoveInput;
    // }

    // public void StopAutoMove()
    // {
    //     _isRunning = false;
    //     Invoke(nameof(ToggleIsRunning), 0.1f);
    // }

    void OnDrawGizmosSelected()
    {
        Gizmos.DrawSphere(transform.position + Vector3.up, _interactionRange);
    }

    [Button]
    public void PlayAnimation(string animationName, float duration = 1f, Action OnComplete = null)
    {

        _vGenericAnimation.animationClip = animationName;
        _vGenericAnimation.animationEnd = duration;
        _vGenericAnimation.PlayAnimation();
        ABUtils.DelayedInvoke(duration, () => OnComplete?.Invoke());

    }

    [Button]
    public void SetDelvEbuddy(vShooterWeapon shooterWeapon)
    {
        // _vShooterManager.SetLeftWeapon(shooterWeapon.gameObject);
        _vShooterMeleeInput.SwitchCameraSide();
    }
}


[Serializable]
public class PostProcessManager
{
    readonly Volume _volume;
    DepthOfField _blur;
    public PostProcessManager(Volume volume)
    {
        _volume = volume;
        SetupEffects();
    }

    void SetupEffects()
    {
        _volume.profile.TryGet(out _blur);
    }

    [Button]
    public void ToggleBlur()
    {

        _blur.active = !_blur.active;
    }
}