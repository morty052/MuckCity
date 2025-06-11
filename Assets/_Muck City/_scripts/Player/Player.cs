using System;
using System.Collections;
using System.Threading;
using System.Threading.Tasks;
using DialogueEditor;
using Invector;
using Invector.vCamera;
using Invector.vCharacterController;
using Invector.vItemManager;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityUtils;


public class Player : MonoBehaviour
{
    public static Player Instance { get; private set; }
    public GenericInput _interactionInput = new("E", "Y", "Y");
    public GenericInput _showPhoneInput = new("C", "Y", "Y");
    public GenericInput _endConvoInput = new("C", "Y", "Y");
    public GenericInput _exitInput = new("C", "Y", "Y");
    public GenericInput _dialogueOneInput = new("C", "Y", "Y");
    public GenericInput _dialogueTwoInput = new("C", "Y", "Y");


    public bool _shouldAutoSave = false;
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

    [SerializeField] BackPack _hotStorage;
    public Storage _activeStorage;

    public bool IsInVehicle => _currentVehicle != null;


    CancellationTokenSource cts = new();



    void OnEnable()
    {
        GameEventsManager.OnConversationStartedEvent += OnEnterConversation;
        GameEventsManager.OnConversationEndEvent += OnExitConversation;
        GameEventsManager.OnCutSceneStartEvent += OnCutSceneStart;
        GameEventsManager.OnCutSceneEndEvent += OnCutSceneEnd;
        GameEventsManager.OnCraftItemEvent += AddItemToInventory;


    }

    void OnDisable()
    {
        GameEventsManager.OnConversationStartedEvent -= OnEnterConversation;
        GameEventsManager.OnConversationEndEvent -= OnExitConversation;
        GameEventsManager.OnCutSceneStartEvent -= OnCutSceneStart;
        GameEventsManager.OnCutSceneEndEvent += OnCutSceneEnd;
        GameEventsManager.OnCraftItemEvent -= AddItemToInventory;
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
                Debug.Log("One");
                ConversationManager.Instance.m_currentSelectedIndex = 0;
                ConversationManager.Instance.PressSelectedOption();

            }
            else if (_dialogueTwoInput.GetButtonDown())
            {
                Debug.Log("Two");
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
        }

        else
        {
            _phoneCamera.gameObject.SetActive(false);
            _phoneModel.SetActive(false);
        }
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
        OnScreenDebugger.Instance.Log("Conversation started with " + conversation);
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

    // private void ShowPhone()
    // {
    //     GameEventsManager.Instance.DisplayPhoneEvent();
    //     _playerInput.SwitchCurrentActionMap("Phone");
    // }




    // private void SelectOptionOne()
    // {
    //     // Debug.Log("se one");
    //     GameEventsManager.Instance.OnBranchDecisionSelected(BranchDecisionManager.BranchDecisionType.OPTION_ONE);
    // }
    // private void SelectOptionTwo()
    // {
    //     // Debug.Log("se two");
    //     GameEventsManager.Instance.OnBranchDecisionSelected(BranchDecisionManager.BranchDecisionType.OPTION_TWO);
    // }


    // private void OnPlayerOverride(Vector3 vector)
    // {
    //     transform.position = vector;
    //     StartCoroutine(WatchForDestinationReached());
    // }

    // IEnumerator WatchForDestinationReached()
    // {
    //     yield return new WaitForSeconds(1f);
    //     GameEventsManager.Instance.OnPlayerOverrideComplete(transform.position);
    // }




    // public void HandleNpcInteraction()
    // {
    //     float interactRange = 2f;

    //     Collider[] hitColliders = Physics.OverlapSphere(transform.position, interactRange, _npcLayerMask);

    //     foreach (Collider hitCollider in hitColliders)
    //     {
    //         if (hitCollider.gameObject.GetComponent<Interactable>() != null)
    //         {
    //             Debug.Log(hitCollider.name + " is interactable");
    //         }

    //         else
    //         {
    //             Debug.Log(hitCollider.name + " is a citizen");
    //         }

    //     }
    // }

    public void EnvironmentInteraction()
    {

        Collider closestCollider = null;
        float closestDistance = _interactionRange + 10f;

        Collider[] hitColliders = Physics.OverlapSphere(transform.position + Vector3.up, _interactionRange, _interactionLayerMask);

        foreach (Collider hitCollider in hitColliders)
        {
            float distance = Vector3.Distance(transform.position, hitCollider.transform.position);
            if (distance < closestDistance)
            {
                // Debug.Log(hitCollider.name);
                closestDistance = distance;
                closestCollider = hitCollider;
            }
        }

        if (closestCollider != null)
        {
            if (closestCollider.gameObject.GetComponent<IInteractable>() != null)
            {
                // Debug.Log(closestCollider.name + " is interactable");
                _lastInteractable = closestCollider.gameObject.GetComponent<IInteractable>();
                if (_lastInteractable != null && _lastInteractable.CanInteract)
                {
                    _lastInteractable.PrepareInteraction();
                }
            }
        }

        if (hitColliders.Length == 0)
        {
            // HudManager.Instance.HideInteractPrompt();
            _lastInteractable = null;
        }

    }

    public void CheckIfItemInInventory()
    {
        // _inventory.ContainItem();

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
    public void UseItem(vItem item)
    {

    }
    public void EquipItem(vEquipArea equipArea, vItem item)
    {
        switch (item.type)
        {
            case vItemType.Consumable:
                Instantiate(item.dropObject, transform.position, Quaternion.identity);
                break;
            default:
                Debug.LogWarning("Item type not handled: " + item.type);
                break;
        }
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
}
