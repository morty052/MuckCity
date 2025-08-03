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
    [HideInInspector] public DelveBuddy _delveBuddy;
    public abstract void Use(DelveBuddy delveBuddy);
    public abstract void Init(DelveBuddy delveBuddy);
    public abstract void Equip();
}

public class SpawnReturnBeacon : DelveBuddyFunction
{
    [SerializeField] ReturnBeacon _returnBeaconPrefab;

    private ReturnBeacon _returnBeaconInstance;
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
        if (_returnBeaconInstance == null)
        {
            _returnBeaconInstance = GameObject.Instantiate(_returnBeaconPrefab, GameManager.Instance.SpawnPoint.transform);
            _returnBeaconInstance.gameObject.SetActive(false);
        }
        SpecialEquipmentManager.Instance._activeEquipment = _delveBuddy;
    }

    public override void Init(DelveBuddy delveBuddy)
    {
        _delveBuddy = delveBuddy;
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
}
public class DelveBuddy : SpecialEquipment, IOnClickSlotReceiver
{
    DelveBuddyFunction _selectedFunction;

    [SerializeReference] public List<DelveBuddyFunction> _installedFunctions;


    [SerializeField] private bool _isAiming;
    public vShooterWeapon _vShooterWeapon;

    public Action<vProjectileControl> OnInstantiateProjectileEvent;

    Animator _animator;

    void Awake()
    {
        _vShooterWeapon = GetComponent<vShooterWeapon>();
    }

    void OnEnable()
    {
        AltInput.OnEnterAimDelveBuddy += HandleEnterAimDelveBuddy;
    }

    void OnDisable()
    {
        AltInput.OnEnterAimDelveBuddy -= HandleEnterAimDelveBuddy;
    }

    public void OnInstantiateProjectile(vProjectileControl projectile)
    {
        OnInstantiateProjectileEvent?.Invoke(projectile);
    }

    void Update()
    {
        if (!_isAiming) return;

    }

    private void HandleEnterAimDelveBuddy(bool isAiming)
    {
        if (isAiming)
        {
            _isAiming = true;
            Player.Instance._vShooterManager.SetLeftWeapon(gameObject);

        }
        else
        {
            _isAiming = false;
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
    }
    public override void Use()
    {
        _selectedFunction.Use(this);
    }

    public override void Init()
    {
        transform.SetParent(Player.Instance._delveBuddySlot);
        transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
        _animator = Player.Instance.transform.GetComponent<Animator>();
        GetComponent<vShooterWeapon>()._ignoreIdleAnim = true;
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
