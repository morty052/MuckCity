using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class Shop : MonoBehaviour, IInteractable
{
    [SerializeField] ShopItemButton _shopButtonPrefab;

    [SerializeField] Transform _shopItemsParent;
    [SerializeField] List<ShopItemSO> _tradeables;

    [SerializeField] GameObject _shopUi;

    Action<int> _onShopItemButtonPressed;



    bool _canInteract;
    public bool CanInteract => _canInteract;

    public string InteractionPrompt => "Shop ";

    public GameObject GameObject => gameObject;

    void OnEnable()
    {
        _onShopItemButtonPressed += OnShopItemButtonPressed;
    }
    void OnDisable()
    {
        _onShopItemButtonPressed -= OnShopItemButtonPressed;
    }

    private void OnShopItemButtonPressed(int obj)
    {
        Debug.Log("OnShopItemButtonPressed " + _tradeables[obj].name);
        BuyItem(obj);
    }

    // void Update()
    // {
    //     if (_exitShopInput.GetButtonDown())
    //     {
    //         ExitShop();
    //     }
    // }



    void Start()
    {
        for (int i = 0; i < _tradeables.Count; i++)
        {
            ShopItemButton shopItem = Instantiate(_shopButtonPrefab, _shopItemsParent);
            AddFunctionToButton(shopItem, i);
        }
    }
    void OpenShop()
    {
        _shopUi.SetActive(true);
        HideInteractionPrompt();
    }

    public void ExitShop()
    {
        Debug.Log("Player exited shop");
        _shopUi.SetActive(false);
        // Player.Instance.UnlockPlayerControls();
        // HudManager.Instance.ShowInteractPrompt();
    }

    void AddFunctionToButton(ShopItemButton button, int index)
    {
        Button btn = button.GetComponent<Button>();
        button.InitVisuals(_tradeables[index]);
        btn.onClick.AddListener(() =>
        {
            Debug.Log("Button clicked");
            _onShopItemButtonPressed?.Invoke(index);
        });


    }

    void BuyItem(int index = -1)
    {
        if (index == -1) return;
        Tradeable tradable = (Tradeable)Instantiate((UnityEngine.Object)_tradeables[index]._tradeable).GetComponent(typeof(Tradeable));
        // ITradeable tradable = Instantiate(_tradeables[index]._tradeable).GetComponent<ITradeable>();
        tradable.OnBuy(_tradeables[index]);
    }

    public void PrepareInteraction()
    {
        if (_shopUi.activeSelf) return;
        HudManager.Instance.ShowInteractPrompt();
        // SetShopAsPlayerInteractable(true);
    }

    public void Interact()
    {
        OpenShop();
        Player.Instance.EnterShop(this);
        HideInteractionPrompt();
    }

    public void HideInteractionPrompt()
    {
        HudManager.Instance.HideInteractPrompt();
    }


}
