using System;
using UnityEngine;

public class DeliveryPoint : MonoBehaviour
{
    [SerializeField] DeliveryData _deliveryData;
    public Action<DeliveryData> OnDeliveryPointReached;

    bool _hasBeenReached = false;


    public void InitDeliveryPoint(DeliveryData deliveryData)
    {
        _deliveryData = deliveryData;
        Waypoint.Instance.Init(transform.position);
    }

    // Update is called once per frame
    void OnTriggerEnter()
    {
        if (_hasBeenReached) return;
        GameEventsManager.Instance.OnDeliveryPointReached(_deliveryData);
        OnDeliveryPointReached?.Invoke(_deliveryData);
        _hasBeenReached = true;
    }
}
