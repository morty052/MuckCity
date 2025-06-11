using UnityEngine;

public class Craftable : Tradeable
{
    [SerializeField] CraftableItemTypes type;
    // Start is called once before the first execution of Update after the MonoBehaviour is created


    // Update is called once per frame
    public void OnCraft(RecipeSO recipeSO)
    {

    }
}
