using System;
using System.Collections;
using Invector.vShooter;
using UnityEngine;
using UnityEngine.InputSystem;

public enum DelveBuddyFunctions
{
    RETURN_BEACON,
    SCAN_ENTITY
}

[Serializable]
public abstract class DelveBuddyFunction
{
    public DelveBuddyFunctions _id;
    public Sprite _icon;

    public bool _updateOnAim;

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

    public void HandleButtonHeldDown(InputAction inputAction)
    {
        if (inputAction.IsPressed())
        {
            _timeHeldDown += Time.deltaTime * _holdRate;
            Debug.Log("Buy button held down for " + _timeHeldDown + " frames");
            if (_timeHeldDown >= _timeToHold)
            {
                Debug.Log($"<color=cyan> Buy Triggered {_timeHeldDown} </color>");
                _timeHeldDown = 0;
            }
        }
        else if (inputAction.WasReleasedThisFrame())
        {
            _timeHeldDown = 0; // reset counter when button is released
        }
    }
}

public class SpawnReturnBeacon : DelveBuddyFunction
{
    [SerializeField] ReturnBeacon _returnBeaconPrefab;

    private ReturnBeacon _returnBeaconInstance;

    public LayerMask _groundLayer = new();

    private bool _canPlace;

    public float _raycastDistance = 100f;
    private bool _lockedInPlace = false;
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

        cam = Camera.main;
    }

    public override void Equip()
    {
        SpecialEquipmentManager.Instance._activeEquipment = _delveBuddy;
        _delveBuddy._vShooterWeapon.onShot.AddListener(HandlePlaceMent);
        _delveBuddy._vShooterWeapon._isMuted = true;
        _delveBuddy.OnToggleAim += OnToggleAim;
    }


    public override void UnEquip()
    {
        _delveBuddy._vShooterWeapon.onShot.RemoveListener(HandlePlaceMent);
        _delveBuddy.OnToggleAim -= OnToggleAim;
        _delveBuddy._vShooterWeapon._isMuted = false;
    }
    public override void Update()
    {
        HandleButtonHeldDown(_delveBuddy._fireInput);
        if (_lockedInPlace) return;
        RayCastForDrop();
    }

    public override void Use(DelveBuddy delveBuddy)
    {
        Debug.Log("Using Delve Buddy Function" + _id);
        Vector3 position = new(Player.Instance.transform.position.x, Player.Instance.transform.position.y, Player.Instance.transform.position.z - 2);
        _returnBeaconInstance.transform.position = position;
        _returnBeaconInstance.gameObject.SetActive(true);
        // _returnBeaconInstance.transform.SetParent(SpecialEquipmentManager.Instance.transform);
    }

    private void OnToggleAim(bool isAiming)
    {
        if (isAiming)
        {
            _lockedInPlace = false;
            _returnBeaconInstance.gameObject.SetActive(true);
        }
        else
        {
            if (!_lockedInPlace)
            {
                _returnBeaconInstance.gameObject.SetActive(false);
            }
        }
    }

    void RayCastForDrop()
    {
        Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f)); // Center of screen
        if (Physics.Raycast(ray, out RaycastHit hit, _raycastDistance, _groundLayer))
        {
            _canPlace = true;
            PreviewBeacon(hit.point, Quaternion.LookRotation(hit.normal));
        }

        else
        {
            _canPlace = false;
        }

    }

    void PreviewBeacon(Vector3 position, Quaternion rotation)
    {
        _returnBeaconInstance.gameObject.SetActive(true);
        // _returnBeaconInstance.transform.SetPositionAndRotation(position, Quaternion.identity);
        _returnBeaconInstance.transform.position = Vector3.Lerp(_returnBeaconInstance.transform.position, position, 0.1f);
    }

    private void HandlePlaceMent()
    {
        if (_canPlace)
        {
            _lockedInPlace = true;
            _canPlace = false;
            Player.Instance.PlayAnimation(_summonPortalAnimationName, _AnimationExitTime);
            Debug.Log("Used");
            // ToggleEquipBuddy();
            // ABUtils.DelayedInvoke(0.2f, () => ToggleEquipBuddy(true));
        }
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

        //* ACTIVATE UPDATE FUNCTION
        _updateOnAim = true;
    }


    public override void Equip()
    {
        _delveBuddy.OnInstantiateProjectileEvent += OnInstantiateProjectile;
    }

    public override void UnEquip()
    {
        _delveBuddy.OnInstantiateProjectileEvent -= OnInstantiateProjectile;
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


    private void OnInstantiateProjectile(vProjectileControl control)
    {
        // Debug.Log($"<color=cyan> Scanned Entity </color>");
        control.onCastCollider.AddListener(OnScanObject);
    }

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
