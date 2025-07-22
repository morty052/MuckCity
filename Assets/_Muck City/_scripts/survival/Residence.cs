using System.Collections.Generic;
using UnityEngine;

public class Residence : Asset
{
    public PropertyID _propertyID;
    public bool _isOwned = false;

    public bool _isPlayerOwned = false;

    [SerializeField] private List<GameObject> _energyObjects = new();

    public List<IUseEnergy> _energyConsumers = new();

    public List<NpcCharacter> _occupants = new();

    public Transform _reSpawnTransform;

    public int _latestBid = 0;
    public int _rent = 0;
    public int _energyCost;

    public bool _hasAmI;

    public ResidenceConfigSO _data;


    void Awake()
    {
        for (int i = 0; i < _energyObjects.Count; i++)
        {
            _energyConsumers.Add(_energyObjects[i].GetComponent<IUseEnergy>());
        }
    }

    public void TransferToPlayer()
    {
        _isPlayerOwned = true;
    }
}
