using UnityEngine;

public class District : MonoBehaviour
{
    public Locations _districtID;
    [SerializeField] DistrictExit _exit;

    [SerializeField] bool _playerIsInCompound;


    public void TogglePlayerPresence(bool state)
    {
        _playerIsInCompound = state;
        if (state)
        {
            GameEventsManager.Instance.OnDistrictEnter(this);
        }
        else
        {
            GameEventsManager.Instance.OnDistrictExit(this);
        }
    }

    public void OnDeliveryMarkerPlaced()
    {
        Debug.Log(" Player has placed delivery marker in district " + _districtID);
        GameEventsManager.OnDeliveryMarkerPlacedEvent(_districtID);
    }
}
