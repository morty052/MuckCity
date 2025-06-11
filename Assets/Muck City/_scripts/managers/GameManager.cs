using System;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    [SerializeField] private DeliveryManager _deliveryManagerPrefab;
    [SerializeField] private DomeManager _domeManagerPrefab;

    CatalogueManager _catalogueManager;


    public List<Locations> _marketingAreas = new();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            _catalogueManager = GetComponent<CatalogueManager>();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void OnEnable()
    {
        GameEventsManager.OnDeliveryMarkerPlacedEvent += AddNewMarkertingArea;
    }

    void OnDisable()
    {
        GameEventsManager.OnDeliveryMarkerPlacedEvent += AddNewMarkertingArea;
    }

    private void AddNewMarkertingArea(Locations districtID)
    {
        _marketingAreas.Add(districtID);
    }

    void Start()
    {
        if (_catalogueManager.HasItems())
        {
            Instantiate(_deliveryManagerPrefab);
        }
        // Instantiate(_domeManagerPrefab);
    }


}
