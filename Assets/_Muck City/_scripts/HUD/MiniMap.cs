using UnityEngine;

public class MiniMap : MonoBehaviour
{
    public static MiniMap Instance { get; private set; }
    public GameObject _deliveryMarkerPrefab;
    [SerializeField] Transform _markersParent;
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }

        else
        {
            Destroy(gameObject);
        }
    }

    // Update is called once per frame
    public void AddMarker(Vector3 pos)
    {
        Instantiate(_deliveryMarkerPrefab);
        _deliveryMarkerPrefab.transform.position = new(pos.x, _deliveryMarkerPrefab.transform.position.y, pos.z);
    }
    public GameObject GetMarker(Transform parent = null)
    {
        if (parent != null)
        {
            GameObject marker = Instantiate(_deliveryMarkerPrefab, parent);
            return marker;
        }

        else
        {
            GameObject marker = Instantiate(_deliveryMarkerPrefab);
            return marker;
        }
    }
}
