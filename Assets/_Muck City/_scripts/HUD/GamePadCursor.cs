using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class GamepadCursorControl : MonoBehaviour
{
    public float cursorSpeed = 1000f;
    public RectTransform cursorRect;
    public Canvas canvas;

    void Update()
    {
        Vector2 input = new(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        Vector2 move = cursorSpeed * Time.deltaTime * input;

        // Move cursor
        cursorRect.anchoredPosition += move;

        // Clamp cursor to screen bounds
        Vector2 clampedPos = new(
            Mathf.Clamp(cursorRect.anchoredPosition.x, 0, canvas.pixelRect.width),
            Mathf.Clamp(cursorRect.anchoredPosition.y, 0, canvas.pixelRect.height)
        );
        cursorRect.anchoredPosition = clampedPos;

        // Simulate click with "Submit" button (usually A on Xbox)
        if (Input.GetButtonDown("Submit"))
        {
            PointerEventData pointer = new(EventSystem.current)
            {
                position = cursorRect.position
            };

            List<RaycastResult> results = new();
            EventSystem.current.RaycastAll(pointer, results);

            foreach (RaycastResult result in results)
            {
                ExecuteEvents.Execute(result.gameObject, pointer, ExecuteEvents.pointerClickHandler);
            }
        }
    }
}
