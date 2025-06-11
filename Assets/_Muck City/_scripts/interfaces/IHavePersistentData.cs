

using UnityEngine;

public interface IHavePersistentData
{
    public bool ShouldAutoSave { get; set; }
    [SerializeField] public string SAVE_FILE_NAME { get; set; }
    void TriggerAutoSave();

    void LoadPersistentData();
}
