using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class StatDisplay : MonoBehaviour
{
    public List<StatDisplayItem> _stats = new();
    public GameObject _itemStatPrefab;

    public void SetStatsList(List<Stat> stats)
    {
        for (int i = 0; i < stats.Count; i++)
        {
            GameObject statObject = Instantiate(_itemStatPrefab, transform.position, Quaternion.identity, transform);
            StatDisplayItem statItem = new(statObject, stats[i]);
            statObject.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
            _stats.Add(statItem);
        }
    }

    public void CreateStatList(ShopItemType type)
    {
        List<Stat> stats = new();
        // Debug.Log($"<color=green> Creating {type} Stats List </color>");
        if (type == ShopItemType.WEAPON)
        {
            stats.Add(new Stat(StatName.DAMAGE, 0));
            stats.Add(new Stat(StatName.FIRE_RATE, 0));
            stats.Add(new Stat(StatName.RECOIL, 0));
            stats.Add(new Stat(StatName.MAGAZINE_SIZE, 0));
            stats.Add(new Stat(StatName.RANGE, 0));
        }
        SetStatsList(stats);
    }

    public void UpdateStatValues(ShopItemSO shopItemSO)
    {
        List<Stat> stats = shopItemSO._stats;
        for (int i = 0; i < _stats.Count; i++)
        {
            _stats[i].UpdateStatValues(stats[i]);
            // Debug.Log(_stats[i]._statImageBar.fillAmount);
        }
    }
}

[Serializable]
public struct StatDisplayItem
{
    public TextMeshProUGUI _statNameText;
    public Image _statImageBar;

    public Stat _stat;

    public StatDisplayItem(GameObject itemStatPrefab, Stat stat)
    {
        _statNameText = itemStatPrefab.transform.Find("StatName").GetComponent<TextMeshProUGUI>();
        _statImageBar = itemStatPrefab.transform.Find("StatBar").GetComponent<Image>();
        _stat = stat;
        UpdateStatValues(stat);
    }

    public readonly void UpdateStatValues(Stat stat)
    {
        _statNameText.text = stat._name.ToString();
        _statImageBar.fillAmount = stat._value / 100;
    }
}

