using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.VFX;
using System.Threading.Tasks;

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

    public VisualEffect _harvestEffect;
    public string ScanDescription { get => _scanDetails._scanDescription; }

    public string ScanName { get => _scanDetails._scanName; }
    public Image ScanImage { get => _scanDetails._scanImage; }

    public string ScanText { get; }

    public bool CanScan => !ScannedObjectUI.Instance._discoverableItems.Contains(_scanDetails);


    void Start()
    {
        _harvestEffect = ScannedObjectUI.Instance._harvestVFX;
    }


    public virtual void OnScan()
    {
        Debug.Log($"<color=cyan> Entity {transform.name} Has been Scanned </color>");
        ScannedObjectUI.Instance.OnScanObject(_scanDetails);

    }

    [Button]
    public virtual void Harvest(Transform player)
    {
        Mesh mesh = GetComponent<MeshFilter>().sharedMesh;

        // *Get Direction of transform to player
        // Vector3 dir = (player.position - transform.position).normalized * Vector3.Distance(transform.position, player.position);
        Vector3 dir = new(5, 5, 10);
        if (ABUtils.IsAhead(transform, player))
        {
            dir = new(5, 5, 10);
        }

        else
        {
            dir = new(5, 5, -10);
        }

        _harvestEffect.SetVector3("Suck Direction", dir);

        _harvestEffect.SetMesh("Sampled Mesh", mesh);

        _harvestEffect.Stop();
        gameObject.SetActive(false);
        _harvestEffect.transform.position = transform.position;
        _harvestEffect.gameObject.SetActive(true);

        _harvestEffect.Play();
        ABUtils.StartLerp(_harvestEffect.transform, player, 3, 0.5f);


        Debug.Log($"dir {dir}, IsAhead {ABUtils.IsAhead(transform, player)}");

    }


}





public class TransformLerper
{
    public Transform target;
    public float duration = 3f;

    public Transform floater;

    public async void StartLerp()
    {
        await LerpToTarget(floater, target.position, duration);
    }

    private async Task LerpToTarget(Transform obj, Vector3 destination, float time)
    {
        Vector3 startPos = obj.position;
        float elapsed = 0f;

        while (elapsed < time)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / time);
            obj.position = Vector3.Lerp(startPos, destination, t);
            await Task.Yield(); // Wait for next frame
        }

        obj.position = destination; // Snap to final position
    }
}