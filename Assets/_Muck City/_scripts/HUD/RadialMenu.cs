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

    void Start()
    {
        ArrangeInCircle();
    }

    public List<RectTransform> GetSlots()
    {
        return uiElements;
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