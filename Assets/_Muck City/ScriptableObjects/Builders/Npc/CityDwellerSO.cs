using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Npc", menuName = "ScriptableObjects/NewNpc/DruggieNpc", order = 3)]
public class CityDwellerSO : NpcSO
{
    public List<SubstanceType> _preferredSubstances = new();
    public List<Vector3> _pickUpSpots = new();

    public int _defaultPurchasingPower = 1;
    public DeliveryPoint _deliveryPointPrefab;

    private void OnValidate()
    {
#if UNITY_EDITOR
        _name = this.name;
        if (_roles.Count == 0)
        {
            _roles.Add(Role.DRUGGIE);
            _roles.Add(Role.CITY_DWELLER);
        }
        UnityEditor.EditorUtility.SetDirty(this);
#endif
    }


}
