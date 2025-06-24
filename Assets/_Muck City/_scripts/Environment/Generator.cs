using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using DG.Tweening;
using Unity.Mathematics;

[System.Serializable]
public struct LightStruct
{
    [HideLabel]
    public Color _defaultColor;
    public float _defaultIntensity;
    public Light _light;
    public LightStruct(Light light)
    {
        _defaultColor = light.color;
        _defaultIntensity = light.intensity;
        _light = light;
    }
}

public class Generator : Equipment
{
    [TabGroup("Lights")]
    // [SerializeField] GameObject[] lights;
    [SerializeField] Transform _lightsParent;
    [TabGroup("Lights")]
    public Color _backupLightColor;

    [TabGroup("Debug")]
    [SerializeField] bool _debug;

    [TabGroup("Lights")]
    [SerializeField] List<LightStruct> _lightStructs;

    bool _isPoweredUp = false;

    void Awake()
    {
        for (int i = 0; i < _lightsParent.childCount; i++)
        {
            _lightStructs.Add(new LightStruct(_lightsParent.GetChild(i).GetComponent<Light>()));
        }
        if (!_isPoweredUp)
        {
            EnterBackUpPowerMode();
        }
    }

    [Button("Turn Off"), TabGroup("Lights")]
    void EnterBackUpPowerMode()
    {
        for (int i = 0; i < _lightStructs.Count; i++)
        {
            _lightStructs[i]._light.DOColor(_backupLightColor, 0.5f);
        }
        _isPoweredUp = false;
    }

    [Button("Turn On"), TabGroup("Lights")]
    void EnterPoweredUpMode()
    {
        for (int i = 0; i < _lightStructs.Count; i++)
        {
            _lightStructs[i]._light.DOColor(_lightStructs[i]._defaultColor, 0.5f);
        }
        _isPoweredUp = true;
    }
    public override void Interact()
    {
        if (!_canInteract) return;
        if (!_isPoweredUp)
        {
            EnterPoweredUpMode();
            _actionText.SetText("Turn Off");
        }
        else
        {
            EnterBackUpPowerMode();
            _actionText.SetText("Turn On");
        }


        if (IsQuestItem)
        {
            QuestItem questItem = GetComponent<QuestItem>();
            OnInteracted?.Invoke(questItem._questItemData._tag);
        }
    }

    void OnTriggerExit(Collider other)
    {
        _actionText.HideInteractionPrompt();
    }


}
