using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityUtils;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public enum DelverScreenName
{
    CONTRACTS = 0,
    BOUNTY = 1,
    DEPOSIT = 2
}

[System.Serializable]
public struct DelverScreenStruct
{
    public GameObject _obj;
    public DelverScreenName _id;
    public DelverScreenStruct(GameObject obj, DelverScreenName delverScreenName)
    {
        _obj = obj;
        _id = delverScreenName;
    }
}


public class ContractGiver : Interactable, IBrowsable
{

    public AssetLabelReference _baseBounties;
    public NpcCharacter _tiedNpc;
    [SerializeField] GameObject _ui;
    [SerializeField] GameObject _viewsParent;

    [SerializeField] List<DelverScreenStruct> _screens = new();

    List<BountySO> _bounties = new();

    bool IsPopupActive => _viewsParent.transform.Children().Any(x => x.gameObject.activeSelf);
    Transform ActivePopup => _viewsParent.transform.Children().FirstOrDefault(x => x.gameObject.activeSelf);

    void Start()
    {
        LoadAddressables();
    }

    public override void Interact()
    {
        Player.Instance.UseAltControls(true, this);
        _ui.SetActive(true);
    }

    public void ShowPopup(int parentIndex)
    {
        // GameObject obj = _screens.Find(x => x._id == parentIndex);
        GameObject obj = _screens.Find(x => x._id == (DelverScreenName)parentIndex)._obj;
        if (obj == null || obj.name == ActivePopup.gameObject.name) return;
        ABUtils.ScaleOut(ActivePopup.transform, () =>
       {
           //* DISABLE ACTIVE POP UP
           ActivePopup.gameObject.SetActive(false);
           //    obj.transform.localScale = Vector3.zero;
           //* ENABLE NEW POP UP
           obj.SetActive(true);
           //Scale in new pop up
           ABUtils.ScaleIn(obj.transform);
       });

    }

    void LoadAddressables()
    {
        // var op = FindAddressable<BountySO>("DenieSO");
        // op.Completed += OnCompleted;

        var handle = Addressables.LoadAssetsAsync<BountySO>(_baseBounties, null);
        handle.Completed += OnCompletedLoad;
    }

    void OnCompleted(AsyncOperationHandle<BountySO> op)
    {
        if (op.Result != null)
        {
            Debug.Log($"Loaded asset: {op.Result.name}");
            op.Completed -= OnCompleted;
        }
        else
        {
            Debug.LogError("Failed to load asset");
        }

    }
    void OnCompletedLoad(AsyncOperationHandle<IList<BountySO>> handle)
    {
        _bounties = (List<BountySO>)handle.Result;
        foreach (var bounty in _bounties)
        {
            Debug.Log(bounty.name);
        }
        handle.Completed -= OnCompletedLoad;
    }



    public void OnButtonPress(Inputs button)
    {
        throw new System.NotImplementedException();
    }


    public AsyncOperationHandle<T> FindAddressable<T>(string id) where T : class
    {
        return Addressables.LoadAssetAsync<T>(id);
    }

}
