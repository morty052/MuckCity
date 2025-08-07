using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class Shop : Interactable, IFindable
{
    [SerializeField] ShopItemButton _shopButtonPrefab;

    [SerializeField] Transform _shopItemsParent;
    [SerializeField] List<ShopItemSO> _tradeables;

    [SerializeField] protected GameObject _shopUi;

    Action<int> _onShopItemButtonPressed;
    [SerializeField] private bool _debug;

    // bool _canInteract;

    // public bool IsHighlighted { get; }

    // bool _isQuestItem;

    // public bool IsQuestItem { get; set; }
    // public bool CanInteract => _canInteract;

    // public string InteractionPrompt => "Shop ";

    // public GameObject GameObject => gameObject;

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
        if (_debug)
        {
            Debug.Log("OnShopItemButtonPressed " + _tradeables[obj].name);
        }
        BuyItem(obj);
    }

    // void Update()
    // {
    //     if (_exitShopInput.GetButtonDown())
    //     {
    //         ExitShop();
    //     }
    // }



    public override void Start()
    {
        base.Start();
        for (int i = 0; i < _tradeables.Count; i++)
        {
            ShopItemButton shopItem = Instantiate(_shopButtonPrefab, _shopItemsParent);
            AddFunctionToButton(shopItem, i);
        }
    }
    protected void OpenShop()
    {
        _shopUi.SetActive(true);
        HideInteractionPrompt();
    }

    public void ExitShop()
    {
        Debug.Log("Player exited shop");
        _shopUi.SetActive(false);
        Player.Instance.SetInteractableObject(null);
    }

    void AddFunctionToButton(ShopItemButton button, int index)
    {
        Button btn = button.GetComponent<Button>();
        button.InitVisuals(_tradeables[index]);
        btn.onClick.AddListener(() =>
        {
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

    public override void PrepareInteraction()
    {
        if (_shopUi.activeSelf) return;
        _actionText.ShowInteractionPrompt();
        Player.Instance.SetInteractableObject(this);
    }

    public override void Interact()
    {
        OpenShop();
        Player.Instance.EnterShop(this);
        HideInteractionPrompt();
    }

    // public virtual void HideInteractionPrompt()
    // {
    //     HudManager.Instance.HideInteractPrompt();
    // }

    // public void ToggleDrawAttention()
    // {
    //     throw new NotImplementedException();
    // }

    // public void SetupInteractionListener(Action<string> action)
    // {

    // }

    // public void RemoveInteractionListener(Action<string> action)
    // {
    //     throw new NotImplementedException();
    // }
}
