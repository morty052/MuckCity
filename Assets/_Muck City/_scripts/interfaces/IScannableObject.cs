using UnityEngine;

public interface IScannableObject
{
    public GameObject GameObject { get; }

    public bool CanScan { get; }

    public void OnScan();
}
