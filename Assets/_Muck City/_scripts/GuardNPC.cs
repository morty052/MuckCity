using Invector;
using Invector.vCharacterController;
using Invector.vCharacterController.AI;
using Invector.vShooter;
using Sirenix.OdinInspector;
using UnityEngine;

public class GuardNPC : NpcCharacter
{
    GuardNpcSO _guardNpcSO;
    vControlAIShooter _shooterController;
    vAIShooterManager _shooterManager;
    vAIHeadtrack _head;

    public GameObject _weaponHolder;
    public vShooterWeapon _defaultWeaponPrefab;

    GameObject _activeWeapon;

    public bool _IsUnderAttack { get; set; }



    protected override void SetupData()
    {
        base.SetupData();
        _guardNpcSO = _npcSO as GuardNpcSO;
        _head = GetComponent<vAIHeadtrack>();
        _shooterController = GetComponent<vControlAIShooter>();
        // _aiController.waypointArea = _guardNpcSO._defaultPatrolPoints;
        _shooterManager = GetComponent<vAIShooterManager>();
    }


    [Button("Hide Weapon")]
    void HideWeapon()
    {
        Destroy(_activeWeapon);
    }
    [Button("Look At Player")]
    void LookAtPlayer()
    {
        _head.LookAtTarget(Player.Instance.transform);
    }

    [Button("Equip Weapon")]
    void EquipWeapon()
    {
        GameObject w = Instantiate(_defaultWeaponPrefab.gameObject, _weaponHolder.transform);
        w.transform.localPosition = Vector3.zero;
        _activeWeapon = w.transform.parent.gameObject;
        _shooterManager.SetRightWeapon(w);
    }

    public void UpdateWayPoint(vWaypointArea waypoint)
    {
        _aiController.waypointArea = waypoint;
    }
}
