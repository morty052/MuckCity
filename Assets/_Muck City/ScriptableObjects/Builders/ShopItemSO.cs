using System;
using System.Collections.Generic;
using Invector.vItemManager;
using Sirenix.OdinInspector;
using UnityEngine;


public enum StatName
{
    DAMAGE,
    FIRE_RATE,
    RECOIL,
    MAGAZINE_SIZE,
    RANGE
}

public enum ShopItemType
{
    WEAPON,
    AMMO,
    APP
}

[Serializable]
public class Stat
{
    public StatName _name;
    public float _value;

    public Stat(StatName name, float value)
    {
        _name = name;
        _value = value;
    }
}

[CreateAssetMenu(fileName = "ShopItem", menuName = "ScriptableObjects/ShopItem", order = 1)]
public class ShopItemSO : ScriptableObject
{
    public string _name;
    public Sprite _icon;
    public int _price;
    public Tradeable _tradeable;

    public ItemReference _itemReference;

    public ShopItemType _type;

    [ShowIf("@_type == ShopItemType.WEAPON")]
    public List<Stat> _stats = new();

    private void OnValidate()
    {
#if UNITY_EDITOR
        _name = this.name;
        if (_type == ShopItemType.WEAPON && _stats.Count == 0)
        {
            _stats.Add(new Stat(StatName.DAMAGE, 0));
            _stats.Add(new Stat(StatName.FIRE_RATE, 0));
            _stats.Add(new Stat(StatName.RECOIL, 0));
            _stats.Add(new Stat(StatName.MAGAZINE_SIZE, 0));
            _stats.Add(new Stat(StatName.RANGE, 0));
        }
        if (_type == ShopItemType.APP && _stats.Count > 0)
        {
            _stats.Clear();
        }
        UnityEditor.EditorUtility.SetDirty(this);
#endif
    }
}
