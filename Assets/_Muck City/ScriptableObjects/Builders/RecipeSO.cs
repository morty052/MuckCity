using System.Collections.Generic;
using Invector.vItemManager;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "Recipe", menuName = "ScriptableObjects/Recipe", order = 1)]
public class RecipeSO : ScriptableObject
{
    public Substance _outputSubstance;

    public List<RecipeInput> _requiredItems;
    public string _name;
    public Sprite _icon;

    [TextArea]
    public string _itemDescription;

    public ItemReference _outPutItemRef;



    //     private void OnValidate()
    //     {
    // #if UNITY_EDITOR
    //         _name = this.name;
    //         UnityEditor.EditorUtility.SetDirty(this);
    //         foreach (RecipeInput input in _requiredItems)
    //         {
    //             input._sprite = Resources.Load<Sprite>(input._inputSubStance._name);
    //         }
    // #endif
    //     }

    private void OnValidate()
    {
#if UNITY_EDITOR
        _name = this.name;
        UnityEditor.EditorUtility.SetDirty(this);
        // for (int i = 0; i < _requiredItems.Count; i++)
        // {
        //     RecipeInput input = _requiredItems[i];
        //     input._sprite = Resources.Load<Sprite>(input._inputSubStance.ToString());
        // }
#endif
    }


    [Button]
    public void SetReferences()
    {
#if UNITY_EDITOR
        Debug.Log("SetImages");
        for (int i = 0; i < _requiredItems.Count; i++)
        {
            RecipeInput input = _requiredItems[i];
            input._sprite = Resources.Load<Sprite>("Icons/" + input._inputSubStance.ToString());
            input._id = input.GetIdFromRawMaterial();
        }

        _outPutItemRef.id = GetIdFromSubstance();
        _outPutItemRef.name = _name;
        _outPutItemRef.amount = 1;

        UnityEditor.EditorUtility.SetDirty(this);
#endif
    }


    public int GetIdFromSubstance()
    {

        return (int)_outputSubstance;
    }

}
