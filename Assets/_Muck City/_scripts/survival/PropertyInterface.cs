using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class PropertyInterface : Interactable
{
    public Residence property;
    public List<GameObject> _energyObjects = new();
    private List<IUseEnergy> _energyConsumers = new();

    public void TransferPropertyToPlayer()
    {
        property.TransferToPlayer();
    }

    void Awake()
    {
        for (int i = 0; i < _energyObjects.Count; i++)
        {
            _energyConsumers.Add(_energyObjects[i].GetComponent<IUseEnergy>());
        }
    }

    public void PowerDownProperty()
    {
        foreach (var energyConsumer in _energyConsumers)
        {
            energyConsumer.PowerDown();
        }
    }
    public void PowerUpProperty()
    {
        foreach (var energyConsumer in _energyConsumers)
        {
            energyConsumer.PowerUp(energyConsumer.EnergyNeededToFunction);
        }
    }
}
