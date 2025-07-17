using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Rendering;
using Invector;
using ImprovedTimers;
using System;


public class MuckFog : MonoBehaviour
{
    [SerializeField] VolumeProfile _fogProfile;
    [SerializeField] VolumeProfile _defaultProfile;

    vObjectDamage _vObjectDamage;

    [SerializeField] GasMask _gasMask;

    CountdownTimer _countdownTimer;

    void Awake()
    {
        _vObjectDamage = GetComponent<vObjectDamage>();
        _vObjectDamage._shouldDamage = false;
    }



    void OnTriggerEnter(Collider other)
    {
        Debug.Log($"<color=green> Player entered the muck </color>");
        // _defaultProfile = Player.Instance.GetComponentInChildren<Volume>().profile;
        HandleCanDamage();
    }



    void OnTriggerExit(Collider other)
    {
        Debug.Log($"<color=yellow> Player exited the muck </color>");
        Player.Instance.GetComponentInChildren<Volume>().profile = _defaultProfile;
        DisposeTimer();
    }


    void HandleCanDamage()
    {
        Player.Instance.GetComponentInChildren<Volume>().profile = _fogProfile;
        if (SpecialEquipmentManager.Instance.HasEquipment(SpecialEquipmentID.GAS_MASK))
        {
            Debug.Log($"<color=orange> Player Shielded Damage With gas mask</color>");
            _vObjectDamage._shouldDamage = false;
            _gasMask = (GasMask)SpecialEquipmentManager.Instance.GetEquipment(SpecialEquipmentID.GAS_MASK);
            SetUpGasMaskDegradingTimer();
            return;
        }
        else
        {
            _vObjectDamage._shouldDamage = true;
        }

    }

    void SetUpGasMaskDegradingTimer()
    {
        _countdownTimer = new(_gasMask.resistance);
        _countdownTimer.OnTimerStop += () => { _countdownTimer.Start(); DegradeGasMask(); };
        _countdownTimer.Start();
    }

    void DisposeTimer()
    {
        _countdownTimer.Dispose();
        _countdownTimer = null;
    }

    private void DegradeGasMask()
    {
        _gasMask._integrity -= 1;
    }
}
