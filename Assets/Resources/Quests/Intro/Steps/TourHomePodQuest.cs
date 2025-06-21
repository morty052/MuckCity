using System;
using System.Collections;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;


public class TourHomePodQuest : QuestStep, ILoadDataOnStart
{

    SpecialNPC _alberto;

    SpecialNPC _hazmatBill;

    TimelinePlayer _activeCutScenePlayer;


    NpcQuestData billQuestData;
    NpcQuestData _albertoQuestData;

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

        Generator gen = GetQuestItem<Generator>("Generator", true);
        gen.ToggleCanInteract();
        gen.OnInteracted += OnQuestItemInteracted;


        _doneSetup = true;
        // Alberto.UpdateQuestData(_questInfoSo, this, questData._conversationForQuest);


    }

    private void OnConversationFinished(SpecialCharacters speakerName)
    {

        switch (speakerName)
        {
            case SpecialCharacters.HAZMAT_BILL:
                billQuestData._conversationForQuest.OnDialogueFinishedEvent -= OnConversationFinished;

                ActivateMission(2);

                //* Set up quest point in elevator to complete exit bunker objective
                InstantiateQuestPoint("Exit Bunker");
                //* Set up quest point in elevator to load muck city 
                // InstantiateQuestPoint("LOAD_MUCK_CITY");
                break;
            case SpecialCharacters.ALBERTO:
                CompleteObjective("Talk To Alberto");
                InstantiateQuestPoint("Get Power Back On");
                UpdateMissionObjectives(3);
                _albertoQuestData._conversationForQuest.OnDialogueFinishedEvent -= OnConversationFinished;
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
                StartCoroutine(InstantiateQuestPointAfterDelay(1f, "Find Officers Mess"));
                break;
            case "Find Officers Mess":
                CompleteObjective(questPointName);
                UpdateMissionObjectives(2);
                break;
            case "Get Power Back On":
                //* REACTIVATE GENERATOR IN BUNKER
                Generator gen = GetQuestItem<Generator>("Generator");
                gen.ToggleCanInteract();
                break;
            default:
                break;
        }
        Debug.Log("Quest point Entered: " + questPointName + " can complete " + completesObjective);
        _activeQuestPoint.OnEnterQuestPoint -= OnEnterQuestPoint;
        _activeQuestPoint = null;
    }


    public override void OnQuestItemInteracted(string questItemTag)
    {
        switch (questItemTag)
        {
            case "Get Power Back On":
                CompleteObjective("Get Power Back On");
                UpdateMissionObjectives(3);
                break;
            default:
                break;
        }
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
        _albertoQuestData = FindNpcQuestDataByName(SpecialCharacters.ALBERTO);
        _alberto = NpcManager.Instance.GetSpecialCharacterByID(_albertoQuestData._characterID);
        _alberto.UpdateQuestData(_questInfoSo, this, _albertoQuestData._conversationForQuest);
        _alberto.OnInteractedWithQuestGiver += SendMessageToPhone;
        _albertoQuestData._conversationForQuest.OnDialogueFinishedEvent += OnConversationFinished;

        // _hazmatBill.gameObject.SetActive(false);
        // _hazmatBob.gameObject.SetActive(false);
        Debug.Log("Setup Alberto");
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




    public Task OnLoadTask()
    {

        return null;
    }

    public void AddLoadingTaskToQueue()
    {
        GameEventsManager.Instance.AddGameStartTask(this);
    }
}
