using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DeliveryManager : MonoBehaviour
{

    [SerializeField] int _pricePerMeter = 1;
    [SerializeField] int _maxIdleDeliveries = 4;
    [SerializeField] List<DruggieNpcSO> _clients = new();
    List<DruggieNpcSO> _druggiesWithOrders = new();
    [SerializeField] List<DeliveryData> _proceduralDeliveries = new();

    void OnEnable()
    {
        GameEventsManager.OnAllNpcLoadedEvent += PopulateDruggiePool;
        GameEventsManager.OnDeliveryMarkerPlacedEvent += UpdateDruggiePool;
        GameEventsManager.OnInGameHoursPassedEvent += HandleProceduralDeliveries;
        GameEventsManager.OnDeliveryAcceptedEvent += HandleDeliveryAccepted;
        GameEventsManager.OnDeliveryPointReachedEvent += HandleDeliveryPointReached;
    }

    void OnDisable()
    {
        GameEventsManager.OnAllNpcLoadedEvent -= PopulateDruggiePool;
        GameEventsManager.OnDeliveryMarkerPlacedEvent -= UpdateDruggiePool;
        GameEventsManager.OnInGameHoursPassedEvent -= HandleProceduralDeliveries;
        GameEventsManager.OnDeliveryAcceptedEvent -= HandleDeliveryAccepted;
        GameEventsManager.OnDeliveryPointReachedEvent -= HandleDeliveryPointReached;
    }

    private void HandleDeliveryPointReached(DeliveryData data)
    {
        Debug.Log("Delivery Point Reached for " + data._deliveryId);
        DruggieNpcSO druggieNpcSO = _druggiesWithOrders.Find(x => x._name == data._deliveryId);
        _druggiesWithOrders.Remove(druggieNpcSO);
    }

    private void HandleDeliveryAccepted(DeliveryData data)
    {
        DruggieNPC druggieNPC = NpcManager.Instance.GetNPCByName(data._deliveryId).GetComponent<DruggieNPC>();
        druggieNPC.StartPickup(data);
    }

    private void HandleProceduralDeliveries()
    {
        if (_clients.Count == 0) return;

        DruggieNpcSO druggieSO = GetRandomDruggie();

        if (druggieSO == null)
        {
            return;
        }


        DeliveryData delivery = CreateOrder(druggieSO);

        // Debug.Log("created delivery for " + delivery._deliveryId + " total items " + delivery._deliverables.Count);


        if (_proceduralDeliveries.Count < _maxIdleDeliveries)
        {
            _proceduralDeliveries.Add(delivery);
            GameEventsManager.Instance.OnDeliveryAdded(delivery);
        }

        else
        {
            _proceduralDeliveries.RemoveAt(0);
            _proceduralDeliveries.Add(delivery);
        }
    }

    DeliveryData CreateOrder(DruggieNpcSO druggieSO)
    {


        DruggieNPC druggieNPC = NpcManager.Instance.GetNPCByName(druggieSO._name).GetComponent<DruggieNPC>(); // Get the druggie component
        Vector3 position = druggieNPC.GetRandomPickupSpot(); // Get a random pickup spot


        List<SubstanceData> deliverable = CatalogueManager.Instance.GetSubstancesInStock(druggieSO._preferredSubstances);

        Dictionary<Substance, int> deliverables = new();
        foreach (SubstanceData item in deliverable)
        {
            deliverables.Add(item._id, GetQuantityFromPurchasingPower(druggieNPC._purchasingPower, item._count));
        }

        int deliveryFee = GetDeliveryFee(position, deliverables); // Get the delivery fee


        DeliveryData delivery = new(druggieSO._name, deliveryFee, druggieSO._primaryLocation, position, deliverables);

        _druggiesWithOrders.Add(druggieSO);
        return delivery;
    }

    //TODO: Make sure same druggie is not assigned twice
    DruggieNpcSO GetRandomDruggie()
    {
        List<DruggieNpcSO> druggieNPCList = _clients.FindAll(x => !_druggiesWithOrders.Contains(x));
        Debug.Log("total unused npc " + druggieNPCList.Count);
        if (druggieNPCList.Count > 0)
        {
            return druggieNPCList[Random.Range(0, druggieNPCList.Count)];
        }

        else return null;
    }

    int GetQuantityFromPurchasingPower(int purchasingPower, int substanceCount)
    {

        int quantity = Random.Range(1, purchasingPower);
        if (quantity > substanceCount) quantity = substanceCount;
        return quantity;
    }

    int GetDeliveryFee(Vector3 position, Dictionary<Substance, int> deliverables)
    {
        LocationData bunker = DomeManager.Instance.GetOthroBunker();
        float distance = Vector3.Distance(position, bunker._entrance);
        float costToDistance = distance * _pricePerMeter;

        int itemsCost = 0;
        foreach (KeyValuePair<Substance, int> item in deliverables)
        {
            // Debug.Log(item.Key + " " + item.Value);
            itemsCost += CatalogueManager.Instance.GetSubstancePrice(item.Key) * item.Value;
        }
        return (int)costToDistance + itemsCost;
    }

    private void PopulateDruggiePool()
    {
        var clients = NpcManager.Instance.GetNPCByRole(Role.DRUGGIE)//*Only druggies
        .FindAll(x => GameManager.Instance._marketingAreas.Contains(x.Data._primaryLocation)); //*Only druggies in marketing areas
        clients.ForEach(x =>
        {
            List<SubstanceType> substances = x.GetComponent<DruggieNPC>()._preferredSubstances;
            if (CatalogueManager.Instance.HasSubstance(substances)) //*Only druggies with preferred substances that player has
            {
                _clients.Add((DruggieNpcSO)x.Data);
            }
        });

    }
    void UpdateDruggiePool(Locations location)
    {
        List<NpcCharacter> npcList = NpcManager.Instance.GetNPCListByDistrict(location, Role.DRUGGIE);
        npcList.ForEach(x =>
        {
            List<SubstanceType> substances = x.GetComponent<DruggieNPC>()._preferredSubstances;
            if (CatalogueManager.Instance.HasSubstance(substances)) //*Only druggies with preferred substances that player has
            {
                _clients.Add((DruggieNpcSO)x.Data);
            }
        });
    }
}
