using UnityEngine;

public interface IScannableObject
{
    public GameObject GameObject { get; }

    public void OnScan();
}
