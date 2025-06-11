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

    public static event Action<PhoneInputs> OnButtonPress;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void OnEnable()
    {

    }


    void OnSelect(InputValue value)
    {
        OnButtonPress?.Invoke(PhoneInputs.SELECT);
    }

    public void OnUp(InputValue value)
    {
        Debug.Log("Up pressed");
        OnButtonPress?.Invoke(PhoneInputs.UP);
    }

    public void OnDown(InputValue value)
    {
        Debug.Log("Down pressed");
        OnButtonPress?.Invoke(PhoneInputs.DOWN);
    }
    public void OnLeft(InputValue value)
    {
        Debug.Log("Left pressed");
        OnButtonPress?.Invoke(PhoneInputs.LEFT);
    }
    public void OnRight(InputValue value)
    {
        Debug.Log("Right pressed");
        OnButtonPress?.Invoke(PhoneInputs.RIGHT);
    }
    public void OnBack(InputValue value)
    {
        OnButtonPress?.Invoke(PhoneInputs.BACK);
    }

    public void OnAccept()
    {
        OnButtonPress?.Invoke(PhoneInputs.ACCEPT);
    }
    public void OnReject()
    {
        OnButtonPress?.Invoke(PhoneInputs.REJECT);
    }

    public void OnHidePhone(InputValue value)
    {
        GameEventsManager.Instance.HidePhoneEvent();
    }
}
