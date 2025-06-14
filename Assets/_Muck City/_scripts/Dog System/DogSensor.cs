using System;
using System.Collections.Generic;
using ImprovedTimers;
using Sirenix.OdinInspector;
using UnityEngine;

public class DogSensor : MonoBehaviour
{

    CountdownTimer _detectionTimer;
    public float _detectionRate = 1;

    public float _detectionRange = 5;
    public LayerMask _detectionLayerMask = new();

    public bool _playerIsInRange = false;

    public static Action OnPlayerExitRange;

    [ShowInInspector]
    public HashSet<GameObject> _detectedEnemies = new();

    void Start()
    {
        _detectionTimer = new CountdownTimer(_detectionRate);
        _detectionTimer.OnTimerStop += () =>
        {
            DetectWithSphereOverlap();
            _detectionTimer.Start();
        };
        _detectionTimer.Start();
    }

    void DetectWithSphereOverlap()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, _detectionRange, _detectionLayerMask);
        if (colliders.Length > 0)
        {

            foreach (Collider collider in colliders)
            {
                if (collider.CompareTag("Player")) // if player is in range
                {
                    _playerIsInRange = true;
                    Debug.Log("Dog Detected Player");

                }
                else                                 // if player is out of range
                {
                    _playerIsInRange = false;
                    OnPlayerExitRange?.Invoke();
                    Debug.Log("Player Out of Range");
                }
                if (collider.CompareTag("Enemy"))             // if enemy is in range
                {
                    if (!_detectedEnemies.Contains(collider.gameObject))
                        _detectedEnemies.Add(collider.gameObject);
                    Debug.Log("Dog Detected" + collider.name);
                }

                else                                 // if enemy is out of range
                {
                    _detectedEnemies.Clear();

                }

            }

        }


    }

    // Update is called once per frame
    void OnDrawGizmos()
    {
        Gizmos.color = _playerIsInRange ? Color.green : Color.red;
        Gizmos.DrawWireSphere(transform.position, _detectionRange);
    }
}
