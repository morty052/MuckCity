using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.Events;

namespace DynamicEnums
{
    // ✅ Matches the "AllowedEnums" list in ChildrenItemFinder
    public enum PodItemName
    {
        PocketDoor,
        PodScreen,
        PodChair,
        PodConsole
    }

    // Another enum that could also be allowed
    public enum WeaponType
    {
        Laser,
        PlasmaRifle,
        RocketLauncher
    }

    // Another allowed enum
    public enum DoorType
    {
        MainDoor,
        SideHatch,
        MaintenancePanel
    }

    // An enum that is NOT in the allowed list
    // (will not appear in the dropdown)
    public enum FruitType
    {
        Apple,
        Banana,
        Mango
    }
}

public class ChildrenItemFinder : MonoBehaviour
{
    // ----------------------------
    // CONFIG: change these values
    // ----------------------------
    [Tooltip("FullName (Namespace.TypeName) of enums that should appear in the dropdown.")]
    [SerializeField]
    private string[] AllowedEnums =
    {
        "DynamicEnums.PodItemName",
        "DynamicEnums.WeaponType",
        "DynamicEnums.DoorType"
    };

    [ValueDropdown(nameof(GetEnumTypeNames))]
    [OnValueChanged(nameof(OnEnumTypeChanged))]
    [SerializeField]
    private string enumTypeName;

    [Tooltip("If true, onItemsFound will only be invoked when every enum value has a matching child.")]
    [SerializeField]
    private bool requireAllFound = false;
    [SerializeField]
    private bool _searchOnAwake = false;

    [FoldoutGroup("Events")]
    public UnityEvent OnSearchComplete;

    // ----------------------------
    // Internal state
    // ----------------------------
    [ShowInInspector, ReadOnly]
    private Dictionary<string, Transform> foundItems = new Dictionary<string, Transform>();

    // normalized name -> enumName (un-normalized, as string)
    private Dictionary<string, string> enumLookup = new Dictionary<string, string>();

    private Type enumType;

    // ----------------------------
    // Unity lifecycle
    // ----------------------------
    private void Awake()
    {
        InitializeEnumType();
        if (_searchOnAwake)
        {
            SearchChildrenIterative(transform);
        }
    }

    // #if UNITY_EDITOR
    //     private void OnValidate()
    //     {
    //         InitializeEnumType();
    //     }
    // #endif

    // ----------------------------
    // Public API
    // ----------------------------
    [Button("Search Children")]
    public void SearchChildrenIterative(Transform root)
    {
        if (enumType == null)
        {
            Debug.LogError($"[ChildrenItemFinder] Enum type not set or invalid ('{enumTypeName}').");
            return;
        }

        foundItems.Clear();

        var stack = new Stack<Transform>();
        stack.Push(root);

        while (stack.Count > 0)
        {
            var current = stack.Pop();
            string cleanName = NormalizeName(current.name);

            if (enumLookup.TryGetValue(cleanName, out string enumName))
            {
                if (!foundItems.ContainsKey(enumName))
                {
                    foundItems.Add(enumName, current);
                    Debug.Log($"[ChildrenItemFinder] ✅ Found and stored: {enumName} at path {GetFullPath(current)}");
                }
                else
                {
                    Debug.LogWarning($"[ChildrenItemFinder] ⚠ Duplicate found for {enumName} at {GetFullPath(current)} — already mapped to {GetFullPath(foundItems[enumName])}");
                }
            }

            foreach (Transform child in current)
                stack.Push(child);
        }

        if (!requireAllFound)
        {
            OnSearchComplete?.Invoke();
        }
        else
        {
            var missing = GetMissingItems();
            if (missing.Count == 0)
            {
                OnSearchComplete?.Invoke();
            }
            else
            {
                Debug.LogWarning($"[ChildrenItemFinder] Not all enum values were found. Missing: {string.Join(", ", missing)}");
            }
        }
    }

    public void SetEnumType<TEnum>(bool searchNow = false) where TEnum : Enum
    {
        string fullName = typeof(TEnum).FullName;
        if (!AllowedEnums.Contains(fullName))
        {
            Debug.LogError($"[ChildrenItemFinder] Enum '{fullName}' is not in AllowedEnums. Add it or use an allowed enum.");
            return;
        }

        enumTypeName = fullName;
        InitializeEnumType();

        Debug.Log($"[ChildrenItemFinder] Enum type changed via script to: {enumTypeName}");
        if (searchNow)
            SearchChildrenIterative(transform);
    }

    public void SetEnumTypeByName(string fullEnumName, bool searchNow = false)
    {
        if (!AllowedEnums.Contains(fullEnumName))
        {
            Debug.LogError($"[ChildrenItemFinder] Enum '{fullEnumName}' is not in AllowedEnums.");
            return;
        }

        enumTypeName = fullEnumName;
        InitializeEnumType();

        Debug.Log($"[ChildrenItemFinder] Enum type changed via name to: {enumTypeName}");
        if (searchNow)
            SearchChildrenIterative(transform);
    }

    public Transform GetItem<TEnum>(TEnum value) where TEnum : Enum
    {
        string requestedEnumFullName = typeof(TEnum).FullName;
        if (enumType != null && enumType.FullName != requestedEnumFullName)
        {
            Debug.LogWarning($"[ChildrenItemFinder] Requested enum type '{requestedEnumFullName}' does not match current selected enum '{enumType?.FullName}'.");
        }

        string key = value.ToString();
        return foundItems.TryGetValue(key, out Transform t) ? t : null;
    }

    public Transform GetItemByName(string enumValueName)
    {
        return foundItems.TryGetValue(enumValueName, out Transform t) ? t : null;
    }

    public List<string> GetMissingItems()
    {
        if (enumType == null) return new List<string>();

        var allNames = Enum.GetNames(enumType);
        return allNames.Where(n => !foundItems.ContainsKey(n)).ToList();
    }

    public List<string> GetAllFoundNames()
    {
        return foundItems.Keys.ToList();
    }

    // ----------------------------
    // Internal helpers
    // ----------------------------
    private void InitializeEnumType()
    {
        if (string.IsNullOrEmpty(enumTypeName))
        {
            enumType = null;
            enumLookup.Clear();
            return;
        }

        enumType = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(a =>
            {
                try { return a.GetTypes(); }
                catch { return Type.EmptyTypes; }
            })
            .FirstOrDefault(t => t.IsEnum && t.FullName == enumTypeName);

        if (enumType == null)
        {
            Debug.LogError($"[ChildrenItemFinder] Could not find enum type '{enumTypeName}'. Make sure it's in AllowedEnums.");
            enumLookup.Clear();
            return;
        }

        enumLookup.Clear();
        foreach (var val in Enum.GetValues(enumType))
        {
            string enumName = val.ToString();
            string normalized = NormalizeName(enumName);
            if (!enumLookup.ContainsKey(normalized))
                enumLookup.Add(normalized, enumName);
            else
                Debug.LogWarning($"[ChildrenItemFinder] Normalized enum name collision for '{enumName}' (normalized '{normalized}').");
        }
    }

    private void OnEnumTypeChanged()
    {
        InitializeEnumType();
        Debug.Log($"[ChildrenItemFinder] Enum type changed in inspector to: {enumTypeName}");
    }

    private static string NormalizeName(string name)
    {
        return new string(name.Where(c => !char.IsWhiteSpace(c) && c != '_').ToArray()).ToLowerInvariant();
    }

    private string GetFullPath(Transform obj)
    {
        return obj == null ? string.Empty : (obj.parent == null ? obj.name : GetFullPath(obj.parent) + "/" + obj.name);
    }

    private IEnumerable<string> GetEnumTypeNames()
    {
        var existing = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(a =>
            {
                try { return a.GetTypes(); }
                catch { return Type.EmptyTypes; }
            })
            .Where(t => t.IsEnum)
            .Select(t => t.FullName)
            .ToHashSet();

        return AllowedEnums.Where(n => existing.Contains(n)).OrderBy(n => n);
    }
}

