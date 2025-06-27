using System;
using Invector.vCharacterController.AI;
using UnityEngine;
using UnityEngine.Pool;

public class Zombie : MonoBehaviour, IPoolable
{
    public IObjectPool<IPoolable> _pool { get; set; }

    public GameObject GameObject => gameObject;

    public PoolID PoolID => PoolID.ZOMBIE;

    public ZombieData _zombieSO;

    public vControlAICombat zombie;

    public bool _spawnOnStart = false;


    void Start()
    {
        if (_spawnOnStart) SpawnZombie();
    }

    private void SpawnZombie()
    {
        GameObject zombiePrefab = _zombieSO.Spawn();
        zombie = zombiePrefab.GetComponent<vControlAICombat>();
        zombie.transform.SetParent(transform, true);
        zombie.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
    }

    public void Release()
    {
        _pool.Release(this);
    }

    public void Die()
    {
        Release();
    }
}
