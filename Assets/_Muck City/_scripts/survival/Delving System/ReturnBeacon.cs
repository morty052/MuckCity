using System;
using Systems.SceneManagement;
using UnityEngine;
using UnityEngine.InputSystem;

public class ReturnBeacon : MonoBehaviour
{
    public SceneData _sceneData;

    void OnTriggerEnter(Collider other)
    {
        PrepareInteraction();
    }

    private void PrepareInteraction()
    {
        // throw new NotImplementedException();
    }

    void OnTriggerExit(Collider other)
    {
        HideInteractionPrompt();
    }

    private void HideInteractionPrompt()
    {
        // throw new NotImplementedException();
    }

    public void Interact()
    {
        HideInteractionPrompt();
        ReturnToHomeRealm();
    }

    public async void ReturnToHomeRealm()
    {
        Debug.Log("Returning to home realm");
        DelveManager.Instance.OnReturnToHomeRealm();
        SceneGroup sceneToLoad = new()
        {
            GroupName = _sceneData.Name,
            Scenes = new() { _sceneData }
        };
        await SceneLoader.Instance.LoadSceneGroup(sceneToLoad, true, true);
    }
}



public class HoldToControlAnimation
{
    public InputActionReference holdAction; // Assign in Inspector
    public Animator animator;               // Assign your Animator
    public float maxSpeed = 3f;             // Max playback speed
    public float holdTimeToMax = 3f;        // Time to reach max speed

    private float holdStartTime = 0f;
    private bool isHolding = false;

    void Update()
    {
        var action = holdAction.action;

        if (action.WasPressedThisFrame())
        {
            holdStartTime = Time.time;
            isHolding = true;
        }

        if (action.IsPressed() && isHolding)
        {
            float heldDuration = Time.time - holdStartTime;
            float speedFactor = Mathf.Clamp01(heldDuration / holdTimeToMax);
            animator.speed = Mathf.Lerp(1f, maxSpeed, speedFactor);
        }

        if (action.WasReleasedThisFrame())
        {
            animator.speed = 1f; // Reset to normal speed
            isHolding = false;
        }
    }
}