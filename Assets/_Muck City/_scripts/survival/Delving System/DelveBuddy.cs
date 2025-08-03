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

    protected Camera cam;
    [HideInInspector] public DelveBuddy _delveBuddy;
    public abstract void Use(DelveBuddy delveBuddy);
    public abstract void Init(DelveBuddy delveBuddy);
    public abstract void Equip();
    public abstract void UnEquip();

    public abstract void Update();
}

public class SpawnReturnBeacon : DelveBuddyFunction
{
    [SerializeField] ReturnBeacon _returnBeaconPrefab;

    private ReturnBeacon _returnBeaconInstance;

    public LayerMask _groundLayer = new();

    private bool _canPlace;

    public float _raycastDistance = 100f;

    private bool _lockedInPlace = false;


    public string _summonPortalAnimationName;

    public float _AnimationExitTime;


    public override void Init(DelveBuddy delveBuddy)
    {
        if (_returnBeaconInstance == null)
        {
            _returnBeaconInstance = GameObject.Instantiate(_returnBeaconPrefab, GameManager.Instance.SpawnPoint.transform);
            _returnBeaconInstance.gameObject.SetActive(false);
        }
        _delveBuddy = delveBuddy;

        cam = Camera.main;
    }

    public override void Equip()
    {
        SpecialEquipmentManager.Instance._activeEquipment = _delveBuddy;
        _delveBuddy._vShooterWeapon.onShot.AddListener(HandlePlaceMent);
        _delveBuddy._vShooterWeapon._isMuted = true;
        _delveBuddy.OnToggleAim += OnToggleAim;
    }


    public override void UnEquip()
    {
        _delveBuddy._vShooterWeapon.onShot.RemoveListener(HandlePlaceMent);
        _delveBuddy.OnToggleAim -= OnToggleAim;
        _delveBuddy._vShooterWeapon._isMuted = false;
    }
    public override void Update()
    {
        if (_lockedInPlace) return;
        RayCastForDrop();
    }

    public override void Use(DelveBuddy delveBuddy)
    {
        Debug.Log("Using Delve Buddy Function" + _id);
        Vector3 position = new(Player.Instance.transform.position.x, Player.Instance.transform.position.y, Player.Instance.transform.position.z - 2);
        _returnBeaconInstance.transform.position = position;
        _returnBeaconInstance.gameObject.SetActive(true);
        // _returnBeaconInstance.transform.SetParent(SpecialEquipmentManager.Instance.transform);
    }

    private void OnToggleAim(bool isAiming)
    {
        if (isAiming)
        {
            _lockedInPlace = false;
            _returnBeaconInstance.gameObject.SetActive(true);
        }
        else
        {
            if (!_lockedInPlace)
            {
                _returnBeaconInstance.gameObject.SetActive(false);
            }
        }
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


    private void HandlePlaceMent()
    {
        if (_canPlace)
        {
            _lockedInPlace = true;
            _canPlace = false;
            Player.Instance.PlayAnimation(_summonPortalAnimationName, _AnimationExitTime);
            Debug.Log("Used");
            // ToggleEquipBuddy();
            // ABUtils.DelayedInvoke(0.2f, () => ToggleEquipBuddy(true));
        }
    }
}
public class ScanEntity : DelveBuddyFunction
{
    public float _raycastDistance = 50;



    public float _timeToScan = 3.5f; // Total time in seconds
    public float _scanRate = 1f;

    public float _timeOnScannable = 0;
    public LayerMask _scannableLayer = new();

    public override void Init(DelveBuddy delveBuddy)
    {
        _delveBuddy = delveBuddy;
        cam = Camera.main;

        //* ACTIVATE UPDATE FUNCTION
        _updateOnAim = true;
    }


    public override void Equip()
    {
        _delveBuddy.OnInstantiateProjectileEvent += OnInstantiateProjectile;
    }

    public override void UnEquip()
    {
        _delveBuddy.OnInstantiateProjectileEvent -= OnInstantiateProjectile;
    }

    public override void Update()
    {
        RayCastForScannable();
    }

    public override void Use(DelveBuddy delveBuddy)
    {
        Debug.Log("Using Delve Buddy Function" + _id);

        // _returnBeaconInstance.transform.SetParent(SpecialEquipmentManager.Instance.transform);
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
            if (entity.CanScan)
            {
                _timeOnScannable += _scanRate * Time.deltaTime;
                Debug.Log("Time Spent Scanning " + _timeOnScannable);
                if (_timeOnScannable >= _timeToScan)
                {
                    entity.OnScan();
                }

            }
        }
    }

    void RayCastForScannable()
    {
        Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f)); // Center of screen
        if (Physics.Raycast(ray, out RaycastHit hit, _raycastDistance, _scannableLayer))
        {
            OnScanObject(hit);
        }
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
        _selectedFunction.Equip();
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

        //* UNEQUIP CURRENT SELECTED FUNTION IF ANY
        if (_selectedFunction != null)
        {
            _selectedFunction.UnEquip();
        }

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


public class TimedCounter
{
    private float duration = 20f; // Total time in seconds
    private float elapsedTime = 0f;
    private float currentValue = 0f;

    void Update()
    {
        if (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            currentValue = Mathf.Lerp(0f, 20f, elapsedTime / duration);
            Debug.Log("Counter: " + currentValue.ToString("F2"));
        }
        else
        {
            currentValue = 20f;
            Debug.Log("Finished! Final Value: " + currentValue);
        }
    }
}