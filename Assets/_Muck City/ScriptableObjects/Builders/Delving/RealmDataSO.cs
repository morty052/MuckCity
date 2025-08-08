using System.Linq;
using Eflatun.SceneReference;
using Systems.SceneManagement;
using Unity.VisualScripting;
using UnityEngine;


public enum RealmID
{
    HELLUS_CONTAMINUS = 0,
    POCKET_DIMENSION = 1
}
[CreateAssetMenu(fileName = "RealmData", menuName = "ScriptableObjects/RealmData", order = 1)]
public class RealmDataSO : ScriptableObject
{
    public TicketTier _requiredTier;
    public int _recommendedLevel;

    public RealmID _realmID;

    public string _realmDescription;

    public SceneData _sceneData;

    public string GetCleanNameFromEnum()
    {
        string[] splitName = _realmID.ToString().Split("");
        if (splitName.Length > 1)
        {
            return string.Join(" ", splitName.Select(n => n.FirstCharacterToUpper()));
        }
        else
        {
            return splitName[0].FirstCharacterToUpper();
        }
    }
}
