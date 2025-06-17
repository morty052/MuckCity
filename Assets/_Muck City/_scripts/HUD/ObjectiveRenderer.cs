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


    public void SetupMission(Mission mission)
    {
        _container.SetActive(true);
        _title.text = mission._title;
        if (mission._objectives.Count == 0) return;
        foreach (Objective objective in mission._objectives)
        {
            ObjectiveItem objectiveItem = Instantiate(_objectiveItemPrefab, _objectiveListParent);
            objectiveItem.SetupObjective(objective);
        }
    }

}
