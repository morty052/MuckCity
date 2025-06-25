using System.Collections;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class TimeManager : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _timeText;
    [SerializeField, InlineEditor] TimeSettings _timeSettings;
    [SerializeField] Light _sun;
    [SerializeField] Light _moon;

    [SerializeField] GameObject _skyDome;

    Material _skyMaterial;

    [SerializeField] AnimationCurve _lightIntensityCurve;

    [SerializeField] float _maxSunIntensity = 1;
    [SerializeField] float _maxMoonIntensity = 0.5f;

    [SerializeField] Color _dayAmbientLight;
    [SerializeField] Color _nightAmbientLight;

    [SerializeField] Volume _volume;

    ColorAdjustments _colorAdjustments;

    TimeService _service;

    bool IsAm => _service.CurrentTime.Hour < 12;

    string _am = "AM";
    string _pm = "PM";

    void OnEnable()
    {
        TimeService.OnHourChange += DoTimeStuff;
    }

    void OnDisable()
    {
        TimeService.OnHourChange -= DoTimeStuff;
    }

    void DoTimeStuff(int hour)
    {
        // Debug.Log("Hour is " + hour);
    }

    void Awake()
    {
        _skyMaterial = _skyDome.GetComponent<MeshRenderer>().material;
    }

    void Start()
    {
        _service = new TimeService(_timeSettings);
        _volume.profile.TryGet(out _colorAdjustments);
    }

    void Update()
    {
        UpdateTimeofDay();
        RotateSun();
        UpDateLightSettings();
        UpdateSkyBlend();
    }

    void RotateSun()
    {
        float rotation = _service.CalculateSunAngle();
        _sun.transform.rotation = Quaternion.AngleAxis(rotation, Vector3.right);
    }

    void UpDateLightSettings()
    {
        float dotProduct = Vector3.Dot(_sun.transform.forward, Vector3.down);
        _sun.intensity = Mathf.Lerp(0, _maxSunIntensity, _lightIntensityCurve.Evaluate(dotProduct));
        _moon.intensity = Mathf.Lerp(_maxMoonIntensity, 0, _lightIntensityCurve.Evaluate(dotProduct));

        if (_colorAdjustments == null) return;

        _colorAdjustments.colorFilter.value = Color.Lerp(_nightAmbientLight, _dayAmbientLight, _lightIntensityCurve.Evaluate(dotProduct));
    }

    void UpdateTimeofDay()
    {
        _service.UpdateTime(Time.deltaTime);
        if (_timeText != null)
        {
            // _timeText.text = _service.CurrentTime.ToString("hh") + $" {(IsAm ? _am : _pm)}".Replace("0", " ");
            _timeText.text = (_service.CurrentTime.Hour % 12 == 0 ? 12 : _service.CurrentTime.Hour % 12).ToString() + $" {(IsAm ? _am : _pm)}";
        }
    }


    void UpdateSkyBlend()
    {
        float dotProduct = Vector3.Dot(_sun.transform.forward, Vector3.up);
        float blend = Mathf.Lerp(0, 0.4f, _lightIntensityCurve.Evaluate(dotProduct));

        _skyMaterial.mainTextureOffset = new Vector2(blend, 0);
    }
}
