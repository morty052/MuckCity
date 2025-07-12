using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SpecialEquipmentManager
{
    public List<SpecialEquipmentID> specialEquipments = new();
    private bool _debug;

    public SpecialEquipmentManager(bool debug = false)
    {
        _debug = debug;
    }

    public void AddSpecialEquipment(SpecialEquipmentID specialEquipment)
    {
        if (specialEquipments.Contains(specialEquipment)) return;
        if (_debug)
        {
            Debug.Log($"Adding {specialEquipment} from SpecialEquipment/{specialEquipment}/{specialEquipment}");
        }
        specialEquipments.Add(specialEquipment);
        SpecialEquipmentSO specialEquipmentSO = Resources.Load<SpecialEquipmentSO>($"SpecialEquipment/{specialEquipment}/{specialEquipment}");
        Debug.Log(specialEquipmentSO.name);
        SpecialEquipment special = GameObject.Instantiate(specialEquipmentSO._equipmentPrefab).GetComponent<SpecialEquipment>();
        special.Init();

        // SpecialEquipment special = GameObject.Instantiate(specialEquipment._equipmentPrefab);
        // special.Init();
    }

    public void SetSpecialEquipments(List<SpecialEquipmentID> specialEquipments)
    {
        foreach (SpecialEquipmentID specialEquipment in specialEquipments)
        {
            AddSpecialEquipment(specialEquipment);
        }
    }
}
