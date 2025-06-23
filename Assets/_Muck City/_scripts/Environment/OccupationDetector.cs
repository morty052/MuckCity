using UnityEngine;

public class OccupationDetector : MonoBehaviour
{

    [SerializeField] int _level;
    [SerializeField] LiftSwitch _liftSwitch;


    void OnTriggerEnter(Collider other)
    {
        if (_liftSwitch._isMoving) return;
        if (_liftSwitch.IsOnLevel(_level))
        {
            _liftSwitch.OpenBarriers(_level);
        }

    }



    // Update is called once per frame
    void OnTriggerExit()
    {
        if (_liftSwitch._isMoving) return;
        _liftSwitch.TryCloseBarriersOnExit(_level);
    }
}
