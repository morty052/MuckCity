using System;
using UnityEngine;
using UnityEngine.InputSystem;

public enum PhoneInputs
{
    UP,
    DOWN,
    LEFT,
    RIGHT,
    SELECT,
    BACK,
    ACCEPT,
    REJECT
}

public class PhoneNavigation : MonoBehaviour
{

    [SerializeField] InputActionAsset _input;
    [SerializeField] bool _isPhoneShowing;
    private InputAction _selectInput;
    private InputAction _upInput;
    private InputAction _downInput;

    private InputAction _leftInput;

    private InputAction _rightInput;

    private InputAction _backInput;

    private InputAction _acceptInput;

    private InputAction _rejectInput;

    public static event Action<PhoneInputs> OnButtonPress;


    void Awake()
    {
        _selectInput = InputSystem.actions.FindAction("Select");
        _upInput = InputSystem.actions.FindAction("Up");
        _downInput = InputSystem.actions.FindAction("Down");
        _leftInput = InputSystem.actions.FindAction("Left");
        _rightInput = InputSystem.actions.FindAction("Right");
        _backInput = InputSystem.actions.FindAction("Back");
        _acceptInput = InputSystem.actions.FindAction("Accept");
        _rejectInput = InputSystem.actions.FindAction("Reject");
    }

    void OnEnable()
    {
        _input.FindActionMap("Phone").Enable();
    }
    void OnDisable()
    {
        _input.FindActionMap("Phone").Disable();
    }

    public void ToggleUseInput()
    {
        _isPhoneShowing = !_isPhoneShowing;
    }

    void Update()
    {
        if (!_isPhoneShowing) return;
        if (_selectInput.WasPressedThisFrame())
        {
            OnButtonPress?.Invoke(PhoneInputs.SELECT);
        }

        if (_upInput.WasPressedThisFrame())
        {
            Debug.Log("Up pressed");
            OnButtonPress?.Invoke(PhoneInputs.UP);
        }

        if (_downInput.WasPressedThisFrame())
        {
            Debug.Log("Down pressed");
            OnButtonPress?.Invoke(PhoneInputs.DOWN);
        }

        if (_leftInput.WasPressedThisFrame())
        {
            Debug.Log("Left pressed");
            OnButtonPress?.Invoke(PhoneInputs.LEFT);
        }

        if (_rightInput.WasPressedThisFrame())
        {
            Debug.Log("Right pressed");
            OnButtonPress?.Invoke(PhoneInputs.RIGHT);
        }

        if (_backInput.WasPressedThisFrame())
        {
            OnButtonPress?.Invoke(PhoneInputs.BACK);
        }

        if (_acceptInput.WasPressedThisFrame())
        {
            OnButtonPress?.Invoke(PhoneInputs.ACCEPT);
        }

        if (_rejectInput.WasPressedThisFrame())
        {
            OnButtonPress?.Invoke(PhoneInputs.REJECT);
        }


    }


    // void OnSelect(InputValue value)
    // {
    //     OnButtonPress?.Invoke(PhoneInputs.SELECT);
    // }

    // public void OnUp(InputValue value)
    // {

    //     OnButtonPress?.Invoke(PhoneInputs.UP);
    // }

    // public void OnDown(InputValue value)
    // {
    //     Debug.Log("Down pressed");
    //     OnButtonPress?.Invoke(PhoneInputs.DOWN);
    // }
    // public void OnLeft(InputValue value)
    // {
    //     Debug.Log("Left pressed");
    //     OnButtonPress?.Invoke(PhoneInputs.LEFT);
    // }
    // public void OnRight(InputValue value)
    // {
    //     Debug.Log("Right pressed");
    //     OnButtonPress?.Invoke(PhoneInputs.RIGHT);
    // }
    // public void OnBack(InputValue value)
    // {
    //     OnButtonPress?.Invoke(PhoneInputs.BACK);
    // }

    // public void OnAccept()
    // {
    //     OnButtonPress?.Invoke(PhoneInputs.ACCEPT);
    // }
    // public void OnReject()
    // {
    //     OnButtonPress?.Invoke(PhoneInputs.REJECT);
    // }

    // public void OnHidePhone(InputValue value)
    // {
    //     GameEventsManager.Instance.HidePhoneEvent();
    // }
}
