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

    public void DrawContract(DelveSO contract)
    {
        // Debug.Log($"Contract {contract.name} selected");
        _rewardText.text = contract._bounty.ToString();
        _titleText.text = contract._name;
        _descriptionText.text = contract._description;
    }
    public void DrawBounty(BountySO bounty)
    {
        _rewardText.text = bounty._bounty.ToString();
        _titleText.text = bounty._name;
        _descriptionText.text = bounty._description;
        // Debug.Log($"Bounty {bounty.name} selected");
    }

    public void DrawContractReward()
    {

    }
    public void DrawBountyReward()
    {

    }
}
