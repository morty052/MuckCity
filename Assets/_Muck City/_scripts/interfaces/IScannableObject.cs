using UnityEngine;

public interface IScannableObject
{
    public GameObject GameObject { get; }

    public bool CanScan { get; }

    public void OnScan();
}
public interface IHarvestableObject
{
    public GameObject GameObject { get; }

    public bool CanHarvest { get; }

    public void OnHarvest();
}
