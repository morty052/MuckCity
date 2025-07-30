using UnityEngine;

public class DelveGateConsole : Interactable
{
    [SerializeField] private GameObject _hologramCurtain;
    [SerializeField] private Canvas _consoleUI;
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
        _hologramCurtain.SetActive(true);
        _consoleUI.gameObject.SetActive(true);
        HideInteractionPrompt();
    }
}
