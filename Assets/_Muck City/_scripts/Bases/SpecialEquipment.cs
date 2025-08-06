using Sirenix.OdinInspector;
using UnityEngine;

public enum SpecialEquipmentID
{
    PHONE = 0,
    ARMOUR = 1,
    ROVER = 2,
    GAS_MASK = 3,
    DELVE_BUDDY = 4
}

public class SpecialEquipment : Tradeable
{
    public SpecialEquipmentID _id;

    [TabGroup("Debug")]
    [SerializeField] protected bool _debugEquipment;
    public virtual void Init()
    {

    }

    public virtual void Equip()
    {

    }
    public virtual void Use()
    {

    }

}
