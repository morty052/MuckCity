using UnityEngine;

public enum SpecialEquipmentID
{
    PHONE = 0,
    ARMOUR = 1,
    ROVER = 2
}

public class SpecialEquipment : MonoBehaviour
{
    public SpecialEquipmentID _id;
    public virtual void Init()
    {

    }

}
