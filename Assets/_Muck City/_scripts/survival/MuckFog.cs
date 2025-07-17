using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Rendering;
using Invector;


public class MuckFog : MonoBehaviour
{
    [SerializeField] VolumeProfile _fogProfile;
    [SerializeField] VolumeProfile _defaultProfile;

    vObjectDamage _vObjectDamage;

    void Awake()
    {
        _vObjectDamage = GetComponent<vObjectDamage>();
        _vObjectDamage.enabled = false;
    }

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
