using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public enum Substance
{
    Kick = 34
}



public class CatalogueManager : MonoBehaviour
{
    public static CatalogueManager Instance { get; private set; }
    [SerializeField] List<SubstanceType> _unlockedSubstances = new();

    [SerializeField] List<SubstanceData> _inventory = new();

    void OnEnable()
    {
        GameEventsManager.OnDeliveryPointReachedEvent += DecreaseSubstanceQuantity;
    }

    void OnDisable()
    {
        GameEventsManager.OnDeliveryPointReachedEvent -= DecreaseSubstanceQuantity;
    }

    private void DecreaseSubstanceQuantity(DeliveryData data)
    {
        foreach (var item in data._deliverables)
        {
            DecreaseItemCount(item.Key, item.Value);
        }
    }

    void Start()
    {
        IncInventoryItemCount(Substance.Kick, 5);
    }

    public bool HasSubstance(List<SubstanceType> substances)
    {
        if (substances.Any(s => _unlockedSubstances.Contains(s)))
        {
            if (substances.Any(s => GetSubstanceByType(s)._count > 0))
            {
                return true;
            }
        }
        return false;
    }

    SubstanceData GetSubstanceByType(SubstanceType type) => _inventory.Find(x => x._type == type);

    public List<SubstanceData> GetSubstancesInStock(List<SubstanceType> substances)
    {
        List<SubstanceData> data = _inventory.FindAll(x => substances.Contains(x._type) && x._count > 0);
        return data;
    }
    public bool HasItems() => _unlockedSubstances.Count > 0;

    public void IncInventoryItemCount(Substance substance, int count = 1)
    {
        SubstanceData substanceData = _inventory.Find(x => x._id == substance);
        substanceData._count += count;
    }
    public void DecreaseItemCount(Substance substance, int count)
    {
        SubstanceData substanceData = _inventory.Find(x => x._id == substance);
        substanceData._count -= count;
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

    public int GetSubstancePrice(Substance key)
    {
        SubstanceData data = _inventory.Find(x => x._id == key);
        return data.Data._price;
    }
}
