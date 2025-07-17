using System.Collections.Generic;
using Invector.vItemManager;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "Special Equipment", menuName = "ScriptableObjects/Special Equipments", order = 1)]
public class SpecialEquipmentSO : ShopItemSO
{
    public GameObject _equipmentPrefab;

    public SpecialEquipmentID _id;

}
