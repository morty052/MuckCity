

using UnityEngine;

public interface IHavePersistentData
{
    public bool ShouldAutoSave { get; }
    public SaveAble SAVE_ID { get; }

    void TriggerAutoSave();
    void AutoSave();

    void LoadPersistentData();
}
