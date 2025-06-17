using UnityEngine;

public class AreaToggle : MonoBehaviour
{

    [SerializeField] GameObject _objectToDeactivate;
    [SerializeField] GameObject _objectToActivate;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void OnTriggerEnter()
    {
        if (_objectToActivate != null)
        {
            _objectToActivate.SetActive(true);
        }

        if (_objectToDeactivate != null)
        {
            _objectToDeactivate.SetActive(false);
        }

        Destroy(gameObject);
    }

}
