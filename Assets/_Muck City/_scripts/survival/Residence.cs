using System.Collections.Generic;
using UnityEngine;

public class Residence : Asset
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public bool _isOwned = false;

    public bool _isPlayerOwned = false;

    public List<NpcCharacter> _occupants = new();

    public Transform _reSpawnTransform;

    public int _latestBid = 0;
    public int _rent;


    public void TransferToPlayer()
    {
        _isPlayerOwned = true;
    }
}
