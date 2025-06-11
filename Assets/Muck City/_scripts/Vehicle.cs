using ArcadeVP;
using Invector.vCamera;
using Invector.vCharacterController;
using Unity.Cinemachine;
using UnityEngine;

public enum VehicleType
{
    SCOOTER,
    SEDAN,
    TRUCK,
    DIRTBIKE

}
public class Vehicle : MonoBehaviour
{

    public GenericInput _exitVehicleInput = new("C", "Y", "Y");
    [SerializeField] VehicleType _vehicleType;
    public Transform _exitPoint;



    ArcadeBP_Pro.ArcadeBikeControllerPro _bikeController;

    ArcadeVehicleController _vehicleController;

    InputManager_ArcadeVP _vehicleInputManager;

    [SerializeField] CinemachineCamera _followCam;


    public bool _isRiding = false;
    Rigidbody _rb;


    // Start is called once before the first execution of Update after the MonoBehaviour is created




    void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _rb.isKinematic = true;
        if (_vehicleType == VehicleType.SEDAN || _vehicleType == VehicleType.TRUCK)
        {
            _vehicleController = GetComponent<ArcadeVehicleController>();
            _vehicleInputManager = GetComponent<InputManager_ArcadeVP>();
            _vehicleController.enabled = false;
            _vehicleInputManager.enabled = false;
        }
        if (_vehicleType == VehicleType.SCOOTER || _vehicleType == VehicleType.DIRTBIKE)
        {
            _bikeController = GetComponent<ArcadeBP_Pro.ArcadeBikeControllerPro>();
            _bikeController.enabled = false;
        }
    }

    void Update()
    {
        if (_isRiding)
        {
            ExitVehicleOnPress();
        }
    }
    public void Activate()
    {
        HudManager.Instance.HideInteractPrompt();
        Player.Instance.EnterVehicleMode(this);
        if (_vehicleType == VehicleType.SEDAN || _vehicleType == VehicleType.TRUCK)
        {
            _vehicleController.enabled = true;
            _vehicleInputManager.enabled = true;
            _followCam.gameObject.SetActive(true);
        }
        if (_vehicleType == VehicleType.SCOOTER || _vehicleType == VehicleType.DIRTBIKE)
        {
            _bikeController.enabled = true;
        }
        _rb.isKinematic = false;
        _isRiding = true;
    }

    void ExitVehicleOnPress()
    {
        if (_exitVehicleInput.GetButtonDown())
        {
            if (_vehicleType == VehicleType.SEDAN || _vehicleType == VehicleType.TRUCK)
            {
                _vehicleController.enabled = false;
                _vehicleInputManager.enabled = false;
                _followCam.gameObject.SetActive(false);
            }
            if (_vehicleType == VehicleType.SCOOTER || _vehicleType == VehicleType.DIRTBIKE)
            {
                _bikeController.enabled = false;
            }
            _rb.isKinematic = true;
            _isRiding = false;
            Player.Instance.ExitVehicleMode();
        }
    }

    public void ShowRidePrompt()
    {
        HudManager.Instance.ShowInteractPrompt("Ride");
    }

    public void HideRidePrompt()
    {
        HudManager.Instance.HideInteractPrompt();
    }



}
