using System;
using System.Collections.Generic;
using UnityEngine;


[Serializable]
public struct CreatureData
{
    public List<Vector3> _spawnPositions;
    public AiAgent _creaturePrefab;

    public Creature _name;

    public string _tag;


    public CreatureData(List<Vector3> spawnPositions, AiAgent creaturePrefab, Creature name, string tag = "Creature")
    {
        _spawnPositions = spawnPositions;
        _creaturePrefab = creaturePrefab;
        _name = name;
        _tag = tag;
    }
}

public class SpawnTrigger : MonoBehaviour
{
    [SerializeField] private GameObject _creatureParent;

    [SerializeField] List<CreatureData> _spawnableCreatures = new();

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            SpawnCreatures();
            Destroy(gameObject); // Destroy the trigger after spawning creatures
        }
    }


    void SpawnCreatures()
    {
        // foreach (CreatureData creatureData in _spawnableCreatures)
        // {
        //     AiAgent creature = Instantiate(creatureData._creaturePrefab, creatureData._spawnPositions, Quaternion.identity, _creatureParent.transform);
        // }

    }

}
