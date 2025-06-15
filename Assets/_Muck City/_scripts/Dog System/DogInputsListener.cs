using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class DogInputsListener : MonoBehaviour
{

    [SerializeField] InputActionAsset _input;
    private InputAction _useRoverInput;
    private InputAction _callRoverInput;

    public bool _holdingUseRoverInputButton;

    public static Action OnCallRoverPressed;

    void Awake()
    {
        _useRoverInput = InputSystem.actions.FindAction("UseRover");
        _callRoverInput = InputSystem.actions.FindAction("CallRover");
    }

    void OnEnable()
    {
        _input.FindActionMap("Rover").Enable();
    }
    void OnDisable()
    {
        _input.FindActionMap("Rover").Disable();
    }


    void Update()
    {
        _holdingUseRoverInputButton = _useRoverInput.IsPressed();
        if (_holdingUseRoverInputButton)
        {
            if (_callRoverInput.WasPressedThisFrame())
            {

                OnCallRoverPressed.Invoke();
            }
        }
    }




    void OnAttackPressed()
    {
        Debug.Log("attack Pressed");
    }
}
