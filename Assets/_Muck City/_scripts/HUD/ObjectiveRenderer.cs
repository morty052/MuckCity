using System;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

public class ObjectiveRenderer : MonoBehaviour
{

    public GameObject _container;
    public TextMeshProUGUI _title;
    public GameObject _icon;

    public ObjectiveItem _objectiveItemPrefab;
    public Transform _objectiveListParent;
    public bool isComplete = false;

    Mission _activeMission;


    public void SetupMission(Mission mission)
    {

        _activeMission = mission;
        _container.SetActive(true);
        _title.text = mission._title;
        if (mission._objectives.Count == 0) return;
        foreach (Objective objective in mission._objectives)
        {
            ObjectiveItem objectiveItem = Instantiate(_objectiveItemPrefab, _objectiveListParent);
            objectiveItem.SetupObjective(objective);
        }
    }
    public void SetupMission(Mission mission, int objectivesToDisplayOnStart)
    {
        _activeMission = mission;
        _container.SetActive(true);
        _title.text = mission._title;
        if (mission._objectives.Count == 0) return;

        for (int i = 0; i < objectivesToDisplayOnStart; i++)
        {
            ObjectiveItem objectiveItem = Instantiate(_objectiveItemPrefab, _objectiveListParent);
            objectiveItem.SetupObjective(mission._objectives[i]);
        }
    }


    [Button("Complete Objective")]
    public void CompleteObjective(int index)
    {
        if (_objectiveListParent.childCount == 0)
        {
            Debug.LogError("no objectives to complete");
            return;
        }


        if (_objectiveListParent.childCount == 1)
        {
            Debug.LogError("only one objective to complete");
            ObjectiveItem item = _objectiveListParent.GetChild(0).GetComponent<ObjectiveItem>();
            item.CompleteObjective();
            return;
        }

        ObjectiveItem objectiveItem = _objectiveListParent.GetChild(index).GetComponent<ObjectiveItem>();
        objectiveItem.CompleteObjective();
    }

    public void UpdateMissionDisplay(int objectivesToAdd)
    {
        ObjectiveItem objectiveItem = Instantiate(_objectiveItemPrefab, _objectiveListParent);
        objectiveItem.SetupObjective(_activeMission._objectives[objectivesToAdd]);
    }
}
