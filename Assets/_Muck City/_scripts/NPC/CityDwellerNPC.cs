using Invector;
using Invector.vCharacterController;
using Invector.vCharacterController.AI;
using Invector.vItemManager;
using Invector.vShooter;
using Sirenix.OdinInspector;
using UnityEngine;

public enum GangStatus
{
    UNAFFILIATED,
    BAGGER
}

public class CityDwellerNPC : NpcCharacter
{
    CityDwellerSO _dwellerSO;
    vControlAIShooter _shooterController;

    [SerializeField] GangStatus _gang;

    vAIHeadtrack _head;

    public bool _IsUnderAttack { get; set; }

    protected override void Awake()
    {
        base.Awake();
        _shooterManager = GetComponent<vAIShooterManager>();
        _shooterController = GetComponent<vControlAIShooter>();

    }



    protected override void SetupData()
    {
        base.SetupData();
        _dwellerSO = _npcSO as CityDwellerSO;
        _head = GetComponent<vAIHeadtrack>();
        _shooterController = GetComponent<vControlAIShooter>();
        // _aiController.waypointArea = _guardNpcSO._defaultPatrolPoints;
        _shooterManager = GetComponent<vAIShooterManager>();
        _gang = _dwellerSO._gangStatus;
        _shooterController.waypointArea = _dwellerSO._waypointArea;

        LootHandler = new(_lootHandler.GetComponent<vItemCollection>());
        LootHandler.AddItemToCollection(_dwellerSO._loot);
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


    public override void Interact()
    {

        if (_canBeSearched)
        {
            GiveSearchResultItems();
        }
    }

    public void UpdateWayPoint(vWaypointArea waypoint)
    {
        _aiController.waypointArea = waypoint;
    }
}
