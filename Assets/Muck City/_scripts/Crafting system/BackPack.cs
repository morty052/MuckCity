using UnityEngine;

public class BackPack : Storage
{
    public bool _isUnequipped;
    void Start()
    {
        _storageType = StorageType.CAN_BE_EQUIPPED;
    }

    // Update is called once per frame
    public override void Interact()
    {
        if (_isUnequipped)
        {
            HudManager.Instance.HideInteractPrompt();
            Player.Instance.EquipBackPack(transform);

        }
    }
}
