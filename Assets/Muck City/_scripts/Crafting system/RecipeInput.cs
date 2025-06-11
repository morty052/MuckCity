using Unity.VisualScripting;
using UnityEngine;
[System.Serializable]
public class RecipeInput
{
    public RawMaterials _inputSubStance;
    public int _idealInputAmount;
    public RawMaterialState _idealState;
    public Sprite _sprite;

    public int _id;

    public string GetName()
    {
        return _inputSubStance.ToString().ToLower().Replace("_", " ").FirstCharacterToUpper();
    }

    public int GetIdFromRawMaterial()
    {

        return (int)_inputSubStance;
    }

    public RecipeInput(RawMaterials inputSubStance, int idealInputAmount, RawMaterialState idealState, Sprite sprite, int id)
    {
        _inputSubStance = inputSubStance;
        _idealInputAmount = idealInputAmount;
        _idealState = idealState;
        _sprite = sprite;
        _id = (int)inputSubStance;
    }
}
