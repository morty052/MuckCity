using System;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.VFX;

public enum DelveBuddyFunctions
{
    RETURN_BEACON = 0,
    SCAN_ENTITY = 1,
    HARVEST = 2,
    SPAWN_POCKET_DIMENSION = 3
}

[Serializable]
public abstract class DelveBuddyFunction
{
    public DelveBuddyFunctions _id;
    public Sprite _icon;

    public bool _updateOnAim;
    public bool updateAlways = false;

    protected Camera cam;
    [HideInInspector] public DelveBuddy _delveBuddy;
    public float _timeToHold = 3.5f; // Total time in seconds
    public float _holdRate = 1f;

    private float _timeHeldDown = 0;


    public abstract void Use(DelveBuddy delveBuddy);
    public abstract void Init(DelveBuddy delveBuddy);
    public abstract void Equip();
    public abstract void UnEquip();

    public abstract void Update();

    public void HandleSecondaryFire(InputAction inputAction, Action OnPressed)
    {
        if (inputAction.WasPressedThisFrame())
        {
            OnPressed?.Invoke();
        }
    }

    public void HandleButtonHeldDown(InputAction inputAction, Action OnInputDown, Action OnInputUp, Action OnComplete)
    {
        if (inputAction.IsPressed())
        {
            if (_timeHeldDown == 0)
            {
                OnInputDown?.Invoke();
            }
            _timeHeldDown += Time.deltaTime * _holdRate;
            // Debug.Log("Buy button held down for " + _timeHeldDown + " frames");
            if (_timeHeldDown >= _timeToHold)
            {
                OnComplete?.Invoke();
                _timeHeldDown = 0;
            }
        }
        else if (inputAction.WasReleasedThisFrame())
        {
            _timeHeldDown = 0; // reset counter when button is released
            OnInputUp?.Invoke();
        }
    }
}

public class SpawnReturnBeacon : DelveBuddyFunction
{
    [SerializeField] ReturnBeacon _returnBeaconPrefab;

    private ReturnBeacon _returnBeaconInstance;

    public LayerMask _groundLayer = new();

    private bool _portalPlaced = false;
    public string _summonPortalAnimationName;

    public float _AnimationExitTime;


    public override void Init(DelveBuddy delveBuddy)
    {
        if (_returnBeaconInstance == null)
        {
            _returnBeaconInstance = GameObject.Instantiate(_returnBeaconPrefab, GameManager.Instance.SpawnPoint.transform);
            _returnBeaconInstance.gameObject.SetActive(false);
        }
        _delveBuddy = delveBuddy;

        //* DEACTIVATE  UPDATE ON AIM FUNCTION
        _updateOnAim = false;

        //* ACTIVATE UPDATE ALWAYS FUNCTION
        updateAlways = true;
    }

    public override void Equip()
    {
        SpecialEquipmentManager.Instance._activeEquipment = _delveBuddy;
        // _delveBuddy._vShooterWeapon.onShot.AddListener(HandlePlaceMent);
        // _delveBuddy._vShooterWeapon._isMuted = true;
        // _delveBuddy.OnToggleAim += OnToggleAim;
    }


    public override void UnEquip()
    {
        // _delveBuddy._vShooterWeapon.onShot.RemoveListener(HandlePlaceMent);
        // _delveBuddy.OnToggleAim -= OnToggleAim;
        _delveBuddy._vShooterWeapon._isMuted = false;
    }
    public override void Update()
    {
        HandleButtonHeldDown(_delveBuddy._fireInput, OnInputDown, OnInputUp, OnComplete);
        HandleSecondaryFire(_delveBuddy._delveBuddySecondaryFire, UsePortal);
    }

    private void UsePortal()
    {
        if (!_portalPlaced) return;
        _returnBeaconInstance.ReturnToHomeRealm();
    }

    private void OnComplete()
    {
        Player.Instance.PlayAnimation(_summonPortalAnimationName, _AnimationExitTime, HandlePlaceMent);
    }

    private void OnInputUp()
    {
        Debug.Log($"<color=cyan>  Q Released </color>");
    }

    private void OnInputDown()
    {
        Debug.Log($"<color=cyan> Q Pressed </color>");
        //* SET PORTAL TO PLAYER POSITION
        _returnBeaconInstance.transform.position = Player.Instance.transform.position;
    }

    public override void Use(DelveBuddy delveBuddy)
    {
        Debug.Log("Using Delve Buddy Function" + _id);
        Vector3 position = new(Player.Instance.transform.position.x, Player.Instance.transform.position.y, Player.Instance.transform.position.z - 2);
        _returnBeaconInstance.transform.position = position;
        _returnBeaconInstance.gameObject.SetActive(true);
        // _returnBeaconInstance.transform.SetParent(SpecialEquipmentManager.Instance.transform);
    }

    // private void OnToggleAim(bool isAiming)
    // {
    //     if (isAiming)
    //     {
    //         _lockedInPlace = false;
    //         _returnBeaconInstance.gameObject.SetActive(true);
    //     }
    //     else
    //     {
    //         if (!_lockedInPlace)
    //         {
    //             _returnBeaconInstance.gameObject.SetActive(false);
    //         }
    //     }
    // }


    private void HandlePlaceMent()
    {
        _returnBeaconInstance.gameObject.SetActive(true);
        _portalPlaced = true;
    }
}
public class ScanEntity : DelveBuddyFunction
{
    public float _raycastDistance = 50;
    public float _timeToScan = 3.5f; // Total time in seconds
    public float _scanRate = 1f;
    public float _timeOnScannable = 0;
    public LayerMask _scannableLayer = new();

    public override void Init(DelveBuddy delveBuddy)
    {
        _delveBuddy = delveBuddy;
        cam = Camera.main;

        //* ACTIVATE UPDATE ON AIM FUNCTION
        _updateOnAim = true;

        //* DEACTIVATE UPDATE ALWAYS FUNCTION
        updateAlways = false;
    }

    public override void Equip()
    {
        // _delveBuddy.OnInstantiateProjectileEvent += OnInstantiateProjectile;
    }

    public override void UnEquip()
    {
        // _delveBuddy.OnInstantiateProjectileEvent -= OnInstantiateProjectile;
    }

    public override void Update()
    {
        RayCastForScannable();
    }

    public override void Use(DelveBuddy delveBuddy)
    {
        Debug.Log("Using Delve Buddy Function" + _id);

        // _returnBeaconInstance.transform.SetParent(SpecialEquipmentManager.Instance.transform);
    }


    // private void OnInstantiateProjectile(vProjectileControl control)
    // {
    //     // Debug.Log($"<color=cyan> Scanned Entity </color>");
    //     control.onCastCollider.AddListener(OnScanObject);
    // }

    void OnScanObject(RaycastHit hit)
    {
        if (hit.transform.TryGetComponent(out IScannableObject entity))
        {
            if (entity.CanScan)
            {
                _timeOnScannable += _scanRate * Time.deltaTime;
                float progress = Mathf.Clamp01(_timeOnScannable / _timeToScan);
                float fillAmount = Mathf.Lerp(0f, 1f, progress);
                Debug.Log($"progress {progress}, timeOnScannable {_timeOnScannable}, timeToScan {_timeToScan}");
                ScannedObjectUI.Instance.ProgressScan(fillAmount, hit.transform);
                if (_timeOnScannable >= _timeToScan)
                {
                    entity.OnScan();
                }
            }
        }
    }

    void RayCastForScannable()
    {
        Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f)); // Center of screen
        if (Physics.Raycast(ray, out RaycastHit hit, _raycastDistance, _scannableLayer))
        {
            OnScanObject(hit);
        }
        else
        {
            _timeOnScannable = 0;
            ScannedObjectUI.Instance.HideScanBar();
        }
    }

}
public class Harvest : DelveBuddyFunction
{
    public float _harvestDistance = 50;
    public float _harvestDuration = 3.5f; // Total time in seconds
    public float _harvestRate = 1f;
    public float _timeOnHarvestable = 0;

    public LayerMask _harvestableLayer = new();

    public VisualEffect _harvestEffect;
    VFXController _harvestEffectController;

    public override void Init(DelveBuddy delveBuddy)
    {
        _delveBuddy = delveBuddy;

        _harvestEffect = delveBuddy._harvestEffect;

        _harvestEffectController = _harvestEffect.GetComponent<VFXController>();

        // _harvestEffect.Stop();
        // _harvestEffect.pause = true;

        //* ACTIVATE UPDATE ON AIM FUNCTION
        _updateOnAim = true;

        //* DEACTIVATE UPDATE ALWAYS FUNCTION
        updateAlways = false;

        cam = Camera.main;
    }

    public override void Equip()
    {

    }

    public override void UnEquip()
    {

    }

    public override void Update()
    {
        RayCastForScannable();
    }

    public override void Use(DelveBuddy delveBuddy)
    {
        Debug.Log("Using Delve Buddy Function" + _id);
    }

    void OnScanObject(RaycastHit hit)
    {
        if (hit.transform.TryGetComponent(out IHarvestableObject entity))
        {
            if (entity.CanHarvest)
            {
                _timeOnHarvestable += _harvestRate * Time.deltaTime;
                float progress = Mathf.Clamp01(_timeOnHarvestable / _harvestDuration);
                float fillAmount = Mathf.Lerp(0f, 1f, progress);
                // Debug.Log($"progress {progress}, timeOnScannable {_timeOnScannable}, timeToScan {_harvestDuration}");
                ScannedObjectUI.Instance.ProgressScan(fillAmount, hit.transform);
                if (_timeOnHarvestable >= _harvestDuration)
                {

                    HarvestEntity(entity.GameObject.transform, () => entity.OnHarvest());
                }
            }
        }
    }

    void RayCastForScannable()
    {
        Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f)); // Center of screen
        if (Physics.Raycast(ray, out RaycastHit hit, _harvestDistance, _harvestableLayer))
        {
            OnScanObject(hit);
        }
        else
        {
            _timeOnHarvestable = 0;
            ScannedObjectUI.Instance.HideScanBar();
        }
    }

    [Button]
    public virtual void HarvestEntity(Transform entity, Action OnHarvest = null)
    {
        // if (!Application.isPlaying)
        // {
        //     _delveBuddy = GameObject.FindFirstObjectByType<DelveBuddy>();
        // }

        Mesh mesh = entity.GetComponent<MeshFilter>().sharedMesh;

        // *CHECK IF PLAYER IS AHEAD OR BEHIND ENTITY
        Vector3 dir;
        if (ABUtils.IsAhead(entity, _delveBuddy.transform))
        {
            dir = new(0, 5, 10);
        }
        else
        {
            dir = new(0, 5, 10);
        }

        //* UPDATE PARTICLE EFFECT SUCK DIRECTION
        _harvestEffect.SetVector3("Suck Direction", dir);

        //* SET EFFECT MESH TO ENTITY MESH
        _harvestEffect.SetMesh("Sampled Mesh", mesh);


        //* DISABLE ENTITY
        entity.gameObject.SetActive(false);

        //* MOVE PARTICLE EFFECT POSITION TO ENTITY POSITION
        _harvestEffect.transform.position = entity.position;

        _harvestEffect.transform.localRotation = Quaternion.identity;


        //* PLAY PARTICLE EFFECT
        _harvestEffect.Play();

        //*ENABLE PARTICLE EFFECT 
        _harvestEffect.gameObject.SetActive(true);

        ABUtils.DelayedInvoke(4f, () =>
        {
            _harvestEffectController.ResetParticle();
            OnHarvest?.Invoke();
        });

        // ABUtils.StartLerp(_harvestEffect.transform, _delveBuddy.transform, _attractionSpeed, _attractionDelay);


        // Debug.Log($"dir {dir}, IsAhead {ABUtils.IsAhead(entity, _delveBuddy.transform)}");
        // if (!Application.isPlaying)
        // {
        //     entity.gameObject.SetActive(true);
        // }
    }

}
public class SpawnPocketDimension : DelveBuddyFunction
{
    public float _spawnDistance = 50;
    public float _harvestDuration = 3.5f; // Total time in seconds
    public float _scanRate = 1f;
    public float _timeOnScannable = 0;

    public LayerMask _groundLayer = new();

    [SerializeField] PocketDimension _pocketDimensionPrefab;

    PocketDimension _pocketDimensionInstance;


    public override void Init(DelveBuddy delveBuddy)
    {
        _delveBuddy = delveBuddy;

        //* ACTIVATE UPDATE ON AIM FUNCTION
        _updateOnAim = true;

        //* DEACTIVATE UPDATE ALWAYS FUNCTION
        updateAlways = false;

        cam = Camera.main;

        if (_pocketDimensionInstance == null)
        {
            _pocketDimensionInstance = GameObject.Instantiate(_pocketDimensionPrefab);
            _pocketDimensionInstance.gameObject.SetActive(false);
        }
    }

    public override void Equip()
    {

    }

    public override void UnEquip()
    {

    }

    public override void Update()
    {
        RayCastForScannable();
    }

    public override void Use(DelveBuddy delveBuddy)
    {
        Debug.Log("Using Delve Buddy Function" + _id);
    }

    void OnScanObject(RaycastHit hit)
    {
        if (hit.transform.TryGetComponent(out IHarvestableObject entity))
        {
            if (entity.CanHarvest)
            {
                _timeOnScannable += _scanRate * Time.deltaTime;
                float progress = Mathf.Clamp01(_timeOnScannable / _harvestDuration);
                float fillAmount = Mathf.Lerp(0f, 1f, progress);
                // Debug.Log($"progress {progress}, timeOnScannable {_timeOnScannable}, timeToScan {_harvestDuration}");
                ScannedObjectUI.Instance.ProgressScan(fillAmount, hit.transform);
                if (_timeOnScannable >= _harvestDuration)
                {

                    SpawnPocketDimensionObject(entity.GameObject.transform, () => entity.OnHarvest());
                }
            }
        }
    }

    void RayCastForScannable()
    {
        Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f)); // Center of screen
        if (Physics.Raycast(ray, out RaycastHit hit, _spawnDistance, _groundLayer))
        {
            OnScanObject(hit);
        }
        else
        {

        }
    }

    [Button]
    public virtual void SpawnPocketDimensionObject(Transform entity, Action OnHarvest = null)
    {
        // if (!Application.isPlaying)
        // {
        //     _delveBuddy = GameObject.FindFirstObjectByType<DelveBuddy>();
        // }
    }

}
