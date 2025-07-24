using UnityEngine;

[CreateAssetMenu(fileName = "Contract", menuName = "ScriptableObjects/Delving/Contract", order = 1)]
public class ContractSO : ScriptableObject
{
    public int _bounty;
    public string _name;
    public string _description;
    public Sprite _sprite;
}
