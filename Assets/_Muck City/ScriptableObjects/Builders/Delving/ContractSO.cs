using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using Sirenix.Utilities.Editor;
using Unity.VisualScripting;
using UnityEngine;


[CreateAssetMenu(fileName = "Contract", menuName = "ScriptableObjects/Delving/Contract", order = 1)]
public class ContractSO : DelveSO
{

    public DelveItem _delveItem;

    public Locations _keyLocation;

    public Pos _itemSpawnPos;




    public string GetCleanNameFromEnum()
    {
        string[] splitName = _tiedRealm.ToString().Split("_");
        if (splitName.Length > 1)
        {
            return string.Join(" ", splitName.Select(n => n.ToLower().FirstCharacterToUpper()));
        }
        else
        {
            return splitName[0].FirstCharacterToUpper();
        }
    }

}
