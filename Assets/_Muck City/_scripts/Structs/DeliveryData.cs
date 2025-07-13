using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct DeliveryData
{
    public string _deliveryId;
    public int _deliveryFee;

    public Locations _deliveryArea;
    public Vector3 _deliveryLocation;

    public Dictionary<Substance, int> _deliverables;

    public Chat _chat;

    public DeliveryData(string deliveryId, int deliveryFee, Locations deliveryArea, Vector3 deliveryLocation, Dictionary<Substance, int> deliverables, Chat chat)
    {
        _deliveryId = deliveryId;
        _deliveryFee = deliveryFee;
        _deliveryArea = deliveryArea;
        _deliveryLocation = deliveryLocation;
        _deliverables = deliverables;
        _chat = chat;
    }
}
