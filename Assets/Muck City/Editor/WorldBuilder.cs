using UnityEngine;
using System.Collections.Generic;
using UnityEditor;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using System.Linq;
using System;
using System.Threading.Tasks;



public class WorldBuilder : OdinEditorWindow
{
    // protected override void OnDestroy()
    // {
    //     base.OnDestroy();


    //     if (_objectPainter != null)
    //     {
    //         _objectPainter.UnSub();
    //         if (_objectPainter._sceneSelection != null)
    //         {
    //             _objectPainter._sceneSelection = null;

    //         }

    //         if (_objectPainter._tables.Count > 0)
    //         {
    //             _objectPainter._tables.Clear();
    //         }

    //         if (_objectPainter._chairs.Count > 0)
    //         {
    //             _objectPainter._chairs.Clear();
    //         }

    //         if (_objectPainter._props.Count > 0)
    //         {
    //             _objectPainter._props.Clear();
    //         }

    //         if (_objectPainter._items.Count > 0)
    //         {
    //             _objectPainter._items.Clear();
    //         }

    //         if (_objectPainter._vehicles.Count > 0)
    //         {
    //             _objectPainter._vehicles.Clear();
    //         }

    //         if (_objectPainter._pallete.Count > 0)
    //         {
    //             _objectPainter._pallete.Clear();
    //         }

    //         if (_objectPainter._selections.Count > 0)
    //         {
    //             _objectPainter._selections.Clear();
    //         }
    //     }

    // }

    // protected override OdinMenuTree BuildMenuTree()
    // {
    //     var tree = new OdinMenuTree();


    //     _objectPainter = new ObjectPainter();
    //     tree.Add("Object Painter", _objectPainter);
    //     return tree;
    // }

}



public class SpawnTools
{


    private readonly string _cursorAssetPath = "Assets/_Citizen16/Prefabs/Utils/Cursor.prefab";

    [ShowInInspector]
    [VerticalGroup("Base")]
    [TabGroup("Base/Spawn Tools", "Details"), PreviewField(50, alignment: Sirenix.OdinInspector.ObjectFieldAlignment.Left), HideLabel, Tooltip("Object to paint in scene")]
    public static GameObject _objectToPaint;
    [ShowInInspector]
    [TabGroup("Base/Spawn Tools", "Details")]
    public static Vector3 _spawnPosition = Vector3.zero;


    [TabGroup("Base/Spawn Tools", "Details")]
    [VerticalGroup("Base"), HideLabel, Tooltip("Number of items to spawn")]
    public int _spawnCount = 0;

    [TabGroup("Base/Spawn Tools", "Details")]
    [VerticalGroup("Base")]
    [Button("Spawn Many")]
    public void SpawnItemsWithinRadius()
    {

        for (int i = 0; i < _spawnCount; i++)
        {
            // SpawnGeneric(GetRandomPosition());
        }
    }

    [TabGroup("Base/Spawn Tools", "Cursor")]
    [VerticalGroup("Base")]
    [ShowInInspector]
    public static BoxCollider _cursorBox;


    public void OnEnable()
    {
        if (_cursorBox != null)
        {
            return;
        }
        GameObject _cursor = AssetDatabase.LoadAssetAtPath<GameObject>(_cursorAssetPath);
        GameObject oldBox = GameObject.FindGameObjectWithTag("Cursor");
        if (oldBox == null)
        {
            _cursorBox = GameObject.Instantiate(_cursor).GetComponent<BoxCollider>();
        }
        else
        {
            _cursorBox = oldBox.GetComponent<BoxCollider>();
        }
        if (_cursorBox != null && Selection.activeGameObject != null)
        {
            _cursorBox.transform.position = Selection.activeGameObject.transform.position;

            //Make cursorbox not selectable in editor window
        }

        _cursorBox.gameObject.hideFlags = HideFlags.HideInHierarchy;
    }

    public void OnDestroy()
    {
        if (_cursorBox != null)
        {
            GameObject.DestroyImmediate(_cursorBox.gameObject);
            _cursorBox = null;
        }
    }

    [Button("Snap Cursor to selected"), TabGroup("Base/Spawn Tools", "Cursor"), VerticalGroup("Base"), ShowInInspector]
    public void SnapCursorToSelected()
    {
        if (ObjectPainter._sceneSelection != null && _cursorBox != null)
        {
            _cursorBox.transform.position = ObjectPainter._sceneSelection.transform.position;
        }

        else
        {
            Debug.Log("No object selected");
        }
    }


    [Button("Spawn Item"), TabGroup("Base/Spawn Tools", "Details"), VerticalGroup("Base")]
    public void SpawnItem()
    {
        if (_objectToPaint != null)
        {
            ObjectPainter._activeObject = PrefabUtility.InstantiatePrefab(_objectToPaint) as GameObject;
            ObjectPainter._activeObject.transform.position = _spawnPosition;
            Selection.activeGameObject = ObjectPainter._activeObject;
            ObjectPainter._position = ObjectPainter._activeObject.transform.position;

            Debug.Log("Spawn tools Spawned " + ObjectPainter._activeObject.name);
            // _selections.Add(new PaintObject(_activeObject, _activeObject.name));
        }
    }


    [TabGroup("Base/Spawn Tools", "Details")]
    [Button("Paste Spawn Position", 10), VerticalGroup("Base")]
    public void PasteSpawnPosition()
    {
        if (ObjectPainter._sceneSelection != null)
        {
            _spawnPosition = ObjectPainter._sceneSelection.transform.position;

            // _selections.Add(new PaintObject(_activeObject, _activeObject.name));
        }
    }



    // [Button("Spawn", 10), VerticalGroup("Base/Left"), BoxGroup("Base/Left/Spawn Control")]
    // public void SpawnItem()
    // {
    //     if (_objectToPaint != null)
    //     {
    //         _activeObject = PrefabUtility.InstantiatePrefab(_objectToPaint) as GameObject;
    //         _activeObject.transform.position = _spawnPosition;
    //         Selection.activeGameObject = _activeObject;
    //         _position = _activeObject.transform.position;

    //         // _selections.Add(new PaintObject(_activeObject, _activeObject.name));
    //     }
    // }

    Vector3 GetRandomPosition()
    {
        // float halfWidth = _cursorBox.size.x;
        // float halfHeight = _cursorBox.size.z;

        float width = _cursorBox.transform.lossyScale.x;
        float height = _cursorBox.transform.lossyScale.z;

        float _maxDistX = _cursorBox.transform.position.x + width / 2;
        float _minDistX = _cursorBox.transform.position.x - width / 2;
        float _maxDistZ = _cursorBox.transform.position.z - height / 2;
        float _minDistZ = _cursorBox.transform.position.z + height / 2;

        float randomX = UnityEngine.Random.Range(_minDistX, _maxDistX);
        float randomZ = UnityEngine.Random.Range(_minDistZ, _maxDistZ);

        Vector3 vector3 = new(randomX, _cursorBox.transform.position.y, randomZ);


        Debug.Log("Width: " + width + " Height: " + height + " halfWidth:");

        return vector3;
    }
}
[Serializable]

public struct WorldObject
{
    [PreviewField(60), HideLabel, HorizontalGroup("Item", 60)]
    public GameObject _item;

    [HorizontalGroup("Item"), VerticalGroup("Item/Details"), HideLabel]
    public string _name;

    [Button("Select"), VerticalGroup("Item/Details"), ButtonGroup("Item/Details/Buttons")]
    public void SelectItem()
    {
        SpawnTools._objectToPaint = _item;
    }
    [Button("Add to list"), VerticalGroup("Item/Details"), ButtonGroup("Item/Details/Buttons")]
    public void Add()
    {
        ObjectPainter._pallete.Add(this);

        string guid = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(_item));
        if (ObjectPainter._palletIds.Contains(guid))
        {
            ObjectPainter.UpdatePalletIds(guid);
        }

    }

    public readonly string GetGuid()
    {
        string guid = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(_item));
        return guid;
    }

    [Button("Rename"), VerticalGroup("Item/Details"), ButtonGroup("Item/Details/Buttons")]
    public void RenameItem()
    {
        if (_item != null)
        {
            // Find original file in project
            var originalFile = AssetDatabase.LoadAssetAtPath<GameObject>(AssetDatabase.GetAssetPath(_item));
            // Rename
            AssetDatabase.RenameAsset(AssetDatabase.GetAssetPath(originalFile), _name);

            Debug.Log("Renamed " + originalFile.name + " to " + _name);
        }
    }



    public WorldObject(GameObject item, string suffix = null)
    {
        _item = item;


        if (item != null)
        {
            _name = item.name
            .Replace("SM_Prop_", "")
            .Replace("SM_Veh_", "")
            .Replace("SM_Item_", "")
            .Replace("SM_Veh_Attach_", "");

            if (suffix != null)
            {
                _name += suffix;
                // Debug.Log(_name);
            }
        }

        else
        {
            _name = "None";
        }
    }
}


[Serializable]
public struct PaintObject
{
    [PreviewField(40, alignment: Sirenix.OdinInspector.ObjectFieldAlignment.Left), HideLabel, HorizontalGroup("Display", 40)]
    public GameObject _item;

    [HorizontalGroup("Display"), VerticalGroup("Display/Details", 60), HideLabel]
    public string _name;

    [Button("Select"), VerticalGroup("Display/Details", 60)]
    public void SelectItem()
    {
        ObjectPainter._activeObject = _item;
    }


    public PaintObject(GameObject item, string name)
    {
        _item = item;
        _name = name;
    }
}
public class ObjectPainter : OdinEditorWindow
{

    [MenuItem("Jive Tools/World Builder")]
    private static void OpenWindow()
    {
        GetWindow<ObjectPainter>().Show();
    }


    protected override void OnEnable()
    {
        Sub();

        _spawnTools = new SpawnTools();
        _spawnTools.OnEnable();

        _jvAssetLoader = new JvAssetLoader();

        _jvAssetLoader.OnEnable();

        _jVCam = new();

        SetupObject();


        if (Selection.activeGameObject != null)
        {
            _sceneSelection = Selection.activeGameObject;
        }

    }


    protected override void OnDestroy()
    {
        base.OnDestroy();
        _spawnTools.OnDestroy();
        _jvAssetLoader.OnDestroy();
        UnSub();
        if (_sceneSelection != null)
        {
            _sceneSelection = null;

        }
        if (_pallete.Count > 0)
        {
            _pallete.Clear();
        }

        if (_selections.Count > 0)
        {
            _selections.Clear();
        }

        if (_spawnTools != null)
        {
            _spawnTools = null;
        }

        if (_jVCam != null)
        {
            _jVCam = null;
        }

        if (_jvAssetLoader != null)
        {
            _jvAssetLoader = null;
        }
    }

    [HideInInspector]
    public static List<string> _palletIds = new();


    [HorizontalGroup("Base")]
    [VerticalGroup("Base/Left")]
    [BoxGroup("Base/Left/Scene Control"), ShowInInspector]
    public static GameObject _sceneSelection;

    [ShowInInspector, BoxGroup("Base/Left/Scene Control")]
    public static GameObject _activeObject;


    [VerticalGroup("Base/Right"), TabGroup("Base/Right/Assets/Tabs1", "Selections")]
    public List<PaintObject> _selections = new();

    [VerticalGroup("Base/Right")]
    [TabGroup("Base/Right/Assets/Tabs1", "Pallet")]
    [ShowInInspector]
    public static List<WorldObject> _pallete = new();

    [BoxGroup("Base/Right/Assets"), OnValueChanged("FilterItemsById")]
    public ItemNames _filter = ItemNames.ALL;


    // [BoxGroup("Base/Left/Spawn Control")]
    // [ShowInInspector]
    // public static Vector3 _spawnPosition = Vector3.zero;

    [TabGroup("Base/Left/Scene Control/Tabs", "Transform")]
    [OnValueChanged("UpdateItemPosition")]
    [ShowInInspector]
    public static Vector3 _position = Vector3.zero;

    [TabGroup("Base/Left/Scene Control/Tabs", "Transform")]
    [OnValueChanged("UpdateItemRotation")]
    public Vector3 _rotation = Vector3.zero;



    // [ShowIf("_filter", ItemNames.CHAIR), BoxGroup("Base/Right/Assets"), Searchable]
    // public List<WorldObject> _chairs = new();
    // [ShowIf("_filter", ItemNames.TABLE), BoxGroup("Base/Right/Assets"), Searchable]
    // public List<WorldObject> _tables = new();
    // [ShowIf("_filter", ItemNames.PROP), BoxGroup("Base/Right/Assets"), Searchable]
    // public List<WorldObject> _props = new();
    // [ShowIf("_filter", ItemNames.ITEM), BoxGroup("Base/Right/Assets"), Searchable]
    // public List<WorldObject> _items = new();

    // [ShowIf("_filter", ItemNames.VEHICLES), BoxGroup("Base/Right/Assets"), Searchable]
    // public List<WorldObject> _vehicles = new();
    // [ShowIf("_filter", ItemNames.CRAFTING_MATERIALS), BoxGroup("Base/Right/Assets"), LabelText("Crafting Items"), Searchable]
    // public List<WorldObject> _craftingMaterials = new();

    // [InlineEditor]
    [ShowInInspector]
    [BoxGroup("Base/Left/Spawn Control"), HideLabel]
    public static SpawnTools _spawnTools;


    [ShowInInspector]
    [BoxGroup("Base/Right/Assets"), HideLabel]
    static JvAssetLoader _jvAssetLoader;

    [ShowInInspector]

    [BoxGroup("Base/Left/Actions"), TabGroup("Base/Left/Actions/Tabs", "Cam")]
    private JVCam _jVCam;

    [Button(icon: SdfIconType.Translate, iconAlignment: IconAlignment.RightOfText), VerticalGroup("Base/Left"), ButtonGroup("Base/Left/Scene Control/Tabs/Buttons"), GUIColor(0.5f, 1f, 0.5f)]
    public void Copy()
    {
        SpawnTools._spawnPosition = _sceneSelection.transform.position;
        _position = SpawnTools._spawnPosition;
    }

    [TabGroup("Base/Left/Scene Control/Tabs", "Transform")]
    [Button(icon: SdfIconType.Brush, iconAlignment: IconAlignment.RightOfText), VerticalGroup("Base/Left"), ButtonGroup("Base/Left/Scene Control/Tabs/Buttons"), GUIColor(0.5f, 1f, 0.5f)]
    public void Paste()
    {
        if (_activeObject != null && _sceneSelection != null)
        {
            _position = _sceneSelection.transform.position;
            _activeObject.transform.position = _position;
        }
    }



    // [Button("Set spawn point from scene", 10), VerticalGroup("Base/Left"), BoxGroup("Base/Left/Spawn Control")]
    // public void PasteSpawnPosition()
    // {
    //     if (_sceneSelection != null)
    //     {
    //         _spawnPosition = _sceneSelection.transform.position;

    //         // _selections.Add(new PaintObject(_activeObject, _activeObject.name));
    //     }
    // }
    [TabGroup("Base/Left/Scene Control/Tabs", "Snapping")]
    [Button("Snap", 10), VerticalGroup("Base/Left"), ButtonGroup("Base/Left/Scene Control/Tabs/Buttons")]
    [InfoBox("@ShowSnapTargetName()")]
    public void SnapToSurface()
    {
        if (_activeObject != null && _sceneSelection != null)
        {
            if (_sceneSelection.TryGetComponent<Collider>(out var collider))
            {

                Debug.Log(collider.name);
                //Snap bottom of object to surface
                float activeObjectHeight = _sceneSelection.GetComponent<Renderer>().bounds.extents.y * 2;

                Vector3 positionToSnapTo = new()
                {
                    x = collider.transform.position.x,
                    z = collider.transform.position.z,
                    y = collider.transform.position.y + activeObjectHeight
                };
                _activeObject.transform.position = positionToSnapTo;
            }
            else
            {
                Debug.LogError("No collider found for object to snap to.");
            }
        }

        else
        {
            Debug.LogError("No object selected to snap to.");
        }
    }

    [TabGroup("Base/Left/Scene Control/Tabs", "Transform")]
    [Button("Set Active Scene Object", 10), ButtonGroup("Base/Left/Scene Control/Tabs/Buttons")]
    public void Select()
    {
        if (_sceneSelection != null)
        {
            // _selections.Add(new PaintObject(_sceneSelection, _sceneSelection.name));
            _activeObject = _sceneSelection;
        }
    }

    public string ShowSnapTargetName()
    {
        if (_sceneSelection != null)
        {
            return $"Snap to {_sceneSelection.name}";
        }

        else
        {
            return " No object selected to snap to.";
        }
    }


    [Button("Clear Pallete", 10), TabGroup("Base/Left/Actions/Tabs", "Actions"), BoxGroup("Base/Left/Actions")]
    void ClearPallete()
    {
        _palletIds.Clear();
        _pallete.Clear();
        ES3.Save("PALLET_IDS", _palletIds);
    }


    public void FilterItemsById()
    {
        _jvAssetLoader.SetFilter(_filter);
    }


    void Sub()
    {
        Selection.selectionChanged += UpdateSceneSelection;
        // SceneView.duringSceneGui += DuringSceneGui;
    }

    public void UnSub()
    {
        Selection.selectionChanged -= UpdateSceneSelection;
        // SceneView.duringSceneGui -= DuringSceneGui;
    }
    void UpdateSceneSelection()
    {
        _sceneSelection = Selection.activeGameObject;
        if (_sceneSelection != null)
        {
            _position = _sceneSelection.transform.position;
            if (_spawnTools != null && SpawnTools._cursorBox != null)
            {
                SpawnTools._cursorBox.transform.position = _position;
                JVCam.TryFocusCamera(_sceneSelection.transform);
            }
        }

    }

    public void UpdateItemPosition()
    {
        if (_activeObject != null) _activeObject.transform.position = _position;
    }
    public void UpdateItemRotation()
    {
        if (_activeObject != null) _activeObject.transform.rotation = Quaternion.Euler(_rotation);
    }


    void GetCursorPosition()
    {
        Vector3 cursorPosition = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition).origin;
        SpawnTools._spawnPosition = cursorPosition;
        Debug.Log(" mouse over scene");
    }

    // void DuringSceneGui(SceneView sceneView)
    // {
    //     GetCursorPosition();
    //     if (_activeObject != null)
    //     {
    //         Handles.color = Color.green;
    //         Handles.SphereHandleCap(0, _activeObject.transform.position, _activeObject.transform.rotation, 0.5f, EventType.Repaint);
    //     }
    // }

    void DuringSceneGui(SceneView sceneView)
    {
        Vector2 mousePosition = Event.current.mousePosition;
        Rect sceneViewRect = sceneView.position;
        if (sceneViewRect.Contains(mousePosition) && _activeObject != null)
        {
            // GetCursorPosition();
            Handles.color = Color.green;
            Handles.SphereHandleCap(0, _activeObject.transform.position, _activeObject.transform.rotation, 0.5f, EventType.Repaint);
        }
    }

    public static void UpdatePalletIds(string guid)
    {
        _palletIds.Add(guid);
        ES3.Save("PALLET_IDS", _palletIds);
    }
    async void SetupObject()
    {
        await RestorePallete();
    }
    async Task<object> RestorePallete()
    {
        await Task.Delay(1000);
        if (ES3.KeyExists("PALLET_IDS"))
        {
            List<string> _palletIds = ES3.Load<List<string>>("PALLET_IDS") ?? new List<string>();
            if (_palletIds.Count > 0)
            {
                foreach (string id in _palletIds)
                {
                    WorldObject obj = _jvAssetLoader._allItems.Find(x => x.GetGuid() == id);
                    _pallete.Add(obj);
                }
            }
        }

        return null;
    }

    public ObjectPainter()
    {

    }




}

[Serializable]
public class JvAssetLoader
{
    private readonly string _assetDataBasePath = "Assets/_Citizen16/ScriptableObjects/Asset Groups";

    [HideInInspector]
    public List<WorldObject> _allItems = new();

    public List<WorldObject> _activeItems = new();


    [HideInInspector]
    public ItemNames _filter = ItemNames.ALL;

    void LoadAssets()
    {

        string[] ids = AssetDatabase.FindAssets("t:AssetGroupInfoSO", new[] { _assetDataBasePath });
        AssetGroupInfoSO[] assetGroupInfoArray = new AssetGroupInfoSO[ids.Length];
        for (int i = 0; i < ids.Length; i++)
        {
            assetGroupInfoArray[i] = AssetDatabase.LoadAssetAtPath<AssetGroupInfoSO>(AssetDatabase.GUIDToAssetPath(ids[i]));

        }

        List<AssetGroupInfoSO> assetGroupInfoList = assetGroupInfoArray.ToList();

        // List<GameObject> assets = new();
        foreach (AssetGroupInfoSO assetGroupInfo in assetGroupInfoList)
        {
            // Debug.Log(assetGroupInfo._groupID);
            UnityEngine.Object[] assets = LoadAssetsAtPath(assetGroupInfo._groupPath);
            List<WorldObject> objectsAtPath = assets.Select(x => new WorldObject(x as GameObject, assetGroupInfo.GetSuffixName())).ToList();
            _allItems.AddRange(objectsAtPath);
        }


        FilterItemsById();

        // Debug.Log("Loaded " + _allItems.Count + " assets" + " filtered items " + _activeItems.Count);
    }

    public void OnEnable()
    {
        LoadAssets();
    }

    public void OnDestroy()
    {
        if (_allItems.Count > 0)
        {
            _allItems.Clear();
            _activeItems.Clear();
        }
    }
    public void SetFilter(ItemNames filter)
    {
        _filter = filter;
        FilterItemsById();
    }
    void FilterItemsById()
    {
        if (_filter == ItemNames.ALL)
        {
            _activeItems = _allItems;
            return;
        }
        _activeItems = _allItems.FindAll(x => x._name.Contains($"_{_filter}"));
    }
    UnityEngine.Object[] LoadAssetsAtPath(string path, string suffix = null)
    {
        string[] ids = AssetDatabase.FindAssets("t:Object", new[] { path });
        UnityEngine.Object[] assets = new UnityEngine.Object[ids.Length];
        for (int i = 0; i < ids.Length; i++)
        {
            assets[i] = AssetDatabase.LoadAssetAtPath<GameObject>(AssetDatabase.GUIDToAssetPath(ids[i]));
            // Debug.Log(ids[i]);
        }

        return assets;
    }

    public JvAssetLoader()
    {

    }
}

[Serializable]
public class JVCam
{
    public static Transform target; // assign the target object in the Inspector

    [ShowInInspector]
    public static bool _autoFocus = false;


    public static void TryFocusCamera(Transform target)
    {
        if (_autoFocus == false) return;
        // Get the current SceneView
        SceneView sceneView = SceneView.lastActiveSceneView;

        // If no SceneView is found, return
        if (sceneView == null)
        {
            Debug.LogError("No SceneView found");
        }


        // Get the camera of the SceneView
        Camera camera = sceneView.camera;

        // // Calculate the position and rotation to focus on the target
        Vector3 position = target.position;
        Quaternion rotation = Quaternion.LookRotation(target.position - camera.transform.position, Vector3.up);


        sceneView.LookAt(position, rotation, 1.5f, false, false);
    }

    [Button]
    public void FocusActiveObject()
    {
        _autoFocus = true;
        TryFocusCamera(ObjectPainter._activeObject.transform);
        _autoFocus = false;
    }

}






