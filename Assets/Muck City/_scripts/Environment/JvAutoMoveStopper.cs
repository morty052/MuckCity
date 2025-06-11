using UnityEngine;

public class JvAutoMoveStopper : MonoBehaviour
{

    // Update is called once per frame
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Player.Instance.StopAutoMove();
            Debug.Log("Stopped auto move");
        }
    }
}
