using System;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class RequirementIcon
{
    public TextMeshProUGUI requiredItemTitle;
    public TextMeshProUGUI requiredItemAmount;

    public Image _sprite;

    public int _id;

    public RequirementIcon(TextMeshProUGUI requiredItemTitle, TextMeshProUGUI requiredItemAmount, Image sprite, int id)
    {
        this.requiredItemTitle = requiredItemTitle;
        this.requiredItemAmount = requiredItemAmount;
        _sprite = sprite;
        _id = id;
    }
}
public class CraftingArea : MonoBehaviour, IInteractable
{

    public Invector.vCharacterController.GenericInput _craftItemInput = new("E", "Y", "Y");

    [SerializeField] GameObject _craftingUi;
    [SerializeField] GameObject _craftingUiPreviewTab;
    [SerializeField] TextMeshProUGUI _previewTitleText;
    [SerializeField] TextMeshProUGUI _previewDescriptionText;
    [SerializeField] Image _previewImage;
    [SerializeField] Button _craftingButton;
    [SerializeField] List<Image> _requiredItemsImageList = new();
    [SerializeField] List<RequirementIcon> _requiredItemsList = new();

    [SerializeField] Transform _itemsParent;
    [SerializeField] RecipeitemButton _recipeItemButtonPrefab;
    [SerializeField] List<RecipeSO> _discoveredRecipes = new();
    bool _canInteract = true;
    public bool CanInteract => _canInteract;

    public string InteractionPrompt => "Craft";

    Action<int> _onShopItemButtonPressed;

    int _activeItemIndex = 0;


    void OnEnable()
    {
        _onShopItemButtonPressed += OnRecipeItemButtonPressed;
    }
    void OnDisable()
    {
        _onShopItemButtonPressed -= OnRecipeItemButtonPressed;
    }

    void OnTriggerEnter(Collider other)
    {
        Player.Instance.SetInteractableObject(this);
        PrepareInteraction();
    }
    void OnTriggerExit(Collider other)
    {
        Player.Instance.SetInteractableObject(null);
        HideInteractionPrompt();
    }



    void Start()
    {
        for (int i = 0; i < _discoveredRecipes.Count; i++)
        {
            RecipeitemButton shopItem = Instantiate(_recipeItemButtonPrefab, _itemsParent);
            AddFunctionToButton(shopItem, i);
        }
        // _craftingButton.onClick.AddListener(() =>
        // {
        //     CraftItem();
        // });
        SetupRequirementIcons();
    }

    private void OnRecipeItemButtonPressed(int obj)
    {
        // Debug.Log("OnShopItemButtonPressed " + _discoveredRecipes[obj].name);
        _activeItemIndex = obj;
        PreviewCraftItem(obj);
    }
    void SetupRequirementIcons()
    {
        int index = 0;
        for (int i = 0; i < _discoveredRecipes[index]._requiredItems.Count; i++)
        {
            GameObject requiredItem = _requiredItemsImageList[i].gameObject;
            TextMeshProUGUI requiredItemTitle = requiredItem.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI requiredItemAmount = requiredItemTitle.transform.GetChild(0).GetComponent<TextMeshProUGUI>();

            _requiredItemsImageList[i].sprite = _discoveredRecipes[index]._requiredItems[i]._sprite;

            requiredItemTitle.text = _discoveredRecipes[index]._requiredItems[i].GetName();
            requiredItemAmount.text = $"x{_discoveredRecipes[index]._requiredItems[i]._idealInputAmount}";
            _requiredItemsImageList[i].gameObject.SetActive(true);

            RequirementIcon requirementIcon = new(requiredItemTitle, requiredItemAmount, _requiredItemsImageList[i], _discoveredRecipes[index]._requiredItems[i]._id);
            _requiredItemsList.Add(requirementIcon);
        }
    }

    void PreviewCraftItem(int index = -1)
    {
        if (index == -1) return;
        _previewTitleText.text = _discoveredRecipes[index]._name;
        _previewDescriptionText.text = _discoveredRecipes[index]._itemDescription;
        _previewImage.sprite = _discoveredRecipes[index]._icon;

        List<RequirementIcon> _eligibleItems = new();

        for (int i = 0; i < _discoveredRecipes[index]._requiredItems.Count; i++)
        {
            RequirementIcon requiredItemIcon = _requiredItemsList[i];
            RecipeInput requiredItem = _discoveredRecipes[index]._requiredItems[i];
            requiredItemIcon.requiredItemTitle.text = requiredItem.GetName();
            requiredItemIcon.requiredItemAmount.text = $"x{requiredItem._idealInputAmount}";
            requiredItemIcon._sprite.sprite = requiredItem._sprite;
            requiredItemIcon._sprite.gameObject.SetActive(true);

            bool hasEnoughToCraft = Player.Instance.IsItemInInventory(requiredItem._id);
            if (!hasEnoughToCraft)
            {
                requiredItemIcon._sprite.gameObject.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
                Debug.Log($" not enough {requiredItem._inputSubStance}");
            }

            else
            {
                _eligibleItems.Add(requiredItemIcon);
                requiredItemIcon._sprite.gameObject.transform.localScale = new Vector3(1f, 1f, 1f);
                Debug.Log($" player has enough {requiredItem._inputSubStance}");
            }
        }
        if (_discoveredRecipes[index]._requiredItems.Count != _eligibleItems.Count)
        {
            _craftingButton.gameObject.SetActive(false);
        }
        else
        {

            _craftingButton.gameObject.SetActive(true);
        }
        DisableUnusedBoxes(_discoveredRecipes[index]._requiredItems.Count);
    }

    void DisableUnusedBoxes(int index)
    {
        // Debug.Log($"Starting Index is {index} maz index to deactivate is {_requiredItemsList.Count}");
        for (int i = index; i < _requiredItemsList.Count; i++)
        {
            _requiredItemsList[i]._sprite.gameObject.SetActive(false);
        }
    }


    public void CraftItem()
    {

        // Craftable tradable = (Craftable)Instantiate((UnityEngine.Object)_discoveredRecipes[index]._outPutItemPrefab).GetComponent(typeof(Craftable));
        // // ITradeable tradable = Instantiate(_tradeables[index]._tradeable).GetComponent<ITradeable>();
        // tradable.OnCraft(_discoveredRecipes[index]);
        Debug.Log($"crafting {_discoveredRecipes[_activeItemIndex]._outPutItemRef.name}");
        GameEventsManager.Instance.OnCraftItem(_discoveredRecipes[_activeItemIndex]._outPutItemRef);
    }

    void AddFunctionToButton(RecipeitemButton button, int index)
    {
        Button btn = button.GetComponent<Button>();
        button.InitVisuals(_discoveredRecipes[index]);
        btn.onClick.AddListener(() =>
        {
            Debug.Log("Button clicked");
            _onShopItemButtonPressed?.Invoke(index);
        });

    }

    public void HideInteractionPrompt()
    {
        HudManager.Instance.HideInteractPrompt();
    }

    public void Interact()
    {
        _craftingUi.SetActive(true);
        Player.Instance.EnterCraftingArea(this);
    }

    public void Close()
    {
        _craftingUi.SetActive(false);
    }


    public void PrepareInteraction()
    {
        if (_craftingUi.activeSelf) return;
        HudManager.Instance.ShowInteractPrompt(InteractionPrompt);
    }


}
