using UnityEngine;



[CreateAssetMenu(fileName = "Bounty", menuName = "ScriptableObjects/Delving/Bounty", order = 1)]
public class BountySO : ScriptableObject
{
    public int _bounty;
    public string _name;
    public string _description;
    public Sprite _sprite;

    public Locations _lastKnownPos;
}
