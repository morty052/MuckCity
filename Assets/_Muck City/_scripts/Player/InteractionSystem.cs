using System.Collections.Generic;
using ImprovedTimers;
using UnityEngine;

[System.Serializable]
public class InteractionSystem
{
    CountdownTimer _detectionTimer;

    float _detectionRate = 0.2f;

    public float _interactionRange = 1f;

    [SerializeField] List<IInteractable> _closestInteractables = new();

    [SerializeField] LayerMask _interactionLayerMask = new();

    [Tooltip("Should also include interactables for obstruction check to function properly")]
    [SerializeField] LayerMask _obstructionLayerMask = new();

    public bool _debug;

    Vector3 _playerLastPos;

    Transform _transform;


    public InteractionSystem(float interactionRange, float detectionRate, Transform transform, LayerMask interactionlayer, LayerMask defaultLayerMask)
    {
        _transform = transform;
        _interactionRange = interactionRange;
        _interactionLayerMask = interactionlayer;
        _detectionTimer = new(detectionRate);
        _obstructionLayerMask = defaultLayerMask;
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
        //* Check if difference between player pos last check and current check is greater than threshold

        // if (Vector3.Distance(_playerLastPos, _transform.position) < 1f)
        // {
        //     Debug.Log("Not enough Movement");
        //     return;
        // }
        // else
        // {

        //     Debug.Log("Player is past check threshold");
        // }
        _playerLastPos = _transform.position;
        Collider[] hitColliders = Physics.OverlapSphere(_transform.position + Vector3.up, _interactionRange, _interactionLayerMask);
        if (hitColliders.Length > 0)
        {
            foreach (Collider hitCollider in hitColliders)
            {
                if (!CheckIfObjectIsBetween(hitCollider.gameObject))
                {
                    IInteractable interactable = hitCollider.GetComponent<IInteractable>();
                    if (interactable != null && !_closestInteractables.Contains(interactable) && interactable.CanInteract && !CheckIfObjectIsBetween(hitCollider.gameObject))
                    {
                        interactable.ToggleDrawAttention();
                        _closestInteractables.Add(interactable);
                        if (_debug)
                        {
                            Debug.Log("added interactable: " + interactable.GameObject.name);
                        }
                    }
                }

                else
                {
                    for (int i = _closestInteractables.Count - 1; i >= 0; i--)
                    {
                        IInteractable interactable = _closestInteractables[i];
                        if (interactable.GameObject.transform.position == hitCollider.transform.position)
                        {
                            interactable.ToggleDrawAttention();
                            _closestInteractables.RemoveAt(i);
                        }
                    }
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


    bool CheckIfObjectIsBetween(GameObject targetObject)
    {

        Camera mainCamera = Camera.main;
        // Create a ray from the main camera to the target object
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        ray.direction = (targetObject.transform.position - mainCamera.transform.position).normalized;

        // Cast the ray
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, _obstructionLayerMask))
        {
            // Check if the hit object is the target object
            if (hit.transform == targetObject.transform)
            {
                if (_debug)
                {
                    Debug.Log("There are no objects between the camera and" + targetObject.name);
                }
                return false;
            }
            else
            {
                if (_debug)
                {
                    Debug.Log("There is an object between the camera and" + targetObject.name + "Object hit is " + hit.transform.name);
                }
                return true;
            }
        }
        else
        {
            Debug.Log("No objects were hit by the ray");
        }

        return true;
    }
}





