using System;
using System.Collections.Generic;
using Invector.vCharacterController;
using Invector.vShooter;
using UnityEditor.Animations;
using UnityEngine;

public enum DelveBuddyFunctions
{
    RETURN_BEACON,
    SCAN_ENTITY
}

[Serializable]
public abstract class DelveBuddyFunction
{
    public DelveBuddyFunctions _id;
    public Sprite _icon;

    public bool _updateOnAim;
    [HideInInspector] public DelveBuddy _delveBuddy;
    public abstract void Use(DelveBuddy delveBuddy);
    public abstract void Init(DelveBuddy delveBuddy);
    public abstract void Equip();

    public abstract void Update();
}

public class SpawnReturnBeacon : DelveBuddyFunction
{
    [SerializeField] ReturnBeacon _returnBeaconPrefab;

    private ReturnBeacon _returnBeaconInstance;

    private Transform _playerAimReference;

    public LayerMask _groundLayer = new();

    private bool _canPlace;

    private float _raycastDistance = 100f;

    private bool _lockedInPlace = false;
    public Camera cam;
    public override void Use(DelveBuddy delveBuddy)
    {
        Debug.Log("Using Delve Buddy Function" + _id);
        Vector3 position = new(Player.Instance.transform.position.x, Player.Instance.transform.position.y, Player.Instance.transform.position.z - 2);
        _returnBeaconInstance.transform.position = position;
        _returnBeaconInstance.gameObject.SetActive(true);
        // _returnBeaconInstance.transform.SetParent(SpecialEquipmentManager.Instance.transform);
    }
    public override void Equip()
    {
        SpecialEquipmentManager.Instance._activeEquipment = _delveBuddy;
    }

    public override void Init(DelveBuddy delveBuddy)
    {
        if (_returnBeaconInstance == null)
        {
            _returnBeaconInstance = GameObject.Instantiate(_returnBeaconPrefab, GameManager.Instance.SpawnPoint.transform);
            _returnBeaconInstance.gameObject.SetActive(false);
        }
        _delveBuddy = delveBuddy;
        _playerAimReference = delveBuddy._vShooterWeapon.aimReference;

        delveBuddy._vShooterWeapon.onShot.AddListener(HandlePlaceMent);

        delveBuddy.OnToggleAim += OnToggleAim;

        cam = Camera.main;
    }

    private void OnToggleAim(bool isAiming)
    {
        if (isAiming)
        {
            _lockedInPlace = false;
        }
    }

    private void HandlePlaceMent()
    {
        if (_canPlace)
        {
            _lockedInPlace = true;
            _canPlace = false;
            // ToggleEquipBuddy();
            // ABUtils.DelayedInvoke(0.2f, () => ToggleEquipBuddy(true));
        }


    }

    void ToggleEquipBuddy(bool state = false)
    {
        _delveBuddy.EquipDelveBuddy(state);
    }

    public override void Update()
    {
        if (_lockedInPlace) return;
        RayCastForDrop();
    }

    void RayCastForDrop()
    {
        Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f)); // Center of screen
        if (Physics.Raycast(ray, out RaycastHit hit, _raycastDistance, _groundLayer))
        {
            _canPlace = true;
            PreviewBeacon(hit.point, Quaternion.LookRotation(hit.normal));
        }

        else
        {
            _canPlace = false;
        }

    }

    void PreviewBeacon(Vector3 position, Quaternion rotation)
    {
        _returnBeaconInstance.gameObject.SetActive(true);
        _returnBeaconInstance.transform.SetPositionAndRotation(position, Quaternion.identity);
    }
}
public class ScanEntity : DelveBuddyFunction
{
    [SerializeField] vShooterWeapon _weapon;

    public override void Use(DelveBuddy delveBuddy)
    {
        Debug.Log("Using Delve Buddy Function" + _id);

        // _returnBeaconInstance.transform.SetParent(SpecialEquipmentManager.Instance.transform);
    }

    public override void Equip()
    {

    }

    public override void Init(DelveBuddy delveBuddy)
    {
        _delveBuddy = delveBuddy;
        _weapon = _delveBuddy._vShooterWeapon;
        _delveBuddy.OnInstantiateProjectileEvent += OnInstantiateProjectile;
        // Debug.Log($"<color=cyan> Init Scan Entity Function</color>");
    }

    private void OnInstantiateProjectile(vProjectileControl control)
    {
        // Debug.Log($"<color=cyan> Scanned Entity </color>");
        control.onCastCollider.AddListener(OnScanObject);
    }

    void OnScanObject(RaycastHit hit)
    {
        if (hit.transform.TryGetComponent(out IScannableObject entity))
        {
            entity.OnScan();
        }
    }

    public override void Update()
    {
        throw new NotImplementedException();
    }
}
public class DelveBuddy : SpecialEquipment, IOnClickSlotReceiver
{
    DelveBuddyFunction _selectedFunction;

    [SerializeReference] public List<DelveBuddyFunction> _installedFunctions;


    [SerializeField] private bool _isAiming;
    public vShooterWeapon _vShooterWeapon;

    public Action<vProjectileControl> OnInstantiateProjectileEvent;
    public Action<bool> OnToggleAim;
    public Action OnDisableAim;

    void Awake()
    {
        _vShooterWeapon = GetComponent<vShooterWeapon>();
        _vShooterWeapon._ignoreIdleAnim = true;
        _vShooterWeapon.onEnableAim.AddListener(ToggleAiming);
        _vShooterWeapon.onDisableAim.AddListener(ToggleAiming);


    }

    void OnEnable()
    {
        AltInput.OnEnterAimDelveBuddy += EquipDelveBuddy;
    }

    void OnDisable()
    {
        AltInput.OnEnterAimDelveBuddy -= EquipDelveBuddy;
    }

    public void OnInstantiateProjectile(vProjectileControl projectile)
    {
        OnInstantiateProjectileEvent?.Invoke(projectile);
    }


    public void EquipDelveBuddy(bool shouldEquip)
    {
        if (shouldEquip)
        {

            Player.Instance._vShooterManager.SetLeftWeapon(gameObject);
        }
        else
        {
            Player.Instance._vShooterManager.SetLeftWeapon(null);
        }
    }

    void Start()
    {
        //*GET ALL AVAILABLE SLOTS
        List<RectTransform> equipmentSlots = SpecialEquipmentManager.Instance._specialEquipmentWheel.GetComponent<RadialMenu>().GetSlots();
        for (int i = 0; i < _installedFunctions.Count; i++)
        {
            //*INITIALIZE FUNCTION
            _installedFunctions[i].Init(this);

            //*INITIALIZE SPECIAL EQUIPMENT SLOT
            SpecialEquipmentSlot specialEquipmentSlot = equipmentSlots[i].GetComponent<SpecialEquipmentSlot>();
            specialEquipmentSlot.Init(_installedFunctions[i]._id.ToString(), this, _installedFunctions[i]._icon);
        }

        _selectedFunction = _installedFunctions[0];
    }

    void Update()
    {
        if (_isAiming && _selectedFunction != null && _selectedFunction._updateOnAim)
        {
            _selectedFunction.Update();
        }
    }

    public override void Use()
    {
        _selectedFunction.Use(this);
    }

    public override void Init()
    {
        transform.SetParent(Player.Instance._delveBuddySlot);
        transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
    }


    public void ToggleAiming()
    {
        _isAiming = !_isAiming;
        if (_isAiming)
        {
            OnToggleAim?.Invoke(true);
        }
        else
        {
            OnToggleAim?.Invoke(false);
        }
        Player.Instance._vShooterMeleeInput.SwitchCameraSide();
    }


    public void OnSlotClicked(string slotId)
    {
        Debug.Log("Slot clicked: " + slotId);
        //* FIND INSTALLED FUNCTION BY ID
        _selectedFunction = _installedFunctions.Find(x => x._id.ToString() == slotId);
        if (_selectedFunction != null)
        {
            //* RUN INITIALIZATION OPERATION FOR SELECTED FUNCTION
            _selectedFunction.Equip();
        }

        //*HIDE ABILITY WHEEL
        AltInput.OnToggleEquipmentWheel?.Invoke();
    }
}



public class CenterRaycast
{
    public Camera cam; // Assign your camera in the Inspector
    public float rayDistance = 100f;

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // Trigger on left-click
        {
            Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f)); // Center of screen
            if (Physics.Raycast(ray, out RaycastHit hit, rayDistance))
            {
                Debug.Log("Hit: " + hit.collider.name);
                // You can add more logic here, like interacting with the object
            }
        }
    }
}