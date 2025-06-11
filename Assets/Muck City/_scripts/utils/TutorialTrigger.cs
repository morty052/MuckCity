using System;
using UnityEngine;

public class TutorialTrigger : MonoBehaviour
{
    [SerializeField] EventTutorial _tutorial = new();

    public void SetTutorial(EventTutorial eventTutorial)
    {
        _tutorial = eventTutorial;
    }

    // Update is called once per frame
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            HudManager.Instance.ToggleTutorialPrompt(true, _tutorial._pcTitle, _tutorial._pcDescription, _tutorial._image);
            Destroy(gameObject);
        }

    }
}
