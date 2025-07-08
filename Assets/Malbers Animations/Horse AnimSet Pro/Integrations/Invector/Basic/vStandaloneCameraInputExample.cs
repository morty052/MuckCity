using Invector;
using Invector.vCharacterController;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class vStandaloneCameraInputExample : MonoBehaviour
{
    [vHelpBox("Enable this component only when the vThirdPersonInput is disable, so you can update the CameraInput separatelly")]

    [vEditorToolbar("Camera Settings")]
    public bool lockCameraInput;
    public bool invertCameraInputVertical, invertCameraInputHorizontal;

    public GenericInput rotateCameraXInput = new GenericInput("Mouse X", "RightAnalogHorizontal", "Mouse X");
    public GenericInput rotateCameraYInput = new GenericInput("Mouse Y", "RightAnalogVertical", "Mouse Y");
    public GenericInput cameraZoomInput = new GenericInput("Mouse ScrollWheel", "", "");

    //[HideInInspector]
    public Invector.vCamera.vThirdPersonCamera tpCamera;         // access tpCamera info

    protected Camera _cameraMain;
    protected bool withoutMainCamera;
    public Camera cameraMain
    {
        get
        {
            if (!_cameraMain && !withoutMainCamera)
            {
                if (!Camera.main)
                {
                    Debug.Log("Missing a Camera with the tag MainCamera, please add one.");
                    withoutMainCamera = true;
                }
                else
                {
                    _cameraMain = Camera.main;
                }
            }
            return _cameraMain;
        }
        set
        {
            _cameraMain = value;
        }
    }

    void Start()
    {
        FindCamera();
    }


    void LateUpdate()
    {
        CameraInput();
    }

    public virtual void FindCamera()
    {
        var tpCameras = FindObjectsOfType<Invector.vCamera.vThirdPersonCamera>();

        if (tpCameras.Length > 1)
        {
            tpCamera = System.Array.Find(tpCameras, tp => !tp.isInit);

            if (tpCamera == null)
            {
                tpCamera = tpCameras[0];
            }

            if (tpCamera != null)
            {
                for (int i = 0; i < tpCameras.Length; i++)
                {
                    if (tpCamera != tpCameras[i])
                    {
                        Destroy(tpCameras[i].gameObject);
                    }
                }
            }
        }
        else if (tpCameras.Length == 1)
        {
            tpCamera = tpCameras[0];
        }
    }

    public virtual void CameraInput()
    {
        if (!cameraMain)
        {
            return;
        }

        if (tpCamera == null)
        {
            return;
        }

        var Y = lockCameraInput ? 0f : rotateCameraYInput.GetAxis();
        var X = lockCameraInput ? 0f : rotateCameraXInput.GetAxis();
        if (invertCameraInputHorizontal)
        {
            X *= -1;
        }

        if (invertCameraInputVertical)
        {
            Y *= -1;
        }

        var zoom = cameraZoomInput.GetAxis();

        tpCamera.RotateCamera(X, Y);
        if (!lockCameraInput)
        {
            tpCamera.Zoom(zoom);
        }
    }

}
