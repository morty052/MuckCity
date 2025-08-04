using System;
using System.Collections;
using System.Collections.Generic;
using Invector.vShooter;
using UnityEngine;
using UnityEngine.InputSystem;


public class DelveBuddy : SpecialEquipment, IOnClickSlotReceiver
{
    DelveBuddyFunction _selectedFunction;

    [HideInInspector] public InputAction _fireInput;

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
        _fireInput = InputSystem.actions.FindAction("Select");
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