using UnityEngine;

public class TheBelly : MonoBehaviour
{
    BoxCollider _collider;

    void Awake()
    {
        _collider = GetComponent<BoxCollider>();
    }

    
}
