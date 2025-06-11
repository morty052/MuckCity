using UnityEngine;
using UnityEngine.Pool;

public class Zombie : MonoBehaviour, IPoolable
{
    public IObjectPool<Zombie> _pool;

    public GameObject GameObject => gameObject;

    public PoolID PoolID => PoolID.ZOMBIE;
}
