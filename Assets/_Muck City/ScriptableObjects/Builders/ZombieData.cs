using System;
using UnityEngine;
[CreateAssetMenu(fileName = "ZombieData", menuName = "ScriptableObjects/Zombies", order = 1)]
public class ZombieData : ScriptableObject
{
    public PoolID _poolID;
    public GameObject _prefab;


    public GameObject Spawn()
    {
        GameObject zombie = Instantiate(_prefab);
        // IPoolable poolable = prefab.GetComponent<IPoolable>();
        return zombie;
    }

    public void OnGet(IPoolable zombie)
    {
        zombie.GameObject.SetActive(true);
    }

    public void OnRelease(IPoolable zombie)
    {
        zombie.GameObject.SetActive(false);
    }

    public void OnDestroyPooled(IPoolable zombie)
    {
        Debug.Log("Destroyed Zombie");
    }


}
