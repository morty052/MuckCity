using System.Collections;
using Unity.Cinemachine;
using ImprovedTimers;
using UnityEngine;
using UnityEngine.AI;

public class Rover : MonoBehaviour, IInteractable
{

    public CinemachineCamera _defaultCam;
    public CinemachineCamera _weaponsCam;

    WeaponsCache _weaponsCache;
    public RoverPowerCore _powerCore;
    public RoverInterface _activeInterface;
    [SerializeField] float _distance = 5f;

    NavMeshAgent _agent;

    CountdownTimer _timer;

    Animator _animator;

    public bool _hasAccessToPlayer = true;

    readonly int OPEN_WEAPONS_CACHE_ANIM = Animator.StringToHash("Open Weapons Cache");
    readonly int EXIT_WEAPONS_CACHE_ANIM = Animator.StringToHash("Close Weapons Cache");

    public enum RoverState { WEAPONS_CACHE, HAULING_CACHE, IDLE }

    public RoverState _state = RoverState.IDLE;

    public string InteractionPrompt => throw new System.NotImplementedException();

    bool _canInteract = true;

    public bool CanInteract { get => _canInteract; set => _canInteract = value; }

    [SerializeField] LayerMask _gunLayerMask = new();

    public static Rover Instance { get; private set; }

    public float InteractionDistance => 2f;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }

        else
        {
            Destroy(gameObject);
        }
        _animator = GetComponent<Animator>();
        _agent = GetComponent<NavMeshAgent>();
        _agent.stoppingDistance = _distance;
        _weaponsCache = GetComponentInChildren<WeaponsCache>();
        _powerCore = GetComponent<RoverPowerCore>();
    }

    void OnEnable()
    {
        GameEventsManager.OnExitBrowseMode += ExitWeaponsCam;
    }


    void OnDisable()
    {
        GameEventsManager.OnExitBrowseMode -= ExitWeaponsCam;
    }


    void Start()
    {
        _timer = new(5f);
        _timer.OnTimerStop += () =>
        {
            Follow();
            _timer.Start();
        };
        _timer.Start();
    }

    // void FixedUpdate()
    // {
    //     if (_stateToTransition == RoverState.WEAPONS_CACHE)
    //     {
    //         FireRay();
    //         return;
    //     }

    // }
    bool CanFollow()
    {
        if (!_hasAccessToPlayer)
        {
            Debug.Log("No access to player");
            return false;
        }
        bool canFollow = Vector3.Distance(transform.position, Player.Instance.transform.position) > _distance;
        return canFollow;
    }

    void Follow()
    {
        if (CanFollow())
        {
            // Debug.Log("Repositioning : " + CanFollow());
            _agent.SetDestination(Player.Instance.transform.position);
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, _distance);
    }

    public void PrepareInteraction()
    {
        HudManager.Instance.ShowInteractPrompt("Access Rover");
    }

    public void CutToWeaponsCam()
    {
        _weaponsCam.gameObject.SetActive(true);
    }

    public void ExitWeaponsCam()
    {
        _weaponsCam.gameObject.SetActive(false);
        _animator.SetTrigger(EXIT_WEAPONS_CACHE_ANIM);
        _state = RoverState.IDLE;
        _canInteract = true;
        _weaponsCache.Exit();
    }

    public void Interact()
    {
        // if (_stateToTransition == RoverState.WEAPONS_CACHE)
        // {
        //     _animator.SetTrigger(EXIT_WEAPONS_CACHE_ANIM);
        //     _stateToTransition = RoverState.IDLE;
        //     return;
        // }
        _animator.SetTrigger(OPEN_WEAPONS_CACHE_ANIM);
        _state = RoverState.WEAPONS_CACHE;
        HudManager.Instance.HideInteractPrompt();
        GameEventsManager.Instance.OnEnterBrowseModeEvent(_weaponsCache);
        Debug.Log("interact called");
    }

    public void ChargeObject(IUseEnergy receiver)
    {
        if (!_hasAccessToPlayer) return;
        StartCoroutine(nameof(ChargeObjectCoroutine), receiver);
    }

    IEnumerator ChargeObjectCoroutine(IUseEnergy receiver)
    {
        _hasAccessToPlayer = false;
        GameObject chargingPort = receiver.ChargingPort.gameObject;
        _agent.SetDestination(chargingPort.transform.position);
        yield return new WaitUntil(() => _agent.remainingDistance <= _distance);
        _powerCore.TransferEnergy(receiver, chargingPort, () =>
        {
            _powerCore._cable.SetEndPoint(null);
            _hasAccessToPlayer = true;
            Debug.Log("Charged " + chargingPort.name);
            receiver.OnChargeComplete();
        });
    }

    private void FireRay()
    {
        Debug.Log("Firing ray");
        Vector3 mouseWorldPosition = Vector3.zero;
        Vector2 screenCenter = new(Screen.width / 2f, Screen.height / 2f);
        Ray ray = Camera.main.ScreenPointToRay(screenCenter);
        if (Physics.Raycast(ray, out RaycastHit hit, 999f, _gunLayerMask))
        {
            if (hit.collider != null)
            {
                Debug.Log(hit.collider.name);
            }
        }
    }

    public void HideInteractionPrompt()
    {
        throw new System.NotImplementedException();
    }
}
