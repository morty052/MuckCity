using UnityEngine;

public class DelveManager : MonoBehaviour
{
    public static DelveManager Instance { get; private set; }
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }


    public void OnAcceptBounty(BountySO bountySO)
    {
        Debug.Log("Accepted Bounty Delve");
    }
    public void OnAcceptContract(ContractSO contractSO)
    {
        Debug.Log("Accepted Contract Delve");
    }

    void InitContract(ContractSO contractSO)
    {

    }
}
