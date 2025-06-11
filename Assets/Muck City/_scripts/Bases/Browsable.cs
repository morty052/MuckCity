using System.Collections;
using System.Collections.Generic;
using Unity.Cinemachine;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public abstract class Browsable : MonoBehaviour
{
    [SerializeField] protected List<Transform> _inventory = new();
    [SerializeField] protected GameObject _slotsParent;

    protected int _activeItemIndex = 0;

    void Awake()
    {
        InitGunSlots();
    }

    void InitGunSlots()
    {
        for (int i = 0; i < _slotsParent.transform.childCount; i++)
        {
            _inventory.Add(_slotsParent.transform.GetChild(i));
        }
    }

    public abstract void Exit();

    public virtual void Next()
    {
        Debug.Log("Next");
        _activeItemIndex = (_activeItemIndex + 1) % _inventory.Count;
    }

    public virtual void Prev()
    {
        Debug.Log("Prev");
        _activeItemIndex = (_activeItemIndex - 1) % _inventory.Count;
    }
    public virtual void Up()
    {
        Debug.Log("Up");
    }

    public virtual void Down()
    {
        Debug.Log("Down");
    }

    public abstract void Select();

}
