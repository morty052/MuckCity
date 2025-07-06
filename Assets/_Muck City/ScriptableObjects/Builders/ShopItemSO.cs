using Invector.vItemManager;
using UnityEngine;

[CreateAssetMenu(fileName = "ShopItem", menuName = "ScriptableObjects/ShopItem", order = 1)]
public class ShopItemSO : ScriptableObject
{
    public string _name;
    public Sprite _icon;
    public int _price;
    public Tradeable _tradeable;

    public ItemReference _itemReference;

    private void OnValidate()
    {
#if UNITY_EDITOR
        _name = this.name;
        UnityEditor.EditorUtility.SetDirty(this);
#endif
    }
}
