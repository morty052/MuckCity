using UnityEngine;
using System.Collections.Generic;
using UnityEditor;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
using Invector.vCharacterController.vActions;
using UnityEngine.UI;
using System;
using System.IO;
using Unity.VisualScripting;
using Invector.vItemManager;
using Sirenix.Utilities;
using Invector;
using System.Linq;



public class RawMaterialMaker : OdinMenuEditorWindow
{
    [ShowInInspector]
    private static vItemListData _itemListData;
    private static Sprite _placeHolderImage;
    private CreateNewCraftingItemSO _createNewCraftingItemSO;
    private CreateNewCraftingItem _createNewCraftingItem;

    [MenuItem("Jive Tools/Raw Material Maker")]
    private static void OpenWindow()
    {
        GetWindow<RawMaterialMaker>().Show();
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        if (_createNewCraftingItemSO != null)
        {
            DestroyImmediate(_createNewCraftingItemSO._newSO);
        }

        if (_createNewCraftingItem != null)
        {
            _createNewCraftingItem._itemToMakeRawMaterialContainer = null;
            _createNewCraftingItem.Unsubscribe();
        }

        // if (_createNewCraftingItem != null)
        // {
        //     DestroyImmediate(CreateNewCraftingItem._triggerPrefab);
        // }
    }
    protected override OdinMenuTree BuildMenuTree()
    {
        var tree = new OdinMenuTree();
        _createNewCraftingItemSO = new CreateNewCraftingItemSO();
        _createNewCraftingItem = new CreateNewCraftingItem();
        tree.Config.DrawSearchToolbar = true;

        tree.Add("New Crafting Item Container", _createNewCraftingItem);
        tree.Add("New Crafting Item SO", _createNewCraftingItemSO);
        tree.AddAllAssetsAtPath("Crafting Materials", "Assets/_Muck City/Prefabs/Construction/Crafting Materials", typeof(RawMaterialSO));

        SetItemListData();
        SetPlaceHolderImage();
        tree.Add("New Crafting Item ID", new EnumEditor());
        return tree;
    }

    void SetItemListData()
    {
        string path = "Assets/_Muck City/ScriptableObjects/ItemsLibrary.asset";
        vItemListData itemListData = AssetDatabase.LoadAssetAtPath<vItemListData>(path);
        _itemListData = itemListData;
    }
    void SetPlaceHolderImage()
    {
        string path = "Assets/_Muck City/Syn/InterfaceApocalypseHUD/Sprites/HUD/SPR_HUD_InactiveItem.png";
        Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(path);


        _placeHolderImage = sprite;
    }


    public class EnumEditor
    {



        public void AddItem(vItem item)
        {
            if (item.name.Contains("(Clone)"))
            {
                item.name = item.name.Replace("(Clone)", string.Empty);
            }

            if (item && !_itemListData.items.Find(it => it.name.ToClearUpper().Equals(item.name.ToClearUpper())))
            {

                AssetDatabase.AddObjectToAsset(item, AssetDatabase.GetAssetPath(_itemListData));
                item.hideFlags = HideFlags.HideInHierarchy;

                if (_itemListData.items.Exists(it => it.id.Equals(item.id)))
                    item.id = GetUniqueID(_itemListData.items);
                _itemListData.items.Add(item);
                OrderByID(ref _itemListData.items);
            }

        }

        protected virtual void OrderByID(ref List<vItem> items)
        {
            if (_itemListData != null && _itemListData.items.Count > 0)
            {
                items = items.OrderBy(i => i.id).ToList();
            }
        }

        public EnumEditor()
        {
            ReadEnumsFromScript();
        }

        [Serializable]
        public struct EnumDisplayStruct
        {

            [HideLabel]
            public string _name;

            [HideLabel]
            public int _id;

            bool _enumCanBeCreated;


            [Button("Set Active"), ShowIf("@_enumCanBeCreated == true")]
            public void SetActive()
            {
                _activeEnum = _name;
            }
            public EnumDisplayStruct(string name, int id, bool enumCanBeCreated = true)
            {
                _name = name;
                _id = id;
                _enumCanBeCreated = enumCanBeCreated;
            }
        }
        public List<EnumDisplayStruct> _enumList = new();

        [ShowInInspector, OnValueChanged("CreatePlaceHolderIdInline")]
        public static string _activeEnum;

        [HorizontalGroup("Enum Settings")]
        [OnValueChanged("CleanupNewEnumName")]
        public string _name;

        [HorizontalGroup("Enum Settings")]
        public int _id;

        [Button("Add Enum")]
        void AddEnumToList()
        {
            if (IsIdAlreadyInUse(_id) || _name == "" || NameIsInUse(_name))
            {
                Debug.LogError("ID  or name already in use or name is empty");
                return;
            }
            EnumDisplayStruct structToAdd = new(_name, CreateVItemPlaceHolderToGetID());
            _enumList.Add(structToAdd);
            _id = 0;
            _name = "";
        }

        [Button("Create Active Enum")]
        public void CreatePlaceHolderIdInline()
        {
            if (_activeEnum == " ")
            {
                Debug.LogError("name not set");
                return;
            }
            EnumDisplayStruct structToUpdate = _enumList.Find(x => x._name == _activeEnum);

            EnumDisplayStruct structToAdd = new(_activeEnum, CreateVItemPlaceHolderToGetID(true), false);

            _enumList[_enumList.IndexOf(structToUpdate)] = structToAdd;
            _activeEnum = "";
        }

        readonly string _rawMaterialsEnumScriptPath = "Assets/_Muck City/_scripts/Crafting system/RM.cs";

        public MonoScript GetMonoScriptFromAsset(string assetPath)
        {
            return AssetDatabase.LoadAssetAtPath<MonoScript>(assetPath);
        }

        // [Button("Get Enums")]
        private void ReadEnumsFromScript()
        {
            MonoScript script = GetMonoScriptFromAsset(_rawMaterialsEnumScriptPath);


            Type scriptType = script.GetClass();

            Type enumType = scriptType.GetNestedType("RawMaterials");

            Array enumValues = Enum.GetValues(enumType);

            foreach (Enum enumValue in enumValues)
            {
                int underlyingValue = (int)Enum.Parse(enumType, enumValue.ToString());
                Debug.Log(enumValue.ToString() + " = " + underlyingValue);
                _enumList.Add(new EnumDisplayStruct(enumValue.ToString(), underlyingValue, IsAlreadyInItemList(enumValue.ToString())));
            }

        }

        bool IsAlreadyInItemList(string query)
        {
            string nameToQuery = GetCleanNameFromEnum(query);
            vItem vItem = _itemListData.items.Find(x => x.name == nameToQuery);
            if (vItem != null)
            {
                return false;
            }
            return true;
        }

        bool IsIdAlreadyInUse(int id)
        {
            foreach (var item in _enumList)
            {
                if (item._id == id)
                {
                    return true;
                }
            }
            return false;
        }

        bool NameIsInUse(string name)
        {
            foreach (var item in _enumList)
            {
                if (item._name == name)
                {
                    return true;
                }
            }
            return false;
        }

        [Button("Write Enums")]
        private void WriteEnumsToScript()
        {

            MonoScript script = GetMonoScriptFromAsset(_rawMaterialsEnumScriptPath);

            string stringToWrite = "public enum RawMaterials\n{";

            string enums;
            foreach (var item in _enumList)
            {
                stringToWrite += "\n    " + item._name + " = " + item._id + ",";
            }

            stringToWrite = stringToWrite.TrimEnd(',') + "\n}";
            enums = stringToWrite;

            string enumHolderClassString = "public class EnumHolder \n {\n" + enums + "\n }";

            string newFileContent = $"{enums} \n {enumHolderClassString}";

            using StreamWriter writer = new(_rawMaterialsEnumScriptPath);

            // add string to end of file
            writer.WriteLine(newFileContent);
            AssetDatabase.Refresh();
        }

        private void CleanupNewEnumName()
        {
            _name = _name.ToUpper().Replace(" ", "_");
        }

        public string GetCleanNameFromEnum()
        {
            string name;
            string[] names = _name.ToLower().Split("_");
            if (names.Length > 1)
            {
                name = names[0].FirstCharacterToUpper() + " " + names[1].FirstCharacterToUpper();
                return name;
            }
            else
            {
                name = names[0].FirstCharacterToUpper();
                return name;
            }

        }
        public string GetCleanNameFromEnum(string name)
        {

            string[] names = name.ToLower().Split("_");
            if (names.Length > 1)
            {
                name = names[0].FirstCharacterToUpper() + " " + names[1].FirstCharacterToUpper();
                return name;
            }
            else
            {
                name = names[0].FirstCharacterToUpper();
                return name;
            }

        }

        int CreateVItemPlaceHolderToGetID(bool isInline = false)
        {

            if (vItemListWindow.Instance == null)
            {
                vItemListWindow.CreateWindow(_itemListData);
            }

            int _id = GetUniqueID(_itemListData.items);
            vItem vItem = new()
            {
                name = GetCleanNameFromEnum(isInline ? _activeEnum : _name),
                type = vItemType.CraftingMaterials,
                description = "Description has not been assigned",
                id = _id,
                icon = _placeHolderImage,
            };

            vItemAttribute vItemAttribute = new(name: Invector.vItemManager.vItemAttributes.CanCraftItemID, value: 0);
            vItem.attributes.Add(vItemAttribute);

            vItemListWindow.Instance.AddItem(vItem);
            return _id;
        }

        protected virtual int GetUniqueID(List<vItem> items, int value = 0)
        {
            var result = value;
            if (items.Count == 0)
            {
                Debug.Log("No items in list");
                return 0;
            }
            for (int i = 0; i < items.Count + 1; i++)
            {
                var item = items.Find(t => t.id == i);
                if (item == null)
                {
                    result = i;
                    break;
                }
            }

            return result;
        }

        void OpenVitemListWindow()
        {
            vItemListWindow.CreateWindow(_itemListData);

            int _id = GetUniqueID(_itemListData.items);
            vItem vItem = new()
            {
                name = GetCleanNameFromEnum(_enumList[0]._name),
                type = vItemType.CraftingMaterials,
                description = "Description has not been assigned",
                id = _id,
                icon = _placeHolderImage,
            };

            vItemAttribute vItemAttribute = new(name: Invector.vItemManager.vItemAttributes.CanCraftItemID, value: 0);
            vItem.attributes.Add(vItemAttribute);

            vItemListWindow.Instance.AddItem(vItem);
        }
    }

    public class CreateNewCraftingItemSO
    {
        public static string _rawMaterialsPath = "Assets/_Muck City/ScriptableObjects/Crafting Materials/";

        public CreateNewCraftingItemSO()
        {
            _newSO = CreateInstance<RawMaterialSO>();
            _newSO.name = "New Crafting Item";
            _newSO._disableAutoUpdateName = true;
            _newSO._itemImage = _placeHolderImage;

        }


        [OnValueChanged("UpdateRawMaterialIDAndName")]
        [LabelText("Raw Material ID")]
        public RawMaterials _id;

        [TitleGroup("Raw Material Properties")]
        [InlineEditor(objectFieldMode: InlineEditorObjectFieldModes.Hidden)]
        public RawMaterialSO _newSO;

        [Button("Create New Raw Material", ButtonSizes.Medium)]
        public void CreateNewSO()
        {
            _newSO._disableAutoUpdateName = false;
            AssetDatabase.CreateAsset(_newSO, _rawMaterialsPath + _newSO._name + ".asset");
            AssetDatabase.SaveAssets();

            _newSO = CreateInstance<RawMaterialSO>();
            _newSO.name = "New Crafting Item";
            _newSO._disableAutoUpdateName = true;
            _newSO._itemImage = _placeHolderImage;

        }

        public string GetCleanNameFromEnum()
        {

            string[] name = _id.ToString().ToLower().Split("_");

            if (name.Length > 1)
            {
                string nameToReturn = name[0].FirstCharacterToUpper() + " " + name[1].FirstCharacterToUpper();
                return nameToReturn;
            }

            else
            {
                string nameToReturn = name[0].FirstCharacterToUpper();
                return nameToReturn;
            }
        }

        void UpdateRawMaterialIDAndName()
        {

            try
            {
                _newSO._id = _id;
                string _vItemName = GetCleanNameFromEnum();

                vItem vItem = _itemListData.items.Find(t => t.name == _vItemName);

                if (vItem == null)
                {
                    throw new Exception($"query for {_vItemName} failed");
                }

                _newSO._ref.id = vItem.id;
                _newSO._name = vItem.name;
                _newSO._ref.name = vItem.name;
                _newSO._itemImage = vItem.icon;
                vItemAttribute vItemAttribute = new(name: Invector.vItemManager.vItemAttributes.CanCraftItemID, value: 0);
                _newSO._ref.attributes.Add(vItemAttribute);
            }
            catch (Exception)
            {
                throw;
            }

        }

        // [Button("Create New VItem")]
        // void CreateVItem()
        // {
        //     vItem vItem = new()
        //     {
        //         name = _newSO._name,
        //         type = vItemType.CraftingMaterials,
        //         description = _newSO._itemDescription,
        //         // attributes = _newSO._ref.attributes,
        //         id = GetUniqueID(_itemListData.items),
        //         icon = _newSO._itemImage
        //     };
        //     _newSO._ref.id = vItem.id;
        //     _newSO._ref.name = vItem.name;
        //     _itemListData.items.Add(vItem);
        // }




    }

    public class CreateNewCraftingItem
    {
        readonly string _rawMaterialsFolderPath = "Assets/_Citizen16/Prefabs/Items/";
        [LabelText("Item To Clone")]
        [ShowInInspector]
        public GameObject _itemToMakeRawMaterialContainer;
        [LabelText("Output Item")]
        [ShowInInspector]
        public static RawMaterialContainer _newItemContainer;
        // [ShowInInspector]
        public static InteractionTrigger _triggerPrefab;

        [TitleGroup("Raw Material Settings")]
        [TabGroup("Output Item Settings")]
        [OnValueChanged("UpdateMaterialNameFromID")]
        [ShowInInspector]
        public static RawMaterials _id;

        [TabGroup("Output Item Settings")]
        [OnValueChanged("UpdateMaterialName")]
        [ShowInInspector]
        public static string _name;
        [TabGroup("Output Item Settings")]
        [ShowInInspector]
        public static BoxCollider _itemCollider;

        [TabGroup("Output Item Settings")]
        [ShowInInspector]
        public static vTriggerGenericAction _trigger;


        [ShowIf("@_trigger != null")]
        [TabGroup("Adjust Collider Settings")]
        [OnValueChanged("AdjustColliderSize")]
        [ShowInInspector]
        public static Vector3 _triggerSize = Vector3.one;

        [ShowIf("@_trigger != null")]
        [TabGroup("Adjust Collider Settings")]
        [OnValueChanged("AdjustTriggerPosition")]
        [ShowInInspector]
        public static Vector3 _triggerPosition = Vector3.zero;



        [TabGroup("Output Item Settings")]
        [ShowInInspector]
        [ValueDropdown("LoadRawMaterials", IsUniqueList = true, DropdownTitle = "Select Raw Material So", DrawDropdownForListElements = false, ExcludeExistingValuesInList = true)]
        [ShowIf("@_trigger != null")]
        [OnValueChanged("UpdateRawMaterialSO")]
        public static RawMaterialSO _rawMaterialSO;


        public CreateNewCraftingItem()
        {
            LoadInteractionTriggerPrefab();
            _itemToMakeRawMaterialContainer = Selection.activeGameObject;
            Selection.selectionChanged += OnSelectionChanged;
        }

        private void OnSelectionChanged()
        {
            _itemToMakeRawMaterialContainer = Selection.activeGameObject;
        }

        [TabGroup("Output Item Settings")]
        [ShowIf("@_itemToMakeRawMaterialContainer != null && _name != null")]
        [Button("Clone Object")]
        public static void CloneObject()
        {
            GameObject selectedObject = (GameObject)Selection.activeObject;
            _newItemContainer = Instantiate(selectedObject, selectedObject.transform.position, selectedObject.transform.rotation).AddComponent<RawMaterialContainer>();


            _newItemContainer.name = _name;

            vTriggerGenericAction trigger = Instantiate(_triggerPrefab, selectedObject.transform.position, selectedObject.transform.rotation, _newItemContainer.transform).GetComponent<vTriggerGenericAction>();
            _trigger = trigger;
            _itemCollider = trigger.GetComponent<BoxCollider>();

            Debug.Log("Cloned Object: " + selectedObject.name);

            selectedObject.SetActive(false); //* Disable the original object
        }

        [TabGroup("Output Item Settings")]
        [ShowIf("@_newItemContainer != null && _newItemContainer.Data != null ")]
        [Button("Create Item")]
        public void CreateItem()
        {
            //create prefab
            PrefabUtility.SaveAsPrefabAssetAndConnect(_newItemContainer.gameObject, _rawMaterialsFolderPath + _name + ".prefab", InteractionMode.AutomatedAction);
            // AssetDatabase.CreateAsset(_newItemContainer, "Assets/_Citizen16/Resources/Items/" + _name + ".prefab");
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            _newItemContainer = null;
            _itemCollider = null;
            _id = 0;
            _name = null;
            _trigger = null;
            _itemToMakeRawMaterialContainer = null;
        }



        [TabGroup("Adjust Collider Settings")]
        [Button("Reset Collider Size")]
        public static void ResetColliderSize()
        {
            _triggerSize = Vector3.one;
        }
        public static void AdjustColliderSize()
        {
            _trigger.transform.localScale = _triggerSize;
        }
        public static void AdjustTriggerPosition()
        {
            _trigger.transform.localPosition = _triggerPosition;
        }
        public static void UpdateMaterialName()
        {
            _newItemContainer.name = _name;
        }
        public static void UpdateRawMaterialSO()
        {
            _newItemContainer.SetData(_rawMaterialSO);
        }

        // public static void AutoAssignCleanNameFromID()
        // {
        //     _rawMaterialSO._name = _rawMaterialSO._id.ToString().ToLower().Replace("_", " ").ToTitleCase();
        // }
        public static void UpdateMaterialNameFromID()
        {
            string[] name = _id.ToString().ToLower().Split("_");
            if (name.Length > 1)
            {
                _name = name[0].FirstCharacterToUpper() + " " + name[1].FirstCharacterToUpper();
            }
            else
            {
                _name = name[0].FirstCharacterToUpper();
            }
        }
        void LoadInteractionTriggerPrefab()
        {
            string path = "Assets/_Citizen16/Prefabs/Utils/Interaction Trigger.prefab";
            InteractionTrigger prefab = AssetDatabase.LoadAssetAtPath<InteractionTrigger>(path);
            _triggerPrefab = prefab;
        }



        IEnumerable<RawMaterialSO> LoadRawMaterials()
        {


            return Resources.FindObjectsOfTypeAll<RawMaterialSO>();

        }

        public void Unsubscribe()
        {
            Selection.selectionChanged -= OnSelectionChanged;
        }
    }
}


