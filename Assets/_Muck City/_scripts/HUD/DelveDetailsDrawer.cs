using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DelveDetailsDrawer : MonoBehaviour
{
    [SerializeField, TabGroup("Components")] Image _previewImage;
    [SerializeField, TabGroup("Text Components")] TextMeshProUGUI _rewardText;
    [SerializeField, TabGroup("Text Components")] TextMeshProUGUI _titleText;
    [SerializeField, TabGroup("Text Components")] TextMeshProUGUI _descriptionText;

    public void DrawContract(ContractSO contract)
    {
        Debug.Log($"Contract {contract.name} selected");
    }
    public void DrawBounty(BountySO bounty)
    {
        Debug.Log($"Bounty {bounty.name} selected");
    }
}
