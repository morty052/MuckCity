using UnityEngine;
using UnityEngine.AI;

public class Dog : MonoBehaviour
{
    NavMeshAgent _agent;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
    }


}
