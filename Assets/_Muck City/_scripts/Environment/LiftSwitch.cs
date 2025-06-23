using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using DG.Tweening;


[System.Serializable]
public struct LiftBarrier
{
  public int _level;
  public Transform _leftItem;
  public Transform _rightItem;
  private bool _isOpen;

  public void SetIsOpen(bool state)
  {
     _isOpen = state;
  }
  public bool IsOpen()
  {
     return _isOpen;
  }
    public LiftBarrier(int level, Transform leftItem, Transform rightItem)
    {
        _level = level;
        _leftItem = leftItem;
        _rightItem = rightItem;
        _isOpen = false;
    }
}


public class LiftSwitch : MonoBehaviour, IInteractable
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

    [SerializeField] int _selectedFloor =0;
    [SerializeField] int _entryFloor =0;
    [SerializeField] List<LiftBarrier> _barriers = new();

   [SerializeField] Text _floorText;

   public bool _isInteracting = false;

    void OnEnable()
    {
        PhoneNavigation.OnButtonPress += OnPhoneButtonPress;
    }

    void OnDisable()
    {
        PhoneNavigation.OnButtonPress -= OnPhoneButtonPress;
    }


    void OnPhoneButtonPress(PhoneInputs input)
    {
        switch (input)
        {
            case PhoneInputs.DOWN:
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
            case PhoneInputs.UP:
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
            case PhoneInputs.SELECT:
            UseLift();
                break;
            case PhoneInputs.BACK:
                break;
            default:
                break;
        }
        Debug.Log("Selected Floor");
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
        Player.Instance.LockAllInput(true);
        Player.Instance._isPhoneShowing.Value = true;
        // _closeUpCam.gameObject.SetActive(true);
        _interface.SetActive(true);
         _isInteracting = true;
        // StartLift();
        // Player.Instance.MoveToPosition(_lift._centerPoint.transform.position, false, () => StartLift());
    }

    void UseLift()
    {  
      if (!_isInteracting)return;
     _interface.SetActive(false);
     (Transform entryLeftBarrier,Transform entryRightBarrier, LiftBarrier entryBarrier) = GetBarrier(_entryFloor);
     (Transform leftBarrier,Transform rightBarrier, LiftBarrier barrier) = GetBarrier(_selectedFloor);
     CloseBarriers(entryLeftBarrier, entryRightBarrier, entryBarrier);
     _lift.Move(_selectedFloor, () => 
     {
         Player.Instance.LockAllInput(false);
        _isInteracting = false;
        OpenBarriers(leftBarrier, rightBarrier, barrier);
     });
    }

    void StartLift()
    {
        _lift.Move(() => Player.Instance.ToggleInputLock());
    }

    public void TryCloseBarriersOnExit()
    {
        
        (Transform leftBarrier,Transform rightBarrier, LiftBarrier barrier) = GetBarrier(_selectedFloor);
        
        if (barrier.IsOpen())
        { 
            Debug.Log($"<color=blue> barrier is open </color>");
            CloseBarriers(leftBarrier,rightBarrier, barrier);
        }
        else
        {
            Debug.Log($"<color=orange> barrier is closed </color>");
        }

    }

    public void CallElevator(int floor)
    {
       _selectedFloor = floor;
       _entryFloor = floor;
        (Transform leftBarrier,Transform rightBarrier, LiftBarrier barrier) = GetBarrier(floor);
        // LiftBarrier barrier = _barriers.Find(x => x._level == floor);
        // Transform leftBarrier = barrier._leftItem;
        // Transform rightBarrier = barrier._rightItem;
        if (_selectedFloor == floor)
        {
            OpenBarriers(leftBarrier, rightBarrier, barrier);
            return;
        }
        _lift.Move(_selectedFloor, () => 
              {
                _isInteracting = false;
                OpenBarriers(leftBarrier, rightBarrier, barrier);
              });
    }

    void OpenBarriers(Transform leftBarrier,Transform rightBarrier, LiftBarrier barrier)
    {
        leftBarrier.DORotate(new(leftBarrier.rotation.x,90,-90),1f);
        rightBarrier.DORotate(new(rightBarrier.rotation.x,90,270),1f);
        barrier.SetIsOpen(true);
    }
    void CloseBarriers(Transform leftBarrier,Transform rightBarrier, LiftBarrier barrier)
    {
        leftBarrier.DORotate(new(0,90,0),1f);
        rightBarrier.DORotate(new(rightBarrier.rotation.x,90,180),1f);
        barrier.SetIsOpen(false);
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
