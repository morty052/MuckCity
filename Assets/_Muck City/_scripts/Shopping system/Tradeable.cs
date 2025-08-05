using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Invector.vItemManager;
using Sirenix.OdinInspector;
using Unity.VisualScripting;
using UnityEngine;


public class Tradeable : MonoBehaviour
{

    // [SerializeField] GameObject _highlight;
    [SerializeField] Material _highlightMat;
    public ShopItemSO _itemData;

    public bool _shouldHighlight = false;

    [ShowInInspector]
    public HashSet<Mesh> meshes = new();

    void Awake()
    {
        GetMeshes();
    }

    public virtual void OnBuy(ShopItemSO shopItemSO)
    {
        Debug.Log("Buying " + shopItemSO._name + " with id " + shopItemSO._itemReference.id);
        if (shopItemSO._type == ShopItemType.SPECIAL_EQUIPMENT)
        {

            SpecialEquipmentManager.Instance.AddSpecialEquipment(shopItemSO as SpecialEquipmentSO);
            return;
        }
        InventoryManager.Instance.AddItemToInventory(shopItemSO._itemReference);
    }
    public virtual void OnSell()
    {
        Debug.Log("Selling");
    }

    [Button]
    public void ToggleHighlight()
    {
        // _highlight.SetActive(!_highlight.activeSelf);
        _shouldHighlight = !_shouldHighlight;
        if (_shouldHighlight)
        {
            StartCoroutine(nameof(DrawHighLight));
        }

        else
        {
            StopCoroutine(nameof(DrawHighLight));
        }

    }

    IEnumerator DrawHighLight()
    {
        while (_shouldHighlight)
        {
            DrawPreview();
            yield return null;
        }
    }

    [Button]
    void GetMeshes()
    {

        // if (TryGetComponent(out MeshFilter mainFilter))
        // {
        //     meshes.Add(mainFilter.sharedMesh);
        // }

        // for (int i = 0; i < transform.childCount; i++)
        // {

        //     if (transform.GetChild(i).TryGetComponent(out MeshFilter filter))
        //     {
        //         meshes.Add(filter.sharedMesh);
        //         if (transform.GetChild(i))
        //         {

        //         }
        //     }
        // }

        MeshFilter[] meshFilters = gameObject.GetComponentsInChildren<MeshFilter>(true);
        foreach (MeshFilter mf in meshFilters)
        {
            Mesh mesh = mf.sharedMesh;
            // Debug.Log($"Found mesh: {mesh.name} in {mf.gameObject.name}");
            meshes.Add(mesh);
        }
    }



    void DrawPreview()
    {
        for (int i = 0; i < meshes.Count; i++)
        {
            Mesh mesh = meshes.ElementAt(i);
            // Find the MeshFilter that uses this mesh to get its transform
            MeshFilter meshFilter = GetComponentsInChildren<MeshFilter>(true)
                .FirstOrDefault(mf => mf.sharedMesh == mesh);
            Transform meshTransform = meshFilter != null ? meshFilter.transform : transform;

            Matrix4x4 matrix = Matrix4x4.TRS(meshTransform.position, meshTransform.rotation, meshTransform.localScale);
            Graphics.DrawMesh(mesh, matrix, _highlightMat, 0);
        }

    }
}
