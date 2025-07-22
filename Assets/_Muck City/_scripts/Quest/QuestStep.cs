using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;



#if UNITY_EDITOR
using UnityEditor;
#endif


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


[System.Serializable]
public struct QuestItemData
{

    public string _name;


    public QuestItemData(string name)
    {
        _name = name;
    }
}

[System.Serializable]
public struct QuestItemStruct
{
    public string _name;

    [Tooltip("USED TO FIND OBJECTIVE OR ACTION TO PERFORM")]
    public string _tag;

    public Vector3 _position;

    public float _radius;

#if UNITY_EDITOR
    [Button]
    public void SetFromScene()
    {
        _position = Selection.activeGameObject.transform.position;
    }
#endif

    public QuestItemStruct(string name, string tag, Vector3 pos, float radius = 0)
    {
        _name = name;
        _tag = tag;
        _position = pos;
        _radius = radius;
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

    [TabGroup("Quest Items")]
    [SerializeField] List<QuestItemStruct> _questItemsData = new();

    [TabGroup("Quest Items")]
    public ObjectDetector _objectDetector;
    [TabGroup("Quest Items")]
    public LayerMask _detectionLayerMask;

    [TabGroup("NPC's")]
    [SerializeField] List<NpcQuestData> _tiedCharactersQuestData = new();

    [TabGroup("Tutorials")]
    [SerializeField] TutorialTrigger _tutorialTriggerPrefab;

    [TabGroup("Tutorials")]
    [SerializeField] List<EventTutorial> _tutorials = new();

    [TabGroup("Mission")]
    [SerializeField] Mission _mission = new();

    [TabGroup("Audio")]
    [SerializeField] List<ClipData> _eventClips = new();

    [TabGroup("CutScene's")]
    [SerializeField] List<CutSceneData> _questCutScenes = new();

    [TabGroup("Debug")]
    public bool _debug = new();

    protected QuestPoint _activeQuestPoint;


    public virtual void Awake()
    {
        _objectDetector = new(_detectionLayerMask);
    }


    #region "Quest helpers"
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
        point._questItemData = pointData;
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

    #endregion


    #region"Cut Scene"
    public (CutSceneData, TimelinePlayer) InstantiateCutSceneAtPoint(string name)
    {
        CutSceneData cutSceneData = FindCutSceneByName(name);
        TimelinePlayer cutScene = Instantiate(cutSceneData._cutScenePlayer.gameObject, cutSceneData._spawnPosition.position, Quaternion.Euler(cutSceneData._spawnPosition.rotation)).GetComponent<TimelinePlayer>();
        return (cutSceneData, cutScene);
    }

    public void GetObjectFromTimeLine(TimelinePlayer timelinePlayer)
    {
        PlayableDirector director = timelinePlayer.GetComponent<PlayableDirector>();
        TimelineAsset timeline = (TimelineAsset)director.playableAsset;

        foreach (var output in timeline.outputs)
        {
            var track = output.sourceObject as TrackAsset;
            var boundObject = director.GetGenericBinding(track);
            if (boundObject is NpcCharacter npc)
            {
                Debug.Log("Child is: " + boundObject.name);
                // Check if it has your NPC class
                var npcScript = npc.GetComponent<NpcCharacter>();
                if (npcScript != null)
                {
                    Debug.Log("Found NPC: " + npc.name);
                    // Now you can use npcScript to do whatever you need
                }
            }
        }
    }


    CutSceneData FindCutSceneByName(string name)
    {
        CutSceneData data = _questCutScenes.Find(x => x._name == name);
        return data;
    }

    #endregion

    #region Quest Item
    public QuestItemStruct FindQuestItemByName(string name)
    {
        List<QuestItemStruct> data = _questItemsData.FindAll(x => x._name == name);
        if (data.Count == 0)
        {
            Debug.LogError("No data found for " + name);
        }
        return data[0];
    }

    public T GetQuestItem<T>(string name, bool setupListener = false) where T : IFindable
    {
        // Debug.Log("Looking for " + name);
        QuestItemStruct itemData = FindQuestItemByName(name);
        T item = _objectDetector.DetectFindable<T>(itemData._position, itemData._radius);

        // Debug.Log("item is " + item.GameObject.name);
        if (setupListener)
        {
            AddQuestItemToObject(item, itemData);
        }

        return item;
    }

    protected void AddQuestItemToObject(IFindable obj, QuestItemStruct itemData)
    {
        QuestItem powerBackOnQuest = obj.GameObject.AddComponent<QuestItem>();
        powerBackOnQuest._questItemData = itemData;
        obj.IsQuestItem = true;
        // Debug.Log(obj.GameObject.name + " is a quest item");
        obj.SetupInteractionListener(OnQuestItemInteracted);
    }
    protected void RemoveInteractionListener(string tag)
    {
        List<QuestItemStruct> data = _questItemsData.FindAll(x => x._name == name);
        if (data.Count == 0)
        {
            Debug.LogError("No data found for " + name);
            return;
        }

        QuestItemStruct itemData = data[0];

        IFindable obj = _objectDetector.DetectFindable<IFindable>(itemData._position, itemData._radius);

        QuestItem item = obj.GameObject.GetComponent<QuestItem>();
        item._questItemData = itemData;

        obj.IsQuestItem = false;
        obj.RemoveInteractionListener(OnQuestItemInteracted);


    }

    public virtual void OnQuestItemInteracted(string questItemName)
    {
        Debug.Log("Quest item interacted with!");
    }
    #endregion


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

    public virtual void UpdateMissionObjectives(int index, bool initWaypoint = false)
    {
        DomeManager.Instance.UpdateMissionDisplay(index);
        if (initWaypoint)
        {
            Objective objective = _mission._objectives[index];
            Waypoint.Instance.Init(objective._wayPoint);
        }
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


    #region "audio"
    protected void UseClipAtPoint(string clipName, Transform position)
    {
        ClipData clip = FindClipByName(clipName);
        SoundsManager.Instance.PlayClip(clip._clip, position, 1f);
    }
    protected void UseClipAtPointWithEvent(string clipName, Transform position, Action OnComplete)
    {
        ClipData clip = FindClipByName(clipName);
        SoundsManager.Instance.PlayClipWithEventAtEnd(clip._clip, position, 1f, OnComplete);
    }

    protected ClipData FindClipByName(string name)
    {
        ClipData clip = _eventClips.Find(x => x._name == name);
        return clip;
    }

    #endregion


    #region"Tutorial"
    protected EventTutorial FindTutorialByName(string title)
    {
        EventTutorial clip = _tutorials.Find(x => x._pcTitle.Contains(title));
        return clip;
    }

    public void InstantiateTutorialPoint(EventTutorial tutorial, Vector3 position)
    {
        TutorialTrigger tutorialPoint = Instantiate(_tutorialTriggerPrefab, position, Quaternion.identity);
        tutorialPoint.SetTutorial(tutorial);
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

    #endregion

    protected IEnumerator DelayedInvoke(float delay, Action action)
    {
        yield return new WaitForSeconds(delay);
        action?.Invoke();
    }
    protected void FinishQuestStep()
    {
        if (!_isFinished)
        {
            _isFinished = true;
            GameEventsManager.Instance._questEvents.AdvanceQuest(_questId);
            Destroy(this.gameObject, 1f);
        }
    }






}




