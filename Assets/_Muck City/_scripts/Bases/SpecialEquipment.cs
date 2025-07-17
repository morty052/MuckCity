using UnityEngine;

public enum SpecialEquipmentID
{
    PHONE = 0,
    ARMOUR = 1,
    ROVER = 2,
    GAS_MASK = 3
}

public class SpecialEquipment : MonoBehaviour
{
    public SpecialEquipmentID _id;

    [SerializeField] protected bool _debugEquipment;
    public virtual void Init()
    {

    }

}
