using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public struct ScanDetails
{
    public string _scanName;
    public string _scanDescription;
    public string _scanText;

    public Image _scanImage;

    public ScanDetails(string scanName, string scanDescription, string scanText, Image scanImage)
    {
        _scanName = scanName;
        _scanDescription = scanDescription;
        _scanText = scanText;
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

    public virtual void OnScan()
    {
        Debug.Log($"<color=cyan> Entity {transform.name} Has been Scanned </color>");
        ScannedObjectUI.Instance.OnScanObject(_scanDetails);
    }
}
