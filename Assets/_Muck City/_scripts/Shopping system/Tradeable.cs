using UnityEngine;


public class Tradeable : MonoBehaviour
{
    public virtual void OnBuy(ShopItemSO shopItemSO)
    {
        Debug.Log("Buying " + shopItemSO._name);
    }
    public virtual void OnSell()
    {
        Debug.Log("Selling");
    }
}
