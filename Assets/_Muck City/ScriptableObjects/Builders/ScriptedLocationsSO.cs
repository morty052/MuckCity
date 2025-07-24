using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

[System.Serializable]
public struct ScriptedLocation
{
    public Pos _posData;
    public Locations _name;

    public readonly string GetCleanName()
    {
        string[] splitName = _name.ToString().Split("_");
        if (splitName.Length > 1)
        {
            return string.Join(" ", splitName.Select(n => n.FirstCharacterToUpper()));
        }
        else
        {
            return splitName[0].FirstCharacterToUpper();
        }
    }
    public ScriptedLocation(Locations name, Pos pos)
    {
        _name = name;
        _posData = pos;
    }
}

[CreateAssetMenu(fileName = "ScriptedLocationsSO", menuName = "ScriptableObjects/ScriptedLocationsSO", order = 1)]
public class ScriptedLocationsSO : ScriptableObject
{
    public List<ScriptedLocation> _locations = new();

    public ScriptedLocation GetLocation(Locations locations)
    {
        return _locations.Find(x => x._name == locations);
    }
}
