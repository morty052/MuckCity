using UnityEngine;


[CreateAssetMenu(fileName = "QuestInfoSo", menuName = "ScriptableObjects/QuestInfo", order = 1)]
public class QuestInfoSo : ScriptableObject
{

    [field: SerializeField] public string _id { get; private set; }

    [Header("General")]

    public string _displayName;

    [Header("Requirements")]
    public int dayRequirement;

    public QuestInfoSo[] questPrerequisites;

    [Header("Steps")]
    public GameObject[] questStepPrefabs;

    [Header("Rewards")]

    public int goldReward;

    public int experienceReward;


    private void OnValidate()
    {
#if UNITY_EDITOR
        _id = this.name;
        UnityEditor.EditorUtility.SetDirty(this);
#endif
    }
}
