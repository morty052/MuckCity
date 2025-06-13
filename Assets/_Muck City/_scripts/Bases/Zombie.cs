using Invector.vCharacterController.AI;
using UnityEngine;
using UnityEngine.Pool;

public class Zombie : MonoBehaviour, IPoolable
{
    public IObjectPool<Zombie> _pool;

    public GameObject GameObject => gameObject;

    public PoolID PoolID => PoolID.ZOMBIE;

    public ZombieData _zombieSO;

    public vControlAICombat zombie;


    void Start()
    {
        GameObject zombiePrefab = _zombieSO.Spawn();
        zombie = zombiePrefab.GetComponent<vControlAICombat>();
        zombie.transform.SetParent(transform, true);
    }
}
