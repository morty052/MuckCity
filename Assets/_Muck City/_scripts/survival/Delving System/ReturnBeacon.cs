using System;
using Systems.SceneManagement;
using UnityEngine;

public class ReturnBeacon : Interactable
{
    public SceneData _sceneData;

    void OnTriggerEnter(Collider other)
    {
        PrepareInteraction();
    }

    void OnTriggerExit(Collider other)
    {
        HideInteractionPrompt();
    }
    public override void Interact()
    {
        HideInteractionPrompt();
        ReturnToHomeRealm();
    }

    private async void ReturnToHomeRealm()
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
