using Invector.vItemManager;
using Sirenix.OdinInspector;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "Raw Material", menuName = "ScriptableObjects/Raw Material", order = 1)]
public class RawMaterialSO : ScriptableObject
{
    [OnValueChanged("AutoAssignCleanNameFromID")]
    public RawMaterials _id;

    public RawMaterialState _defaultState;
    public string _name;
    public Sprite _itemImage;
    public string _itemDescription;

    public ItemReference _ref;

    public bool _disableAutoUpdateName = false;
    public PoolID _poolID;

    private void OnValidate()
    {
#if UNITY_EDITOR
        if (_disableAutoUpdateName) return;
        _name = this.name;
        UnityEditor.EditorUtility.SetDirty(this);
#endif
    }

    [Button]
    public void SetReferences()
    {
#if UNITY_EDITOR
        Debug.Log("Set Item ID");
        _ref.id = GetIdFromRawMaterial();
        _ref.name = _name;
        UnityEditor.EditorUtility.SetDirty(this);
#endif
    }



    public int GetIdFromRawMaterial()
    {

        return (int)_id;
    }

    private void AutoAssignCleanNameFromID()
    {
        if (!_disableAutoUpdateName) return;
        _name = _id.ToString().ToLower().Replace("_", " ").FirstCharacterToUpper();

    }


}
