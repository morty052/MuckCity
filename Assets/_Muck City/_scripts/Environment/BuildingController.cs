using System;
using UnityEngine;

public class BuildingController : MonoBehaviour
{

    [SerializeField] GameObject _interior;

    [SerializeField] Door[] _entrances;

    [SerializeField] bool _playerIsInBuilding = false;
    [SerializeField] private float _inactiveDelay = 3f;

    void OnTriggerExit()
    {
        if (IsPlayerAheadOfPos())
        {
            Debug.Log("Player is ahead of the door");
            _playerIsInBuilding = true;
        }

        else
        {
            Debug.Log("Player is behind the door");
            _playerIsInBuilding = false;
            Invoke(nameof(GoInactive), _inactiveDelay);
        }
    }

    private void GoInactive()
    {
        if (_playerIsInBuilding) return;
        _interior.SetActive(false);
    }

    public void HandleDoorInteraction()
    {
        Debug.Log("Player trying to interact with door");
        if (_playerIsInBuilding) return;
        if (!_interior.activeSelf)
        {
            _interior.SetActive(true);
        }
    }

    bool IsPlayerAheadOfPos()
    {
        float dot = Vector3.Dot(transform.forward, (Player.Instance.transform.position - transform.position).normalized);
        // Debug.Log("Dot is " + dot);
        return dot > 0;
    }
}
