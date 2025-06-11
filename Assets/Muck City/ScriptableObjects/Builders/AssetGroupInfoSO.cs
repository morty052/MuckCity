using System;
using Unity.VisualScripting;
using UnityEngine;



public enum ItemNames
{
    CHAIR,
    TABLE,
    PROP,
    ITEM,
    VEHICLES,
    CRAFTING_MATERIALS,
    ALL
}


[CreateAssetMenu(fileName = "Asset Group Info", menuName = "ScriptableObjects/Asset Group Info", order = 1)]
public class AssetGroupInfoSO : ScriptableObject
{
    readonly static string _folderPath = "Assets/_Citizen16/Prefabs/Construction/";
    public ItemNames _groupID;
    public string _groupPath;

    void OnValidate()
    {
#if UNITY_EDITOR
        _groupPath = EnumToAssetPath();
#endif
    }

    public string EnumToAssetPath()
    {
        string[] s = _groupID.ToString().Split("_");
        string cleanFolderPath;
        if (s.Length > 1)
        {
            string name = s[0].ToLower().FirstCharacterToUpper() + " " + s[1].ToLower().FirstCharacterToUpper();
            cleanFolderPath = _folderPath + name;
        }

        else
        {
            cleanFolderPath = _folderPath + _groupID.ToString().ToLower().FirstCharacterToUpper();
        }
        return cleanFolderPath;
    }

    public string GetSuffixName()
    {
        // string[] s = _groupID.ToString().Split("_");
        // string cleanFolderPath;
        // if (s.Length > 1)
        // {
        //     string name = s[0].ToLower().FirstCharacterToUpper() + " " + s[1].ToLower().FirstCharacterToUpper();
        //     cleanFolderPath = _folderPath + name;
        // }

        // else
        // {
        //     cleanFolderPath = _folderPath + _groupID.ToString().ToLower().FirstCharacterToUpper();
        // }
        return $"_{_groupID}";
    }
}
