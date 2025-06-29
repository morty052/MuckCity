using System.Collections.Generic;
using Invector.vCharacterController.AI;
using UnityEngine;

public class EnemyBrain : MonoBehaviour
{
    [SerializeField] List<vControlAICombat> _engagedEnemies = new();
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out vControlAICombat zombie))
        {
            Debug.Log(" enemy entered brain zone");
            if (!_engagedEnemies.Contains(zombie))
            {
                _engagedEnemies.Add(GetComponent<vControlAICombat>());
            }
        }

    }
}
