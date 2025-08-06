using System;
using System.Collections;
using System.Collections.Generic;
using Invector.vShooter;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.VFX;


public class DelveBuddy : SpecialEquipment, IOnClickSlotReceiver
{
    DelveBuddyFunction _selectedFunction;

    [HideInInspector] public InputAction _fireInput;
    [HideInInspector] public InputAction _delveBuddySecondaryFire;

    [TabGroup("Components"), HideInInspector]
    public RadialMenu _specialEquipmentWheel;

    [TabGroup("Installs")]
    [SerializeReference] public List<DelveBuddyFunction> _installedFunctions;

    [TabGroup("Effects")]
    public Transform _effectsParent;

    [TabGroup("Effects")]
    public VisualEffect _harvestEffect;

    [TabGroup("Debug")]
    [SerializeField] private bool _isAiming;

    [TabGroup("Components"), HideInInspector]
    public vShooterWeapon _vShooterWeapon;



    public Action<vProjectileControl> OnInstantiateProjectileEvent;
    public Action<bool> OnToggleAim;

    void Awake()
    {
        _vShooterWeapon = GetComponent<vShooterWeapon>();
        _vShooterWeapon._ignoreIdleAnim = true;
        _vShooterWeapon.onEnableAim.AddListener(ToggleAiming);
        _vShooterWeapon.onDisableAim.AddListener(ToggleAiming);
        _effectsParent.SetParent(null);
    }

    void OnEnable()
    {
        _fireInput = InputSystem.actions.FindAction("DelveBuddyFire");
        _delveBuddySecondaryFire = InputSystem.actions.FindAction("DelveBuddySecondaryFire");
        AltInput.OnEquipDelveBuddy += EquipDelveBuddy;
        AltInput.OnToggleEquipmentWheel += OnToggleEquipmentWheel;

    }

    void OnDisable()
    {
        AltInput.OnEquipDelveBuddy -= EquipDelveBuddy;
        AltInput.OnToggleEquipmentWheel -= OnToggleEquipmentWheel;
    }

    public void OnToggleEquipmentWheel()
    {
        _specialEquipmentWheel.gameObject.SetActive(!_specialEquipmentWheel.gameObject.activeSelf);
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
        _specialEquipmentWheel = HudManager.Instance._delveBuddyEquipmentWheel;
        List<RectTransform> equipmentSlots = _specialEquipmentWheel.GetSlots();
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
        if (_selectedFunction == null) return;
        if (_selectedFunction.updateAlways)
        {
            _selectedFunction.Update();
        }
        if (_isAiming && !_selectedFunction.updateAlways && _selectedFunction._updateOnAim)
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

