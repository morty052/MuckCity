using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using DialogueEditor;
using UnityEngine;



public class TourHomePodQuest : QuestStep, ILoadDataOnStart
{

    SpecialNPC _alberto;

    SpecialNPC _hazmatBill;
    SpecialNPC _hazmatBob;

    TimelinePlayer _activeCutScenePlayer;

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
        // StartCoroutine(PlayClipAfterDelay(2f, "Wing is Getting Detached", OnComplete: () => ShowTutorialPrompt("Basic Locomotion")));
    }

    void SetupQuest()
    {
        if (_doneSetup) return;
        Debug.Log("Started Quest: " + _questInfoSo._id);
        NpcQuestData billQuestData = FindNpcQuestDataByName(SpecialCharacters.HAZMAT_BILL);
        NpcQuestData bobQuestData = FindNpcQuestDataByName(SpecialCharacters.HAZMAT_BOB);

        _hazmatBill = NpcManager.Instance.SpawnAndMoveToPosition(billQuestData._npcSO, billQuestData._specialPosition);
        _hazmatBob = NpcManager.Instance.SpawnAndMoveToPosition(bobQuestData._npcSO, bobQuestData._specialPosition);

        _hazmatBill.UpdateQuestData(_questInfoSo, this, billQuestData._conversationForQuest);
        _hazmatBob.UpdateQuestData(_questInfoSo, this, bobQuestData._conversationForQuest);

        // _hazmatBill.gameObject.SetActive(false);

        // QuestPointData pointData = FindQuestPointDataByName("INTRO_TO_HAZMAT_BILL");
        // InstantiateQuestPoint(pointData._spawnPosition.position, pointData._name);

        var (cutSceneData, cutScene) = InstantiateCutSceneAtPoint("INTRO_TO_HAZMAT_BILL");
        _activeCutScenePlayer = cutScene;
        _activeCutScenePlayer.OnCutSceneEnded += OnCutSceneEnded;
        _activeCutScenePlayer.OnCutSceneStarted += OnCutSceneStarted;
        _doneSetup = true;
        // Alberto.UpdateQuestData(_questInfoSo, this, questData._conversationForQuest);
    }

    void SetupAlberto()
    {
        NpcQuestData questData = FindNpcQuestDataByName(SpecialCharacters.ALBERTO);
        _alberto = NpcManager.Instance.GetSpecialCharacterByID(questData._characterID);
        _alberto.UpdateQuestData(_questInfoSo, this, questData._conversationForQuest);
        _alberto.OnInteractedWithQuestGiver += SendMessageToPhone;

        // _hazmatBill.gameObject.SetActive(false);
        // _hazmatBob.gameObject.SetActive(false);
        Debug.Log("Setup Alberto");
    }


    void SendMessageToPhone()
    {
        Debug.Log("Phone Should Ring Now");
    }




    public override void OnQuestItemInteracted(string questItemName)
    {
        switch (questItemName)
        {
            case "Door to Rover":
                // Rover.Instance._hasAccessToPlayer = true;
                QuestPointData pointData = FindQuestPointDataByName("First Energy Transfer");
                InstantiateQuestPoint(pointData._spawnPosition.position, pointData._name);
                break;
            case "First Weapon":
                ShowTutorialPrompt("Shooting");
                EventTutorial tutorial = FindTutorialByName("Aim");
                InstantiateTutorialPoint(tutorial, tutorial._spawnPoint);
                InstantiateCreature("WAVE_ONE");
                break;
            case "First Energy Transfer":
                ShowTutorialPrompt("First Energy Transfer");
                break;
            case "First Rover Interaction":
                ShowTutorialPrompt("First Energy Transfer");
                // Rover.Instance._hasAccessToPlayer = true;
                // Rover.Instance._activeInterface.DisconnectCable();
                break;
            case "SECTOR_16_EXIT":
                Debug.Log("Exiting Sector 16");
                break;
            default:
                break;
        }
        Debug.Log("Quest item interacted with: " + questItemName);
    }


    void OnCutSceneEnded(string cutSceneName)
    {
        Debug.Log("Cut scene ended: " + cutSceneName);
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
        Debug.Log("Cut scene started: " + cutSceneName);
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


    private T SphereCastForComponent<T>(Vector3 position, float radius, Vector3 dir) where T : Component
    {
        RaycastHit[] hits = new RaycastHit[10];
        int hitCount = Physics.SphereCastNonAlloc(new Ray(position, dir), radius, hits, 10f);

        for (int i = 0; i < hitCount; i++)
        {
            Debug.Log("Hit: " + hits[i].transform.name);
            T component = hits[i].transform.GetComponent<T>();
            if (component != null)
            {
                return component;
            }
        }
        return null;
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
