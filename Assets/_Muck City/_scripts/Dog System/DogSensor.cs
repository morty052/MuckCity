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
    public LayerMask _playerLayerMask = new();
    public LayerMask _enemyLayerMask = new();

    [ShowInInspector]
    private bool _playerIsInRange = false;

    public bool PlayerIsInRange => _playerIsInRange;
    public bool EnemiesInSight => _detectedEnemies.Count > 0;


    public static Action OnPlayerExitRange;

    [ShowInInspector]
    public HashSet<GameObject> _detectedEnemies = new();

    void Start()
    {
        _detectionTimer = new CountdownTimer(_detectionRate);
        _detectionTimer.OnTimerStop += () =>
        {
            SenseEnvironment();
            _detectionTimer.Start();
        };
        _detectionTimer.Start();
    }

    void SenseEnvironment()
    {
        DetectPlayer();
        DetectEnemies();
    }

    void DetectPlayer()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, _detectionRange, _playerLayerMask);
        if (colliders.Length > 0)
        {
            _playerIsInRange = true;
        }

        else                              // if player is out of range
        {
            _playerIsInRange = false;
        }
    }
    void DetectEnemies()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, _detectionRange, _enemyLayerMask);
        if (colliders.Length > 0)
        {

            foreach (Collider collider in colliders)
            {

                if (collider.CompareTag("Enemy"))             // if enemy is in range
                {
                    if (!_detectedEnemies.Contains(collider.gameObject))
                        _detectedEnemies.Add(collider.gameObject);
                }


            }

        }

        else                             // if player is out of range
        {
            if (_detectedEnemies.Count > 0)                                 // if enemy is out of range
            {
                _detectedEnemies.Clear();

            }
        }
    }

    public GameObject GetClosestEnemy()
    {
        if (_detectedEnemies.Count == 0) return null;
        GameObject closestEnemy = null;
        float closestDistance = Mathf.Infinity;
        foreach (GameObject enemy in _detectedEnemies)
        {
            float distance = Vector3.Distance(transform.position, enemy.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestEnemy = enemy;
            }
        }
        return closestEnemy;
    }
    // Update is called once per frame
    void OnDrawGizmos()
    {
        Gizmos.color = _playerIsInRange ? Color.green : Color.red;
        Gizmos.DrawWireSphere(transform.position, _detectionRange);
    }
}
