using TMPro;
using UnityEngine;

public class DelveRewardPopUp : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _bountyText;
    public void DrawContractReward(DelveSO contractSO)
    {
        gameObject.SetActive(true);
    }


}
