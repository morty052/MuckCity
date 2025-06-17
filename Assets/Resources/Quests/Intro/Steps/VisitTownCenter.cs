
using System;
using System.Collections.Generic;
using UnityEngine;


// [Serializable]
// public struct TourObject
// {
//     public Vector3 _position;

//     public string _name;

//     public string _description;

//     public TourObject(Vector3 position, string name, string description)
//     {
//         _position = position;
//         _name = name;
//         _description = description;
//     }
// }

public class VisitTownCenter : QuestStep
{

    enum Phase
    {
        OVERRIDE,
        ESCAPE_ANNOUNCEMENT,
        ESCAPE_SEGMENT,
        MAIN_ANNOUNCEMENT,
        PREY_PREDATOR_REWARD,
    };

    [SerializeField] Phase _phase = Phase.OVERRIDE;


    [SerializeField] Vector3 _playerEventCentrePos;
    [SerializeField] Vector3 _gate;
    [SerializeField] bool _isPrey;

    [SerializeField] WeaponSO _pistol;


    void OnEnable()
    {
        Waypoint.OnMarkerReached += HandleMarkerReached;
        GameEventsManager.BranchDecisionEnded += HandleBranchDecisionEnd;
        GameEventsManager.BranchDecisionSelected += HandleBranchDecisionSelected;
    }
    void OnDisable()
    {
        Waypoint.OnMarkerReached -= HandleMarkerReached;
        GameEventsManager.BranchDecisionEnded -= HandleBranchDecisionEnd;
        GameEventsManager.BranchDecisionSelected -= HandleBranchDecisionSelected;
    }

    void Start()
    {

        Invoke(nameof(InviteToEventCentre), 1f);
    }

    private void OnAnnouncementComplete()
    {
        switch (_phase)
        {
            case Phase.OVERRIDE:
                OverridePlayerPosition();
                break;
            case Phase.ESCAPE_ANNOUNCEMENT:
                HandleEscapeBait();
                break;
            case Phase.ESCAPE_SEGMENT:
                break;
            case Phase.MAIN_ANNOUNCEMENT:
                HandlePostMainAnnouncement();
                break;
            case Phase.PREY_PREDATOR_REWARD:
                HandleStatusReward();
                break;
            default:
                break;
        }

    }

    private void HandleMarkerReached(string markerTitle)
    {
        //* MARKER INSTANTIATED AFTER PLAYER GETS TO ESCAPE POINT
        if (markerTitle == "Escape Point")
        {
            AnswerToEscape();
        }

        //* MARKER INSTANTIATED AFTER PLAYER IS DONE EXPLORING ESCAPE
        if (markerTitle == "Event Centre Spot")
        {
            StartMainAnnouncement();
        }

        if (markerTitle == "Visit Gun Shop" || markerTitle == "Visit Weapon Storage")
        {
            Debug.Log("Quest should end now");

            GameEventsManager.Instance.OnObjectiveUpdatedEvent(DomeManager.Instance._activeObjective, ObjectiveState.COMPLETED);
            FinishQuestStep();
        }
    }

    private void HandleBranchDecisionSelected(BranchDecisionManager.BranchDecisionType type)
    {
        switch (_phase)
        {
            case Phase.ESCAPE_ANNOUNCEMENT:
                if (type == BranchDecisionManager.BranchDecisionType.OPTION_ONE)
                {
                    Debug.Log("Player chose to Do nothing");
                    StartMainAnnouncement();
                }
                if (type == BranchDecisionManager.BranchDecisionType.OPTION_TWO)
                {
                    HandleEscapeDecision();
                }
                break;
            case Phase.MAIN_ANNOUNCEMENT:
                if (type == BranchDecisionManager.BranchDecisionType.OPTION_ONE)
                {
                    Debug.Log("Player chose to wait for answers");
                    IsPreyOrPredator(true);
                }
                if (type == BranchDecisionManager.BranchDecisionType.OPTION_TWO)
                {
                    Debug.Log("Player chose to fight for answers");
                    IsPreyOrPredator(false);
                }
                HandleStatusRewardAnnouncement();
                _phase = Phase.PREY_PREDATOR_REWARD;
                break;
            default:
                break;
        }
    }

    private void HandleBranchDecisionEnd()
    {
        Debug.Log("Branch decision ended");
    }

    void InviteToEventCentre()
    {
        UseClip("Report to Hall");
    }
    private void OverridePlayerPosition()
    {
        GameEventsManager.Instance.OnPlayerOverride(_playerEventCentrePos);
    }



    void BaitToEscape()
    {
        UseClip("Escape Bait");
    }

    private void HandleEscapeBait()
    {
        BranchDecision branchDecision = new("Raise your Hand if you wish to escape ", "Do nothing", "Raise Hand");
        GameEventsManager.Instance.StartBranchDecision(branchDecision);
    }

    private void HandleEscapeDecision()
    {
        Debug.Log("Player chose to Raise hand");
        DomeManager.Instance.InstantiateQuestMarker(_gate, "Escape Point");
        _phase = Phase.ESCAPE_SEGMENT;
    }

    void AnswerToEscape()
    {
        UseClip("We Are Free");
    }
    void StartMainAnnouncement()
    {
        _phase = Phase.MAIN_ANNOUNCEMENT;
        UseClip("We Are Free");
    }

    private void HandlePostMainAnnouncement()
    {
        BranchDecision branchDecision = new("Would you fight for the answers you want?", "No", "Yes");
        GameEventsManager.Instance.StartBranchDecision(branchDecision);
    }

    private void IsPreyOrPredator(bool isPrey)
    {
        if (isPrey)
        {
            Debug.Log("player status updated to prey");
            _isPrey = true;
        }
        else
        {
            Debug.Log("player status updated to predator");
            _isPrey = false;
        }
    }


    private void HandleStatusRewardAnnouncement()
    {
        UseClip("Escape Bait");
        if (_isPrey)
        {

        }

        else
        {

        }

    }
    private void HandleStatusReward()
    {
        // if (_isPrey)
        // {
        //     Debug.Log("player granted credits");
        //     GameEventsManager.Instance.OnSocialCreditUpdateEvent(100, false);
        //     Location objectiveLocation = DomeManager.Instance.GetLocationByName(Locations.GUN_SHOP, DomeManager.LocationType.SHOP);
        //     Objective visitGunShop = new("Visit Gun Shop", objectiveLocation._entrance);
        //     GameEventsManager.Instance.OnObjectiveUpdatedEvent(visitGunShop, DomeManager.ObjectiveState.STARTED);
        // }

        // else
        // {
        //     Debug.Log("player granted gun");
        //     GameEventsManager.Instance.OnAcquireWeaponEvent(_pistol);
        //     Location objectiveLocation = DomeManager.Instance.GetLocationByName(Locations.PLAYER_GUN_RACK, DomeManager.LocationType.SPECIAL);
        //     Objective visitGunStorage = new("Visit Weapon Storage", objectiveLocation._entrance);
        //     GameEventsManager.Instance.OnObjectiveUpdatedEvent(visitGunStorage, DomeManager.ObjectiveState.STARTED);
        // }
    }



}
