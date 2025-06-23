using UnityEngine;

public class OccupationDetector : MonoBehaviour
{
    public Observer<bool> _isOccupied = new(false);

    void OnTriggerEnter()
    {
        _isOccupied.Value = true;
    }

    // Update is called once per frame
    void OnTriggerExit()
    {
       _isOccupied.Value = false; 
    }
}
