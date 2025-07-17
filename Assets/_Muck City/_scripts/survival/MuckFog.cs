using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Rendering;


public class MuckFog : MonoBehaviour
{
    [SerializeField] VolumeProfile _fogProfile;
    [SerializeField] VolumeProfile _defaultProfile;

    void OnTriggerEnter(Collider other)
    {
        Debug.Log($"<color=green> Player entered the muck </color>");
        // _defaultProfile = Player.Instance.GetComponentInChildren<Volume>().profile;
        Player.Instance.GetComponentInChildren<Volume>().profile = _fogProfile;
    }

    void OnTriggerExit(Collider other)
    {
        Debug.Log($"<color=yellow> Player exited the muck </color>");
        Player.Instance.GetComponentInChildren<Volume>().profile = _defaultProfile;
    }


}
