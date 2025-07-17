using UnityEngine;

public class GasMask : SpecialEquipment
{
    public float resistance = 0.5f;
    public float _integrity = 50f;

    void Awake()
    {
        _id = SpecialEquipmentID.GAS_MASK;
    }

    public override void Init()
    {
        transform.SetParent(Player.Instance._headHolder);
        transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
    }
}
