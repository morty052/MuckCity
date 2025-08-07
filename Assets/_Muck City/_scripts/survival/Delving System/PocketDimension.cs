using System.Threading.Tasks;
using ImprovedTimers;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Animations;

public struct PocketDimensionData
{
    public bool _expanded;

    public bool _playerIsInPocketDimension;
    public  PocketDimensionData(bool expanded, bool playerIsInDimension)
    {
       _expanded = expanded;
       _playerIsInPocketDimension = playerIsInDimension;
    }
}

public class PocketDimension : MonoBehaviour
{
    public GameObject _pocketDimensionMiniaturePrefab;
    public GameObject _pocketDimensionMiniature;

    [SerializeField] ParentConstraint _parentConstraint;

    [SerializeField] Pos _miniaturePos;



    [SerializeField] Transform _outerStructure;

    [SerializeField] GameObject _landingAreaHelper;

    public bool _lockedInPlace = false;

    public bool _canExpand = false;
    public bool _expanded = false;

    public bool _playerIsInPocketDimension = false;


    // [SerializeField, HideInInspector] Material _previewMat;
    // [SerializeField, HideInInspector] List<Material> _childrenMat;
    // [SerializeField, HideInInspector] List<Transform> _outerChildren;

    void Awake()
    {
        _pocketDimensionMiniature = GameObject.Instantiate(_pocketDimensionMiniaturePrefab);
        ParentToMiniature();
        LoadPersistentData();
        if (_playerIsInPocketDimension)
        {
           return;   
        }
        //TODO WHEN YOU ADD SAVING REFACTOR TO ONLY SET INACTIVE IF PLYER IS NOT IN POCKET DIMENSION
        _outerStructure.gameObject.SetActive(false);

        //* DISABLE LANDING AREA HELPER
        _landingAreaHelper.SetActive(false);

        //* DISABLE POCKET DIMENSION
        gameObject.SetActive(false);
    }


    void AutoSave()
    {
        PocketDimensionData data = new(_expanded,_playerIsInPocketDimension );
        ES3.Save("POCKET_DIMENSION_DATA", data);
    }

     void LoadPersistentData()
    {
        if(!ES3.KeyExists("POCKET_DIMENSION_DATA")) return;
        PocketDimensionData data = (PocketDimensionData)ES3.Load("POCKET_DIMENSION_DATA");
        _expanded = data._expanded;
        _playerIsInPocketDimension = data._playerIsInPocketDimension;
    }

    void OnDisable()
    {
        AutoSave();
    }



    // void OnTriggerEnter(Collider other)
    // {
    //     if (!_lockedInPlace)
    //     {
    //         HudManager.Instance.UseStatusText("Exit Spawn Area", Color.red, false);
    //     }
    // }

    // void OnTriggerExit(Collider other)
    // {
    //     if (!_lockedInPlace)
    //     {
    //         HudManager.Instance.HideStatusText();
    //     }
    // }



    public void Expand()
    {
        if (_canExpand && _lockedInPlace)
        {
            _outerStructure.gameObject.SetActive(true);
            _landingAreaHelper.SetActive(false);
            _pocketDimensionMiniature.SetActive(false);
            _expanded = true;
        }

        else
        {
            Debug.Log($"Either not locked in place or cannot expand, Can Expand {_canExpand}, Locked in place {_lockedInPlace}");
        }

    }

    public void Shrink()
    {
        _lockedInPlace = false;
        _expanded = false;
        gameObject.SetActive(false);
    }

    public void ToggleExpansionWarning(bool show)
    {
        if (show)
        {
            HudManager.Instance.UseStatusText("Exit Spawn Area", Color.red, false);
            _canExpand = false;
        }

        else
        {
            HudManager.Instance.HideStatusText();
            _canExpand = true;
        }
    }
    public void TogglePlayerPresence(bool state)
    {
        _playerIsInPocketDimension = state;
        AutoSave();
    }


    public void HandlePlaceMent()
    {
        //* ENABLE POCKET DIMENSION MINIATURE
        _pocketDimensionMiniature.SetActive(true);

        //* DISABLE OUTER STRUCTURE
        _outerStructure.gameObject.SetActive(false);

        //* Enable Self
        gameObject.SetActive(true);

        //* Enable Landing Area
        _landingAreaHelper.SetActive(true);

        //* Locked in place
        _lockedInPlace = true;
        Debug.Log("Locked in place");
    }
    [Button]
    void ParentToMiniature()
    {
        if (_parentConstraint != null && _pocketDimensionMiniature != null)
        {
            //* POSITION MINIATURE EXACTLY IN THE CENTER OF THE PARENT OBJECT
            _pocketDimensionMiniature.transform.position = transform.TransformPoint(_miniaturePos.position);
            ConstraintSource newSource = new()
            {
                sourceTransform = _pocketDimensionMiniature.transform,
                weight = 1.0f
            };

            int sourceIndex = _parentConstraint.sourceCount;

            _parentConstraint.AddSource(newSource);
            _parentConstraint.SetSource(sourceIndex, newSource);

            //* Maintain the relative position and rotation of the parent object   
            _parentConstraint.SetTranslationOffset(sourceIndex, _parentConstraint.transform.position - _pocketDimensionMiniature.transform.position);
            _parentConstraint.SetRotationOffset(sourceIndex, (_parentConstraint.transform.rotation * Quaternion.Inverse(_pocketDimensionMiniature.transform.rotation)).eulerAngles);


            // Optionally, activate the constraint
            _parentConstraint.constraintActive = true;
        }
    }


    [Button]
    public virtual void DebugSafeDistance(float _safeDistance)
    {
        Player delveBuddy = GameObject.FindFirstObjectByType<Player>();
        bool IsInSafeDistanceToSpawn = Vector3.Distance(delveBuddy.transform.position, _pocketDimensionMiniature.transform.position) > _safeDistance;
        Debug.Log($"<color=orange>Is In Safe distance: {IsInSafeDistanceToSpawn}, Current distance {Vector3.Distance(delveBuddy.transform.position, _pocketDimensionMiniature.transform.position)} </color>");
    }

    // [Button]
    // void GetChildrenMat()
    // {
    //     //Get ALL Outer Children
    //     for (int i = 0; i < _outerStructureParent.childCount; i++)
    //     {
    //         _outerChildren.Add(_outerStructureParent.transform.GetChild(i));
    //         if (_outerChildren[i].childCount > 0)
    //         {
    //             for (int b = 0; b < _outerChildren[i].childCount; b++)
    //             {
    //                 _outerChildren.Add(_outerChildren[i].transform.GetChild(b));
    //             }
    //         }
    //     }
    //     for (int i = 0; i < _outerChildren.Count; i++)
    //     {
    //         _childrenMat.Add(_outerChildren[i].GetComponent<MeshRenderer>().sharedMaterial);
    //     }
    // }
    // [Button]
    // public void ToggleOuterStructMat(bool showPreview)
    // {
    //     if (_childrenMat.Count == 0)
    //     {
    //         GetChildrenMat();
    //     }
    //     if (showPreview)
    //     {
    //         for (int i = 0; i < _outerChildren.Count; i++)
    //         {
    //             _outerChildren[i].GetComponent<MeshRenderer>().material = _previewMat;
    //         }
    //     }

    //     else
    //     {
    //         for (int i = 0; i < _outerChildren.Count; i++)
    //         {
    //             _outerChildren[i].GetComponent<MeshRenderer>().material = _childrenMat[i];
    //         }
    //     }

    // }


}


