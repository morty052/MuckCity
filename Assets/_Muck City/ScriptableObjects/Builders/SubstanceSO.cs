using UnityEngine;

[CreateAssetMenu(fileName = "Substance", menuName = "ScriptableObjects/Substance", order = 1)]
public class SubstanceSO : ScriptableObject
{
    public Substance _id;
    public SubstanceType _type;

    public string _name;
    public Sprite _itemImage;
    public string _itemDescription;

    public GameObject _itemPrefab;

    public int _price;

    private void OnValidate()
    {
#if UNITY_EDITOR
        _name = this.name;
        UnityEditor.EditorUtility.SetDirty(this);
#endif
    }
}
