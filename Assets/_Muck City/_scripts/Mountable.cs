using UnityEngine;
using System;

public class Mountable : MonoBehaviour, IFindable
{
    public bool _playerCanMount = true;
    [SerializeField] bool _isQuestItem;
    public GameObject GameObject => gameObject;

    public bool IsQuestItem { get => _isQuestItem; set => _isQuestItem = value; }
    public Action<string> OnInteracted;
    public void SetupInteractionListener(Action<string> action)
    {
        OnInteracted += action;
    }

    public void RemoveInteractionListener(Action<string> action)
    {
        OnInteracted -= action;
    }

    public void DisableMount()
    {

    }
}
