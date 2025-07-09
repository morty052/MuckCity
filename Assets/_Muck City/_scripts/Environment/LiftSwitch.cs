using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using DG.Tweening;
using System.Linq;


[System.Serializable]
public struct LiftBarrier
{
    public int _level;
    public Transform _leftItem;
    public Transform _rightItem;



    public LiftBarrier(int level, Transform leftItem, Transform rightItem)
    {
        _level = level;
        _leftItem = leftItem;
        _rightItem = rightItem;
    }
}


//TODO : NEEDS POLISHING
//TODO : NEEDS MINOR TWEAKS
public class LiftSwitch : MonoBehaviour, IInteractable, IBrowsable
{
    [SerializeField] private Lift _lift;
    [SerializeField] private bool _canInteract = true;
    string _interactionPrompt => "Use Lift";
    public bool CanInteract => _canInteract;

    bool isHighlighted = false;
    public bool IsHighlighted { get => isHighlighted; set => isHighlighted = value; }

    public string InteractionPrompt => _interactionPrompt;

    public GameObject GameObject => gameObject;

    public GameObject _interface;

    public Camera _closeUpCam;

    bool _isQuestItem;

    public bool IsQuestItem { get; set; }

    public int _selectedFloor = 0;
    [SerializeField] int _entryFloor = 0;
    [SerializeField] int _occupiedFloor = 0;
    [SerializeField] List<LiftBarrier> _barriers = new();

    [SerializeField] Text _floorText;

    public bool _isInteracting = false;

    public bool _isMoving;

    public void OnButtonPress(Inputs input)
    {
        switch (input)
        {
            case Inputs.DOWN:
                // MOVE TO PREVIOUS APP IF CAN GO BACK
                if (_selectedFloor > 0)
                {
                    _selectedFloor--;
                }
                else
                {
                    //MOVE TO LAST APP IF CANT GO BACK
                    _selectedFloor = _lift._points.Length - 1;
                }
                break;
            case Inputs.UP:
                if (_selectedFloor == _lift._points.Length - 1)
                {
                    //MOVE TO FIRST APP IF CANT GO FORWARD
                    _selectedFloor = 0;
                }

                else
                {
                    //MOVE TO NEXT APP IF CAN GO FORWARD
                    _selectedFloor++;
                }
                break;
            case Inputs.SELECT:
                UseLift();
                break;
            case Inputs.BACK:
                break;
            default:
                break;
        }
        if (_floorText != null)
        {
            _floorText.text = _selectedFloor.ToString();
        }

    }

    public void HideInteractionPrompt()
    {
        HudManager.Instance.HideInteractPrompt();
    }

    public void Interact()
    {
        HideInteractionPrompt();
        Player.Instance.transform.parent = _lift.transform;
        Player.Instance.UseAltControls(true, this);
        _interface.SetActive(true);
        _isInteracting = true;

    }

    void UseLift()
    {
        if (!_isInteracting || _selectedFloor == _entryFloor) return;
        _interface.SetActive(false);
        Player.Instance._vFootStep.Volume = 0f;
        (Transform entryLeftBarrier, Transform entryRightBarrier, LiftBarrier entryBarrier) = GetBarrier(_entryFloor);
        (Transform leftBarrier, Transform rightBarrier, LiftBarrier barrier) = GetBarrier(_selectedFloor);
        CloseBarriers(_entryFloor);
        _isMoving = true;
        //* LET LIFT KNOW IT IS OCCUPIED SO IT DISABLES AREAS 
        _lift._isCarryingPlayer = true;
        _lift.Move(_selectedFloor, () =>
        {
            Player.Instance.UseAltControls(false);
            Player.Instance._vFootStep.Volume = 1f;
            Player.Instance.transform.parent = null;
            _isInteracting = false;
            OpenBarriers(_selectedFloor);
            _occupiedFloor = _selectedFloor;
            _isMoving = false;
            _floorText.text = _selectedFloor.ToString();
        });
    }



    public void TryCloseBarriersOnExit(int level)
    {
        CloseBarriers(level);
    }

    public bool IsOnLevel(int level)
    {
        return _occupiedFloor == level;
    }

    public void CallElevator(int floor)
    {

        _selectedFloor = floor;
        _entryFloor = floor;
        (Transform leftBarrier, Transform rightBarrier, LiftBarrier barrier) = GetBarrier(floor);


        //* LET LIFT KNOW IT IS EMPTY SO IT DOES NOT DISABLE AREAS YET
        _lift._isCarryingPlayer = false;

        _lift.Move(_selectedFloor, () =>
              {
                  _isInteracting = false;
                  OpenBarriers(floor);
                  _occupiedFloor = _selectedFloor;
                  _floorText.text = _selectedFloor.ToString();
              });
    }


    public void OpenBarriers(int floor)
    {
        (Transform leftBarrier, Transform rightBarrier, LiftBarrier barrier) = GetBarrier(floor);
        leftBarrier.DORotate(new(leftBarrier.rotation.x, 90, -90), 1f);
        rightBarrier.DORotate(new(rightBarrier.rotation.x, 90, 270), 1f);
    }

    void CloseBarriers(int floor)
    {
        (Transform leftBarrier, Transform rightBarrier, LiftBarrier barrier) = GetBarrier(floor);
        leftBarrier.DORotate(new(0, 90, 0), 1f);
        rightBarrier.DORotate(new(rightBarrier.rotation.x, 90, 180), 1f);
    }

    public (Transform, Transform, LiftBarrier) GetBarrier(int floor)
    {
        LiftBarrier barrier = _barriers.Find(x => x._level == floor);
        Transform leftBarrier = barrier._leftItem;
        Transform rightBarrier = barrier._rightItem;


        return (leftBarrier, rightBarrier, barrier);
    }

    public void PrepareInteraction()
    {
        HudManager.Instance.ShowInteractPrompt(_interactionPrompt);
    }

    public void ToggleDrawAttention()
    {
        // Debug.Log("Toggle Draw Attention");

        isHighlighted = !isHighlighted;
    }
}
