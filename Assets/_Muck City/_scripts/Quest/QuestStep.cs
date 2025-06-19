using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;


public enum Creature
{
    SLIME_SLUG,
    GASTARID,
    VELOCIRAPTOR
}

public enum EventClipType
{
    ANNOUNCEMENT,
    ASSESMENT,
    MYSTERYGUY,
}

[Serializable]
public struct EventClip
{

    public string _name;

    public AudioClip _clip;

    public EventClipType _clipType;

    public Vector3 _position;

    public EventClip(string name, AudioClip clip, EventClipType clipType, Vector3 position)
    {
        _name = name;
        _clip = clip;
        _clipType = clipType;
        _position = position;
    }
}
[Serializable]
public struct EventTutorial
{

    public Sprite _image;
    public string _pcTitle;
    public string _xboxTitle;
    public string _playstationTitle;

    public string _pcDescription;
    public string _xboxDescription;
    public string _playstationDescription;

    public Vector3 _spawnPoint;



    public EventTutorial(string pcTitle, string xboxTitle, string playstationTitle, string pcDescription, string xboxDescription, string playstationDescription, Sprite image, Vector3 spawnPoint)
    {
        _image = image;
        _pcTitle = pcTitle;
        _pcDescription = pcDescription;
        _xboxTitle = xboxTitle;
        _xboxDescription = xboxDescription;
        _playstationTitle = playstationTitle;
        _playstationDescription = playstationDescription;
        _spawnPoint = spawnPoint;
    }
}
[Serializable]
public struct QuestPointData
{
    public string _name;
    public bool _completesObjective;


    public Pos _spawnPosition;

    public QuestPointData(Pos spawnPosition, string name, bool completes)
    {
        _spawnPosition = spawnPosition;
        _name = name;
        _completesObjective = completes;
    }
}



public abstract class QuestStep : MonoBehaviour
{

    [TabGroup("Details")]
    [SerializeField] protected QuestInfoSo _questInfoSo;
    [TabGroup("Details")]
    [SerializeField] bool _isFinished = false;
    [TabGroup("Details")]
    [SerializeField] string _questId;

    [TabGroup("Quest Points")]
    [SerializeField] GameObject _questPointPrefab;

    [TabGroup("Quest Points")]
    [SerializeField] List<QuestPointData> _questPointsData = new();

    [TabGroup("NPC's")]
    [SerializeField] List<NpcQuestData> _tiedCharactersQuestData = new();


    [TabGroup("Tutorials")]
    [SerializeField] TutorialTrigger _tutorialTriggerPrefab;

    [SerializeField] List<EventTutorial> _tutorials = new();

    [TabGroup("Mission")]
    [SerializeField] Mission _mission = new();


    [TabGroup("Audio")]
    [SerializeField] List<EventClip> _eventClips = new();

    [TabGroup("Audio")]
    [SerializeField] GameObject _currentClipObject;

    [TabGroup("CutScene's")]
    [SerializeField] List<CutSceneData> _questCutScenes = new();


    [TabGroup("Spawning")]
    [SerializeField] List<CreatureData> _spawnableCreatures = new();




    protected QuestPoint _activeQuestPoint;


    public void InitializeQuest(string questId)
    {
        this._questId = questId;
    }


    public QuestPoint InstantiateQuestPoint(string name)
    {
        QuestPointData pointData = FindQuestPointDataByName(name);
        GameObject questPoint = Instantiate(_questPointPrefab, pointData._spawnPosition.position, Quaternion.identity);
        questPoint.name = name;
        QuestPoint point = questPoint.GetComponent<QuestPoint>();
        point._tiedQuestStep = this;
        point._questItemData = new QuestItemData(name);
        point._completesObjective = pointData._completesObjective;

        _activeQuestPoint = point;

        _activeQuestPoint.OnEnterQuestPoint += OnEnterQuestPoint;

        return point;
    }

    protected virtual void OnEnterQuestPoint(string questPointName, bool completesObjective)
    {
        _activeQuestPoint.OnEnterQuestPoint -= OnEnterQuestPoint;
        _activeQuestPoint = null;
    }
    public (CutSceneData, TimelinePlayer) InstantiateCutSceneAtPoint(string name)
    {
        CutSceneData cutSceneData = FindCutSceneByName(name);
        TimelinePlayer cutScene = Instantiate(cutSceneData._cutScenePlayer.gameObject, cutSceneData._spawnPosition.position, Quaternion.identity).GetComponent<TimelinePlayer>();
        return (cutSceneData, cutScene);
    }

    public NpcQuestData FindNpcQuestDataByName(SpecialCharacters name)
    {
        NpcQuestData data = _tiedCharactersQuestData.Find(x => x._characterID == name);
        return data;
    }

    public QuestPointData FindQuestPointDataByName(string name)
    {
        QuestPointData data = _questPointsData.Find(x => x._name == name);
        return data;
    }
    CutSceneData FindCutSceneByName(string name)
    {
        CutSceneData data = _questCutScenes.Find(x => x._name == name);
        return data;
    }

    #region Mission Control
    public virtual void ActivateMission(int objectivesToDisplayOnstart = 0)
    {
        if (objectivesToDisplayOnstart > 0)
        {
            DomeManager.Instance.SetupMissionDisplay(_mission, objectivesToDisplayOnstart);
        }
        else
        {
            DomeManager.Instance.SetupMissionDisplay(_mission);
        }
    }

    public virtual void UpdateMissionObjectives(int index)
    {
        DomeManager.Instance.UpdateMissionDisplay(index);
    }

    public virtual void CompleteObjective(string objectiveTitle)
    {
        Objective objective = _mission._objectives.Find(x => x._title == objectiveTitle);
        DomeManager.Instance.CompleteObjective(objective._index);
        if (objective.IsUnityNull())
        {
            Debug.LogError("Could not find objective");
            return;
        }
    }

    #endregion
    protected void UseClip(string name)
    {
        EventClip clip = FindClipByName(name);
        switch (clip._clipType)
        {
            case EventClipType.ANNOUNCEMENT:
                break;
            case EventClipType.ASSESMENT:
                break;
            case EventClipType.MYSTERYGUY:
                break;
            default:
                break;
        }
    }

    protected IEnumerator DelayedInvoke(float delay, Action action)
    {
        yield return new WaitForSeconds(delay);
        action?.Invoke();
    }

    protected IEnumerator PlayClipAfterDelay(float delay, string clipName, float maxDistance = 50f, float volume = 1f, Action OnComplete = null)
    {
        yield return new WaitForSeconds(delay);
        UseClipAtPoint(clipName, maxDistance, volume, OnComplete);
    }

    protected void UseClipAtPoint(string name, float maxDistance = 50f, float volume = 1f, Action OnComplete = null)
    {
        EventClip clip = FindClipByName(name);
        switch (clip._clipType)
        {
            case EventClipType.ANNOUNCEMENT:
                break;
            case EventClipType.ASSESMENT:
                break;
            case EventClipType.MYSTERYGUY:
                break;
            default:
                break;
        }
        // AudioSource.PlayClipAtPoint(clip._clip, clip._position, 1f);
        PlaySoundAtPoint(clip._clip, clip._position, maxDistance, volume, OnComplete);
    }

    protected EventClip FindClipByName(string name)
    {
        EventClip clip = _eventClips.Find(x => x._name == name);
        return clip;
    }

    protected EventTutorial FindTutorialByName(string title)
    {
        EventTutorial clip = _tutorials.Find(x => x._pcTitle.Contains(title));
        return clip;
    }


    public CreatureData FindCreatureByType(string name)
    {
        CreatureData data = _spawnableCreatures.Find(x => x._tag == name);
        return data;
    }

    void PlaySoundAtPoint(AudioClip clip, Vector3 position, float maxDistance = 50f, float volume = 1f, Action OnComplete = null)
    {
        if (_currentClipObject != null)
        {
            return; // Prevent playing multiple clips at the same time
        }
        GameObject tempAudioSource = new("TempAudio");
        _currentClipObject = tempAudioSource;
        tempAudioSource.transform.position = position;

        AudioSource audioSource = tempAudioSource.AddComponent<AudioSource>();
        audioSource.clip = clip;
        audioSource.spatialBlend = 1.0f; // Fully 3D
        audioSource.maxDistance = maxDistance;  // Adjust as needed
        audioSource.rolloffMode = AudioRolloffMode.Linear; // Adjust rolloff mode
        audioSource.Play();

        if (OnComplete != null)
        {
            StartCoroutine(DelayedInvoke(clip.length, OnComplete));
        }
        Destroy(tempAudioSource, clip.length); // Destroy after the clip finishes
    }


    protected void ShowTutorialPrompt(string title)
    {
        EventTutorial tutorial = FindTutorialByName(title);
        string platformTitle = tutorial._pcTitle;
        string platformDescription = tutorial._pcDescription;
        GamePadType gamePadType = HudManager.Instance.DetectControllerType();
        if (gamePadType != GamePadType.NONE)
        {
            if (gamePadType == GamePadType.XBOX)
            {
                platformTitle = tutorial._xboxTitle;
                platformDescription = tutorial._xboxDescription;
            }
            else if (gamePadType == GamePadType.PLAYSTATION)
            {
                platformTitle = tutorial._playstationTitle;
                platformDescription = tutorial._playstationDescription;
            }
        }

        HudManager.Instance.ToggleTutorialPrompt(true, platformTitle, platformDescription, tutorial._image);
    }
    protected void FinishQuestStep()
    {
        if (!_isFinished)
        {
            _isFinished = true;
            GameEventsManager.Instance._questEvents.AdvanceQuest(_questId);
            Destroy(this.gameObject);
        }
    }


    public virtual void OnQuestItemInteracted(string questItemName)
    {
        Debug.Log("Quest item interacted with!");
    }



    public void InstantiateTutorialPoint(EventTutorial tutorial, Vector3 position)
    {
        TutorialTrigger tutorialPoint = Instantiate(_tutorialTriggerPrefab, position, Quaternion.identity);
        tutorialPoint.SetTutorial(tutorial);
    }



    public void InstantiateCreature(string tag)
    {
        CreatureData creatureData = FindCreatureByType(tag);
        if (creatureData._spawnPositions.Count > 0)
        {

            foreach (Vector3 spawnPosition in creatureData._spawnPositions)
            {
                AiAgent creatureObject = Instantiate(creatureData._creaturePrefab, spawnPosition, Quaternion.identity);
                // creatureObject.transform.SetParent(transform);
            }
        }
        else
        {
            AiAgent creatureObject = Instantiate(creatureData._creaturePrefab, creatureData._spawnPositions[0], Quaternion.identity);
            // creatureObject.transform.SetParent(transform);
        }
    }



}




