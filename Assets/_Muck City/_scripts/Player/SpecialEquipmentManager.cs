using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Unity.VisualScripting;
using UnityEngine;

[Serializable]
public class SpecialEquipmentManager : MonoBehaviour
{
    public static SpecialEquipmentManager Instance { get; private set; }
    public List<SpecialEquipmentID> specialEquipments = new();
    public List<SpecialEquipmentSO> _data = new();
    public List<SpecialEquipment> _equipments = new();
    private bool _debug;

    public SpecialEquipmentManager(bool debug = false)
    {
        _debug = debug;
    }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }

        else
        {
            Destroy(gameObject);
        }
    }

    // void Start()
    // {
    //     SetSpecialEquipments();
    // }

    public void AddSpecialEquipment(SpecialEquipmentID specialEquipment)
    {
        Add(specialEquipment);
    }
    public void AddSpecialEquipment(SpecialEquipmentSO specialEquipment)
    {
        InitSpecialEquipment(specialEquipment);
    }

    public void Add(SpecialEquipmentID specialEquipment)
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

    void LoadEquipment(SpecialEquipmentID id)
    {

    }

    [Button]
    void SaveEquipments()
    {
        ES3.Save("SPECIAL_EQUIPMENTS", _data);
    }
    [Button]
    void LoadEquipmentEquipments()
    {
        List<SpecialEquipmentSO> so = (List<SpecialEquipmentSO>)ES3.Load("SPECIAL_EQUIPMENTS");
        Debug.Log(so.Count);
    }

    public void SetSpecialEquipments()
    {
        // foreach (SpecialEquipmentID specialEquipment in specialEquipments)
        // {
        //     AddSpecialEquipment(specialEquipment);
        // }

        foreach (SpecialEquipmentSO item in _data)
        {
            InitSpecialEquipment(item);
        }
    }

    void InitSpecialEquipment(SpecialEquipmentSO item)
    {
        // Debug.Log("bought special equipment" + item._id);
        SpecialEquipmentID id = item._id;
        if (specialEquipments.Contains(id)) return;
        if (_debug)
        {
            Debug.Log($"Adding {id} ");
        }
        specialEquipments.Add(id);
        Debug.Log(item.name);
        SpecialEquipment special = Instantiate(item._equipmentPrefab).GetComponent<SpecialEquipment>();
        _equipments.Add(special);
        special.Init();
    }

    public bool HasEquipment(SpecialEquipmentID equipment)
    {
        return specialEquipments.Contains(equipment);
    }
    public SpecialEquipment GetEquipment(SpecialEquipmentID id)
    {
        return _equipments.Find(x => x._id == id);
    }
}
