using Unity.VisualScripting;
using UnityEngine;


public class Tradeable : MonoBehaviour
{

    [SerializeField] GameObject _highlight;
    public ShopItemSO _itemData;
    public virtual void OnBuy(ShopItemSO shopItemSO)
    {
        Debug.Log("Buying " + shopItemSO._name);
    }
    public virtual void OnSell()
    {
        Debug.Log("Selling");
    }

    public void ToggleHighlight()
    {
        _highlight.SetActive(!_highlight.activeSelf);
    }
}
