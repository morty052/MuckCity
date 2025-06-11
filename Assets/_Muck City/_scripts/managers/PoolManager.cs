using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;
using UnityEngine.Pool;


public interface IPoolable
{
    GameObject GameObject { get; }
    public PoolID PoolID { get; }
    // void OnRelease();
}

public enum PoolID
{
    NULL = 0,
    ZOMBIE = 1,
    MUSHROOM = 2
}

public class PoolManager : MonoBehaviour
{


    public PoolID _activePoolID = PoolID.ZOMBIE;
    [SerializeField] Zombie _zombiePrefab;
    // private IObjectPool<Zombie> _zombiePool;

    public List<PoolData> _objectsToPool = new();

    [ShowInInspector]
    private Dictionary<PoolID, IObjectPool<IPoolable>> _pools = new();
    [SerializeField] private int _defaultCapacity = 8;
    [SerializeField] private int _maxSize = 50;
    [SerializeField] bool _collectionCheck;

    [SerializeField] private int _itemCount;

    [OnValueChanged("UpdateCursorPosition")]
    [SerializeField] private Vector3 _spawnPosition;

    [OnValueChanged("UpdateCursorSize")]
    [SerializeField] private float _spawnRadius;

    [SerializeField] BoxCollider _cursorBox;

    [ShowInInspector]
    public List<IPoolable> _activeSpawns = new();
    public PoolID _filterByID;

    private bool IsPoolAvailable => _pools.Count > 0;
    void Awake()
    {
        // _zombiePool = new ObjectPool<Zombie>(CreateZombie, OnGetZombie, OnReleaseZombie, OnDestroyPooledZombie, _collectionCheck, _defaultCapacity, _maxSize);
        CreatePools();
    }



    void Start()
    {
        Invoke(nameof(SetSpawnPositionToPlayer), 1f);
    }


    void CreatePools()
    {
        foreach (PoolData data in _objectsToPool)
        {
            IObjectPool<IPoolable> pool = new ObjectPool<IPoolable>(data.Create, data.OnGet, data.OnRelease, data.OnDestroyPooled, _collectionCheck, _defaultCapacity, _maxSize);
            _pools.Add(data._poolID, pool);
        }
    }

    private IPoolable GetPoolable(PoolID poolID)
    {
        IObjectPool<IPoolable> pool = _pools[poolID];
        IPoolable zombie = pool.Get();
        return zombie;
    }


    [Button("Spawn Many"), ButtonGroup("Zombies")]
    public void SpawnItemsWithinRadius(int itemCount = 0)
    {
        if (itemCount == 0)
        {
            itemCount = _itemCount;
        }
        for (int i = 0; i < itemCount; i++)
        {
            SpawnGeneric(GetRandomPosition());
        }
        _pools.Clear();
    }

    [Button("Destroy All"), ButtonGroup("Zombies")]
    public void DestroySpawnedZombies()
    {
        foreach (IPoolable zombie in _activeSpawns)
        {
            DestroyImmediate(zombie.GameObject);
        }
        _activeSpawns.Clear();
    }
    Vector3 GetRandomPosition()
    {
        // float halfWidth = _cursorBox.size.x;
        // float halfHeight = _cursorBox.size.z;

        float width = _cursorBox.transform.lossyScale.x;
        float height = _cursorBox.transform.lossyScale.z;

        float _maxDistX = _cursorBox.transform.position.x + width / 2;
        float _minDistX = _cursorBox.transform.position.x - width / 2;
        float _maxDistZ = _cursorBox.transform.position.z - height / 2;
        float _minDistZ = _cursorBox.transform.position.z + height / 2;

        float randomX = UnityEngine.Random.Range(_minDistX, _maxDistX);
        float randomZ = UnityEngine.Random.Range(_minDistZ, _maxDistZ);

        Vector3 vector3 = new(randomX, _cursorBox.transform.position.y, randomZ);


        Debug.Log("Width: " + width + " Height: " + height + " halfWidth:");

        return vector3;
    }

    // private Zombie CreateZombie()
    // {
    //     Zombie zombie = Instantiate(_zombiePrefab);
    //     zombie._pool = _zombiePool;
    //     Debug.Log("Created Zombie");
    //     return zombie;
    // }

    // private void OnGetZombie(Zombie zombie)
    // {
    //     zombie.gameObject.SetActive(true);
    // }

    // private void OnReleaseZombie(Zombie zombie)
    // {
    //     zombie.gameObject.SetActive(false);
    // }

    // private void OnDestroyPooledZombie(Zombie zombie)
    // {
    //     throw new NotImplementedException();
    // }

    public Zombie GetZombie()
    {
        IObjectPool<IPoolable> pool = _pools[PoolID.ZOMBIE];
        IPoolable zombie = pool.Get();
        return (Zombie)zombie;
    }

    [Button, LabelText("Spawn Zombie"), ButtonGroup("Zombies")]
    public void SpawnZombie(Vector3 spawnPoint = default)
    {
        if (!IsPoolAvailable)
        {
            CreatePools();
        }
        // _zombiePool ??= new ObjectPool<Zombie>(CreateZombie, OnGetZombie, OnReleaseZombie, OnDestroyPooledZombie, _collectionCheck, _defaultCapacity, _maxSize);
        Zombie zombie = GetZombie();
        if (spawnPoint == default)
        {
            zombie.transform.position = _spawnPosition;
        }

        else
        {
            zombie.transform.position = spawnPoint;
        }

        _activeSpawns.Add(zombie);
    }
    [Button, LabelText("Spawn Generic"), ButtonGroup("Generic")]
    public void SpawnGeneric(Vector3 spawnPoint = default, PoolID id = PoolID.NULL)
    {
        if (!IsPoolAvailable)
        {
            CreatePools();
        }
        // _zombiePool ??= new ObjectPool<Zombie>(CreateZombie, OnGetZombie, OnReleaseZombie, OnDestroyPooledZombie, _collectionCheck, _defaultCapacity, _maxSize);
        IPoolable poolable = GetPoolable(id == PoolID.NULL ? _activePoolID : id);
        if (spawnPoint == default)
        {
            poolable.GameObject.transform.position = _cursorBox.transform.position;
        }

        else
        {
            poolable.GameObject.transform.position = spawnPoint;
        }

        _activeSpawns.Add(poolable);
    }



    public void UpdateCursorSize()
    {
        _cursorBox.transform.localScale = new Vector3(_spawnRadius * 2, _spawnRadius * 2, _spawnRadius * 2);
    }
    public void UpdateCursorPosition()
    {
        _cursorBox.transform.position = new Vector3(_spawnPosition.x, _spawnPosition.y, _spawnPosition.z);
    }

    [Button, LabelText("Select All Spawned By ID"), ButtonGroup("Generic")]
    public void SelectAllSpawnedByIDInInspector()
    {
        List<GameObject> poolables = new();
        foreach (IPoolable poolable in _activeSpawns)
        {
            if (poolable.PoolID == _filterByID)
            {
                poolables.Add(poolable.GameObject);
            }
        }
        Selection.objects = poolables.ToArray();
    }
    void SetSpawnPositionToPlayer()
    {
        _spawnPosition = Player.Instance.transform.position;
    }


}
