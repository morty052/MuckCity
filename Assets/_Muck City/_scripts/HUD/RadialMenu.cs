using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Sirenix.OdinInspector;

public class RadialMenu : MonoBehaviour
{
    [Header("UI Elements to Arrange")]
    public List<RectTransform> uiElements;

    [Header("Circle Settings")]
    [OnValueChanged("ArrangeInCircle")] public float radius = 100f;
    public float startAngle = 0f;
    public bool clockwise = true;

    public RectTransform _dial; // The rotating UI element
    public Transform _centerPoint; // The center of the circle (can be the dial itself)

    DialRotator _dialRotator;

    public bool UseDial => _dial != null;

    void Awake()
    {
        if (UseDial)
        {
            _dialRotator = new(_dial, _centerPoint);
        }
    }

    void Start()
    {
        ArrangeInCircle();
    }

    void Update()
    {
        if (UseDial)
        {
            _dialRotator.Update();
        }
    }

    public List<RectTransform> GetSlots()
    {
        return uiElements;
    }

    public void OnSlotClicked(SpecialEquipmentSlot slot)
    {

    }
    public void OnSlotHovered(SpecialEquipmentSlot slot)
    {
        _dialRotator._selectedTarget = slot.transform;
    }

    [Button("Arrange")]
    public void ArrangeInCircle()
    {
        if (uiElements == null || uiElements.Count == 0) return;

        float angleStep = 360f / uiElements.Count;
        float direction = clockwise ? -1f : 1f;

        for (int i = 0; i < uiElements.Count; i++)
        {
            float angle = startAngle + direction * angleStep * i;
            float rad = angle * Mathf.Deg2Rad;

            Vector2 position = new Vector2(
                Mathf.Cos(rad) * radius,
                Mathf.Sin(rad) * radius
            );

            uiElements[i].anchoredPosition = position;
        }
    }
}



public class DialRotator
{
    [Header("Dial Settings")]
    public RectTransform _dial; // The rotating UI element
    public Transform _centerPoint; // The center of the circle (can be the dial itself)

    [Header("Target")]
    public Transform _selectedTarget; // The target to look at

    public DialRotator(RectTransform dial, Transform centerPoint)
    {
        _dial = dial;
        _centerPoint = centerPoint;
    }

    // public void Update()
    // {
    //     if (_selectedTarget == null || _dial == null || _centerPoint == null) return;

    //     // Get direction from center to target
    //     Vector2 direction = _selectedTarget.position - _centerPoint.position;

    //     // Calculate angle in degrees
    //     float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

    //     // Apply rotation (UI uses Z-axis for rotation)
    //     _dial.rotation = Quaternion.Euler(0f, 0f, angle);
    // }


    public void Update()
    {
        if (_selectedTarget == null || _dial == null || _centerPoint == null) return;

        // Get direction from center to target
        Vector2 direction = _selectedTarget.position - _centerPoint.position;

        // Calculate angle in degrees
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // Apply rotation (UI uses Z-axis for rotation) with lerping
        _dial.rotation = Quaternion.Slerp(_dial.rotation, Quaternion.Euler(0f, 0f, angle), Time.deltaTime * 5f);
    }
}