using System;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Pool;
[CreateAssetMenu(fileName = "PoolData", menuName = "ScriptableObjects/PoolData", order = 1)]
public class PoolData : ScriptableObject
{
    public PoolID _poolID;
    public GameObject _prefab;


    public IPoolable Create()
    {
        GameObject prefab = Instantiate(_prefab);
        IPoolable poolable = prefab.GetComponent<IPoolable>();
        return poolable;
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
