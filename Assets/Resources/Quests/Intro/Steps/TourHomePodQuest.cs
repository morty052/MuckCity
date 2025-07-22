using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;


public class TourHomePodQuest : QuestStep
{

    SpecialNPC _alberto;

    SpecialNPC _hazmatBill;
    SpecialNPC _hazmatBob;

    TimelinePlayer _activeCutScenePlayer;


    NpcQuestData billQuestData;
    NpcQuestData _albertoQuestData;
    NpcQuestData _bobQuestData;

    bool _doneSetup;


    void OnEnable()
    {
        GameEventsManager.OnSceneLoadEndEvent += SetupQuest;
        GameEventsManager.OnSceneLoadEndEvent += SetupAlberto;
    }

    void OnDisable()
    {
        GameEventsManager.OnSceneLoadEndEvent -= SetupQuest;
        GameEventsManager.OnSceneLoadEndEvent -= SetupAlberto;
    }

    void Start()
    {
        SetupQuest();
        // ActivateMission(1);
        // UpdateMissionObjectives(4, true);
        // InstantiateQuestPoint("Find Officers Mess");
        // StartCoroutine(PlayClipAfterDelay(2f, "Wing is Getting Detached", OnComplete: () => ShowTutorialPrompt("Basic Locomotion")));
    }

    void SetupQuest()
    {
        if (_doneSetup) return;
        // Debug.Log("Started Quest: " + _questInfoSo._id);
        billQuestData = FindNpcQuestDataByName(SpecialCharacters.HAZMAT_BILL);
        // bobQuestData = FindNpcQuestDataByName(SpecialCharacters.HAZMAT_BOB);

        _hazmatBill = NpcManager.Instance.SpawnAndMoveToPosition(billQuestData._npcSO, billQuestData._specialPosition);
        // _hazmatBob = NpcManager.Instance.SpawnAndMoveToPosition(bobQuestData._npcSO, bobQuestData._specialPosition);

        _hazmatBill.UpdateQuestData(_questInfoSo, this, billQuestData._conversationForQuest);
        // _hazmatBob.UpdateQuestData(_questInfoSo, this, bobQuestData._conversationForQuest);

        billQuestData._conversationForQuest.OnDialogueFinishedEvent += OnConversationFinished;


        _hazmatBill.gameObject.SetActive(false);

        // QuestPointData pointData = FindQuestPointDataByName("INTRO_TO_HAZMAT_BILL");
        // InstantiateQuestPoint(pointData._spawnPosition.position, pointData._name);

        var (cutSceneData, cutScene) = InstantiateCutSceneAtPoint("INTRO_TO_HAZMAT_BILL");
        _activeCutScenePlayer = cutScene;
        _activeCutScenePlayer.OnCutSceneEnded += OnCutSceneEnded;
        _activeCutScenePlayer.OnCutSceneStarted += OnCutSceneStarted;
        GetObjectFromTimeLine(_activeCutScenePlayer);

        PropertyInterface propertyInterface = GetQuestItem<PropertyInterface>("PROPERTY_INTERFACE", true);
        ItemPickUpContainer phonePickUp = GetQuestItem<ItemPickUpContainer>("Phone", true);

        propertyInterface.ToggleCanInteract(false);
        propertyInterface.PowerDownProperty(propertyInterface.PlayerLot);
        // propertyInterface.TransferPropertyToPlayer(propertyInterface.PlayerLot);



        _doneSetup = true;
    }

    private void OnConversationFinished(SpecialCharacters speakerName)
    {

        switch (speakerName)
        {
            case SpecialCharacters.HAZMAT_BILL:
                billQuestData._conversationForQuest.OnDialogueFinishedEvent -= OnConversationFinished;

                ActivateMission(1);

                //* Set up quest point in officers mess;
                StartCoroutine(InstantiateQuestPointAfterDelay(0.5f, "Find Officers Mess"));
                SetupAlberto();
                break;
            case SpecialCharacters.ALBERTO:
                CompleteObjective("Talk To Alberto");
                InstantiateQuestPoint("ENTER_MAIN_ROOM");
                UpdateMissionObjectives(3);
                _albertoQuestData._conversationForQuest.OnDialogueFinishedEvent -= OnConversationFinished;
                //* POWER UP BUNKER
                PropertyInterface propertyInterface = GetQuestItem<PropertyInterface>("PROPERTY_INTERFACE");
                propertyInterface.PowerUpProperty(propertyInterface.PlayerLot);
                break;
            default:
                break;
        }
    }

    protected override void OnEnterQuestPoint(string questPointName, bool completesObjective)
    {
        switch (questPointName)
        {
            case "Exit Bunker":
                CompleteObjective(questPointName);
                // UpdateMissionObjectives(1);
                InitBunkerHeights();
                //* SETUP QUESTPOINT TO FIGURE OUT WHEN PLAYER IS ON RIGHT TRACK TO ALBERTO
                // * AFTER ALLOWING CITY TO LOAD BY DELAYING FUNCTION
                StartCoroutine(InstantiateQuestPointAfterDelay(1f, "Find Officers Mess"));
                break;
            case "Find Officers Mess":
                CompleteObjective(questPointName);
                UpdateMissionObjectives(2);
                break;
            case "Get Power Back On":
                break;
            case "ENTER_MAIN_ROOM":
                ItemPickUpContainer phonePickUp = GetQuestItem<ItemPickUpContainer>("Phone", false);
                UseClipAtPoint("COMPAD_RINGTONE", phonePickUp.transform);
                break;
            default:
                break;
        }
        if (_debug)
        {

            Debug.Log("Quest point Entered: " + questPointName + " can complete " + completesObjective);
        }
        _activeQuestPoint.OnEnterQuestPoint -= OnEnterQuestPoint;
        _activeQuestPoint = null;
    }


    public override void OnQuestItemInteracted(string questItemTag)
    {
        switch (questItemTag)
        {
            case "Get Power Back On":
                CompleteObjective("Get Power Back On");
                Debug.Log("Player Has turned on Gen");
                UpdateMissionObjectives(4, true);
                break;
            case "Phone Pickup":
                Debug.Log("Player has entered main room");
                CompleteObjective("Search Bunker");
                DomeManager.Instance.ClearMissionDisplay();
                FinishQuestStep();
                break;
            default:
                break;
        }

        Debug.Log("Interacting with " + questItemTag);
        RemoveInteractionListener(questItemTag);
    }

    IEnumerator InstantiateQuestPointAfterDelay(float delay, string pointName)
    {
        yield return new WaitForSeconds(delay);
        InstantiateQuestPoint(pointName);
    }

    async void InitBunkerHeights()
    {
        await NpcManager.Instance.LoadNpcInArea(Locations.BUNKER_HEIGHTS);
        SetupAlberto();
        Debug.Log("Loaded Bunker Heights");
    }



    void SetupAlberto()
    {
        //* FIND LBERTO QUEST DATA
        _albertoQuestData = FindNpcQuestDataByName(SpecialCharacters.ALBERTO);

        //* SPAWN ALBERTO
        _alberto = NpcManager.Instance.SpawnAndMoveToPosition(_albertoQuestData._npcSO, _albertoQuestData._specialPosition);

        // _alberto = NpcManager.Instance.GetSpecialCharacterByID(_albertoQuestData._characterID);

        //* GIVE ALBERTO CURRENT QUEST DATA
        _alberto.UpdateQuestData(_questInfoSo, this, _albertoQuestData._conversationForQuest);


        _alberto.OnInteractedWithQuestGiver += SendMessageToPhone;

        //* SUBSCRIBE TO ALBERTO CONVERSATION
        _albertoQuestData._conversationForQuest.OnDialogueFinishedEvent += OnConversationFinished;

        Debug.Log("Setup Alberto");
    }

    void SetupBob()
    {
        _bobQuestData = FindNpcQuestDataByName(SpecialCharacters.HAZMAT_BOB);
        _hazmatBob = NpcManager.Instance.SpawnAndMoveToPosition(_bobQuestData._npcSO, _bobQuestData._specialPosition);
        _hazmatBob.UpdateQuestData(_questInfoSo, this, _bobQuestData._conversationForQuest);
        _hazmatBob.OnInteractedWithQuestGiver += SendMessageToPhone;
        _bobQuestData._conversationForQuest.OnDialogueFinishedEvent += OnConversationFinished;

        Debug.Log($"<color=orange>Setup Bob </color>");
    }


    void SendMessageToPhone()
    {
        Debug.Log("Phone Should Ring Now");
    }

    void OnCutSceneEnded(string cutSceneName)
    {
        // Debug.Log("Cut scene ended: " + cutSceneName);
        switch (cutSceneName)
        {
            case "INTRO_TO_HAZMAT_BILL":
                _hazmatBill.gameObject.SetActive(true);
                // _hazmatBill.StartConversation(_hazmatBill.ActiveConversation);
                break;
            default:
                break;
        }
        _activeCutScenePlayer.OnCutSceneStarted -= OnCutSceneEnded;
        _activeCutScenePlayer = null;
    }
    void OnCutSceneStarted(string cutSceneName)
    {
        // Debug.Log("Cut scene started: " + cutSceneName);
        switch (cutSceneName)
        {
            case "INTRO_TO_HAZMAT_BILL":
                _hazmatBill.gameObject.SetActive(false);
                // _hazmatBill.StartConversation(_hazmatBill.ActiveConversation);
                break;
            default:
                break;
        }
        _activeCutScenePlayer.OnCutSceneStarted -= OnCutSceneStarted;
    }



}
