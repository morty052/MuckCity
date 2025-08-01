using System;
using System.Collections.Generic;
using UnityEngine;
using UnityUtils;

public enum DelveBuddyFunctions
{
    RETURN_BEACON
}

[Serializable]
public abstract class DelveBuddyFunction
{
    public DelveBuddyFunctions _id;
    [HideInInspector] public DelveBuddy _delveBuddy;
    public abstract void Use(DelveBuddy delveBuddy);
    public abstract void Init(DelveBuddy delveBuddy);
    public abstract void Equip();
}
[Serializable]
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
            _returnBeaconInstance = GameObject.Instantiate(_returnBeaconPrefab);
            _returnBeaconInstance.gameObject.SetActive(false);
        }
        SpecialEquipmentManager.Instance._activeEquipment = _delveBuddy;
    }

    public override void Init(DelveBuddy delveBuddy)
    {
        _delveBuddy = delveBuddy;
    }
}
public class DelveBuddy : SpecialEquipment, IOnClickSlotReceiver
{
    DelveBuddyFunction _selectedFunction;

    [SerializeReference] public List<DelveBuddyFunction> _installedFunctions;

    void Start()
    {
        List<RectTransform> equipmentSlots = SpecialEquipmentManager.Instance._specialEquipmentWheel.GetComponent<RadialMenu>().GetSlots();
        for (int i = 0; i < _installedFunctions.Count; i++)
        {
            _installedFunctions[i].Init(this);
            SpecialEquipmentSlot specialEquipmentSlot = equipmentSlots[i].GetComponent<SpecialEquipmentSlot>();
            specialEquipmentSlot._slotId = _installedFunctions[i]._id.ToString();
            specialEquipmentSlot._onClickSlotReceiver = this;
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

    public void OnSlotClicked(string slotId)
    {
        Debug.Log("Slot clicked: " + slotId);
        _selectedFunction = _installedFunctions.Find(x => x._id.ToString() == slotId);
        if (_selectedFunction != null)
        {
            _selectedFunction.Equip();
        }
        AltInput.OnToggleEquipmentWheel?.Invoke();
    }
}
