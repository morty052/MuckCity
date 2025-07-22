using UnityEngine;

[CreateAssetMenu(fileName = "Residence Config", menuName = "ScriptableObjects/Residence Config", order = 1)]
public class ResidenceConfigSO : ScriptableObject
{
    public int _rent;
    public string _residenceName;
    public int _energyCost;
    public int _maxResidents;

    public bool _hasAmI;
}
