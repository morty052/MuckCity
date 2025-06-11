
// using StarterAssets;
using TMPro;
using UnityEngine;

[System.Serializable]
public struct BranchDecision
{
    public string _title;
    public string _optionOne;
    public string _optionTwo;


    public BranchDecision(string title, string optionOne, string optionTwo)
    {
        _title = title;
        _optionOne = optionOne;
        _optionTwo = optionTwo;
    }
}
public class BranchDecisionManager : MonoBehaviour
{

    [SerializeField] private GameObject _branchDecisionCanvas;
    [SerializeField] private TextMeshProUGUI _titleText;
    [SerializeField] private TextMeshProUGUI _optionOneText;
    [SerializeField] private TextMeshProUGUI _optionTwoText;



    public enum BranchDecisionType { NONE, OPTION_ONE, OPTION_TWO }

    void OnEnable()
    {
        GameEventsManager.BranchDecisionStarted += OnBranchDecisionStarted;
        GameEventsManager.BranchDecisionSelected += OnBranchDecisionSelected;
        GameEventsManager.BranchDecisionEnded += OnBranchDecisionEnded;
    }
    void OnDisable()
    {
        GameEventsManager.BranchDecisionStarted -= OnBranchDecisionStarted;
        GameEventsManager.BranchDecisionSelected -= OnBranchDecisionSelected;
        GameEventsManager.BranchDecisionEnded -= OnBranchDecisionEnded;
        StopAllCoroutines();
    }

    private void OnBranchDecisionSelected(BranchDecisionType type)
    {
        _branchDecisionCanvas.SetActive(false);
    }

    private void OnBranchDecisionEnded()
    {
        _branchDecisionCanvas.SetActive(false);
    }

    private void OnBranchDecisionStarted(BranchDecision branchDecision)
    {
        _titleText.text = branchDecision._title;
        _optionOneText.text = branchDecision._optionOne;
        _optionTwoText.text = branchDecision._optionTwo;
        _branchDecisionCanvas.SetActive(true);
    }




}
