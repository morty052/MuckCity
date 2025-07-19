using System;
using UnityEngine;

public class PropertyInterface : Interactable
{
    public Residence property;

    public void TransferPropertyToPlayer()
    {
        property.TransferToPlayer();
    }
}
