using System;
using UnityEngine;
using UnityEngine.InputSystem;

public enum Inputs
{
    UP,
    DOWN,
    LEFT,
    RIGHT,
    SELECT,
    BACK,
    ACCEPT,
    REJECT,
    EXIT,
    BUY,
    INSPECT
}


public class AltInput : MonoBehaviour
{

    public InputActionAsset _input;

    #region Browsable Input

    public bool _isUsingAltInput;
    private InputAction _selectInput;
    private InputAction _upInput;
    private InputAction _downInput;

    private InputAction _leftInput;

    private InputAction _rightInput;

    private InputAction _backInput;

    private InputAction _acceptInput;

    private InputAction _rejectInput;
    private InputAction _exitInput;
    private InputAction _buyInput;
    private InputAction _inspectInput;

    private int _buyHoldCounter = 0;
    private int _buyHoldThreshold = 30;

    public IBrowsable _activeBrowsable;
    #endregion

    #region Secondary Inputs
    private InputAction _useEquipmentInput;
    private InputAction _toggleEquipmentWheelInput;
    #endregion

    #region Events
    public static Action OnPressUseSpecialEquipment;
    public static Action OnToggleEquipmentWheel;
    #endregion


    void OnEnable()
    {
        InitInputs();
        InitSecondaryInputs();
        _input.FindActionMap("Phone").Enable();
        _input.FindActionMap("Alts").Enable();
    }


    public void OnDisable()
    {
        _input.FindActionMap("Phone").Disable();

    }


    void Update()
    {
        if (_isUsingAltInput)
        {
            HandleBrowsingInputs();
        }

        HandleSecondaryInputs();
    }

    void HandleSecondaryInputs()
    {
        if (_useEquipmentInput.WasPressedThisFrame())
        {
            OnPressUseSpecialEquipment?.Invoke();
        }

        if (_toggleEquipmentWheelInput.WasPressedThisFrame())
        {
            OnToggleEquipmentWheel?.Invoke();
        }
    }

    void HandleBrowsingInputs()
    {
        if (_selectInput.WasPressedThisFrame())
        {
            Debug.Log("Select Pressed");
            _activeBrowsable.OnButtonPress(Inputs.SELECT);
        }

        if (_upInput.WasPressedThisFrame())
        {
            Debug.Log("Up pressed");
            _activeBrowsable.OnButtonPress(Inputs.UP);
        }

        if (_downInput.WasPressedThisFrame())
        {
            Debug.Log("Down pressed");
            _activeBrowsable.OnButtonPress(Inputs.DOWN);
        }

        if (_leftInput.WasPressedThisFrame())
        {
            Debug.Log("Left pressed");
            _activeBrowsable.OnButtonPress(Inputs.LEFT);
        }

        if (_rightInput.WasPressedThisFrame())
        {
            Debug.Log("Right pressed");
            _activeBrowsable.OnButtonPress(Inputs.RIGHT);
        }

        if (_backInput.WasPressedThisFrame())
        {
            _activeBrowsable.OnButtonPress(Inputs.BACK);
        }

        if (_acceptInput.WasPressedThisFrame())
        {
            _activeBrowsable.OnButtonPress(Inputs.ACCEPT);
        }

        if (_rejectInput.WasPressedThisFrame())
        {
            _activeBrowsable.OnButtonPress(Inputs.REJECT);
        }
        if (_exitInput.WasPressedThisFrame())
        {
            _activeBrowsable.OnButtonPress(Inputs.EXIT);
        }

        if (_inspectInput.WasPressedThisFrame())
        {
            _activeBrowsable.OnButtonPress(Inputs.INSPECT);
        }

        HandleBuyButton();
    }


    void InitInputs()
    {
        _selectInput = InputSystem.actions.FindAction("Select");
        _upInput = InputSystem.actions.FindAction("Up");
        _downInput = InputSystem.actions.FindAction("Down");
        _leftInput = InputSystem.actions.FindAction("Left");
        _rightInput = InputSystem.actions.FindAction("Right");
        _backInput = InputSystem.actions.FindAction("Back");
        _acceptInput = InputSystem.actions.FindAction("Accept");
        _rejectInput = InputSystem.actions.FindAction("Reject");
        _exitInput = InputSystem.actions.FindAction("Exit");
        _buyInput = InputSystem.actions.FindAction("Buy");
        _inspectInput = InputSystem.actions.FindAction("Inspect");
    }
    void InitSecondaryInputs()
    {
        _useEquipmentInput = InputSystem.actions.FindAction("UseSpecialEquipment");
        _toggleEquipmentWheelInput = InputSystem.actions.FindAction("ToggleEquipmentWheel");
    }


    public void ToggleUseInput(bool state)
    {
        _isUsingAltInput = state;
    }





    public void HandleBuyButton()
    {


        if (_buyInput.IsPressed())
        {
            _buyHoldCounter++; // increment counter while button is held down
                               // you can use the _buyHoldCounter value as needed

            if (_buyHoldCounter == _buyHoldThreshold)
            {
                Debug.Log("Buy button held down for " + _buyHoldCounter + " frames");
                _activeBrowsable.OnButtonPress(Inputs.BUY);
                _buyHoldCounter = 0;
            }
        }
        else if (_buyInput.WasReleasedThisFrame())
        {
            _buyHoldCounter = 0; // reset counter when button is released
        }
    }

}


