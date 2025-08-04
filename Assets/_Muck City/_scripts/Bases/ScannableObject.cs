using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public struct ScanDetails
{
    public DiscoverableItem _id;
    public string _scanName;
    public string _scanDescription;

    public Image _scanImage;

    public ScanDetails(DiscoverableItem id, string scanName, string scanDescription, Image scanImage)
    {
        _id = id;
        _scanName = scanName;
        _scanDescription = scanDescription;
        _scanImage = scanImage;
    }
}

public class ScannableObject : MonoBehaviour, IScannableObject
{
    public ScanDetails _scanDetails;
    public GameObject GameObject => gameObject;

    public GameObject _ScanCanvas;

    public string ScanDescription { get => _scanDetails._scanDescription; }

    public string ScanName { get => _scanDetails._scanName; }
    public Image ScanImage { get => _scanDetails._scanImage; }

    public string ScanText { get; }

    public bool CanScan => !ScannedObjectUI.Instance._discoverableItems.Contains(_scanDetails);



    public virtual void OnScan()
    {
        Debug.Log($"<color=cyan> Entity {transform.name} Has been Scanned </color>");
        ScannedObjectUI.Instance.OnScanObject(_scanDetails);
    }
}
