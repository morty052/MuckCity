using System;
using System.Collections.Generic;
using UnityEngine;

public enum EffectName
{
    InitWayPointEffect,
    DecreasePriceEffect,
    ReduceTrust,
    IncreaseTrust
}

[CreateAssetMenu(fileName = "DecisionData", menuName = "ScriptableObjects/DecisionData")]
public class DecisionData : ScriptableObject
{
    public string _label;
    [SerializeReference] public List<DecisionEffect> _effects;

    public T GetEffect<T>(EffectName effectName) where T : DecisionEffect
    {
        DecisionEffect effect = _effects.Find(x => x.GetType().ToString() == effectName.ToString());

        return (T)effect;
    }

    void OnEnable()
    {
        if (string.IsNullOrEmpty(_label)) _label = name;
        _effects ??= new();
    }
}



[Serializable]
public abstract class DecisionEffect
{
    public abstract void Execute();
}

[Serializable]
public class InitWayPointEffect : DecisionEffect
{
    public Pos _waypoint;

    public override void Execute()
    {
        Debug.Log("Initializing Waypoint");
    }
}
[Serializable]
public class DecreasePriceEffect : DecisionEffect
{

    public override void Execute()
    {
        Debug.Log("Decreasing price");
    }
}
