using System.Collections.Generic;
using ImprovedTimers;
using UnityEngine;

public class InteractionSystem
{
    CountdownTimer _detectionTimer;

    float _detectionRate = 0.2f;

    public float _interactionRange = 1f;

    [SerializeField] List<IInteractable> _closestInteractables = new();

    [SerializeField] LayerMask _interactionLayerMask = new();

    Transform _transform;


    public InteractionSystem(float interactionRange, float detectionRate, Transform transform, LayerMask interactionlayer)
    {
        _transform = transform;
        _interactionRange = interactionRange;
        _interactionLayerMask = interactionlayer;
        _detectionTimer = new(detectionRate);
        _detectionTimer.OnTimerStop += () =>
        {
            EnvironmentInteraction();
            _detectionTimer.Start();
        };

        _detectionTimer.Start();
    }

    public void Dispose()
    {
        _detectionTimer.Dispose();
    }

    public void EnvironmentInteraction()
    {

        Collider[] hitColliders = Physics.OverlapSphere(_transform.position + Vector3.up, _interactionRange, _interactionLayerMask);
        if (hitColliders.Length > 0)
        {
            foreach (Collider hitCollider in hitColliders)
            {
                IInteractable interactable = hitCollider.GetComponent<IInteractable>();
                if (!interactable.IsHighlighted)
                {
                    interactable.ToggleDrawAttention();
                    _closestInteractables.Add(interactable);
                    Debug.Log("added interactable: " + interactable.GameObject.name);
                }
            }

        }

        if (_closestInteractables.Count > 0)
        {
            for (int i = _closestInteractables.Count - 1; i >= 0; i--)
            {
                IInteractable interactable = _closestInteractables[i];
                if (Vector3.Distance(interactable.GameObject.transform.position, _transform.position) > _interactionRange)
                {
                    interactable.ToggleDrawAttention();
                    _closestInteractables.RemoveAt(i);
                }
            }
        }

        if (hitColliders.Length == 0)
        {
            if (_closestInteractables.Count > 0)
            {
                for (int i = _closestInteractables.Count - 1; i >= 0; i--)
                {
                    IInteractable interactable = _closestInteractables[i];
                    interactable.ToggleDrawAttention();
                    _closestInteractables.RemoveAt(i);
                }
            }
        }

    }
}
