using System.Collections.Generic;
using UnityEngine;

public class DruggieNPC : NpcCharacter
{
    public List<SubstanceType> _preferredSubstances = new();
    public List<Vector3> _pickupSpots = new();

    [SerializeField] DeliveryPoint _deliveryPointPrefab;

    Vector3 _lastPickupSpot = Vector3.zero;
    DeliveryPoint _currentDeliveryPoint = null;
    public int _purchasingPower;


    //TODO ADD WAY TO INCREASE PURCHASING POWER
    protected override void SetupData()
    {
        base.SetupData();
        DruggieNpcSO npcSO = _npcSO as DruggieNpcSO;
        _preferredSubstances = npcSO._preferredSubstances;
        _pickupSpots = npcSO._pickUpSpots;
        _deliveryPointPrefab = npcSO._deliveryPointPrefab;
        _purchasingPower = npcSO._defaultPurchasingPower;
    }

    public Vector3 GetRandomPickupSpot()
    {
        Vector3 randomSpot = _pickupSpots[Random.Range(0, _pickupSpots.Count)];
        _lastPickupSpot = randomSpot;
        return randomSpot;
    }

    public void StartPickup(DeliveryData data)
    {
        Debug.Log("Delivery Accepted for " + data._deliveryId);
        DeliveryPoint deliveryPoint = Instantiate(_deliveryPointPrefab, _lastPickupSpot, Quaternion.identity);
        deliveryPoint.InitDeliveryPoint(data);
        _currentDeliveryPoint = deliveryPoint;
        deliveryPoint.OnDeliveryPointReached += CompletePickup;

        // GameObject marker = MiniMap.Instance.GetMarker(deliveryPoint.transform);
        // marker.transform.localPosition = Vector3.up * 10;
        transform.position = new Vector3(deliveryPoint.transform.position.x, transform.position.y, deliveryPoint.transform.position.z);

    }

    void CompletePickup(DeliveryData data)
    {
        Debug.Log("Delivery Completed for " + data._deliveryId);
        _currentDeliveryPoint.OnDeliveryPointReached -= CompletePickup;
        Destroy(_currentDeliveryPoint.gameObject);
        _currentDeliveryPoint = null;
    }
}
