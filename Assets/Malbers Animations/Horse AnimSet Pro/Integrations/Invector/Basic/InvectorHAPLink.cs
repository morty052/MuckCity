using UnityEngine;
using System.Collections;
using MalbersAnimations.HAP;
using Invector.CharacterController;
using MalbersAnimations.Events;
using Invector.vCharacterController;
using Invector.vCamera;
using Invector.vCharacterController.vActions;

/// <summary>
/// This is the Link between Invector and Horse AnimSet Pro Riding System
/// </summary>
namespace Invector.CharacterController
{
    public class InvectorHAPLink : MonoBehaviour
    {
        [Header("Camera States")]
        public string ride = "Ride";                              //Camera State to change to the Ride State
        public string Default = "Default";                        //Camera State to change to the Default State
        [Tooltip("Main Collider is set to Trigger")]
        public bool isTrigger = true;
        public bool debug;

         protected MRider Rider;                                   //Reference for the Rider
        [HideInInspector] protected vThirdPersonCamera vCamera;                     //Reference for the Invector's Camera
        [HideInInspector] protected vThirdPersonController v_TCP;                    //Reference for the Invector's Controller
        [HideInInspector] protected vThirdPersonInput VInput;                       //Reference for the Invector's Input

        void Awake()
        {
            InitializeLink();
        }

        /// <summary> Gets all the References and Connects all the Events </summary>
        protected virtual void InitializeLink()
        {
            Rider = GetComponent<MRider>();                     //Gets the Rider
            v_TCP = GetComponent<vThirdPersonController>();             //Gets the Invector's Controller
            VInput = GetComponent<vThirdPersonInput>();                 //Gets the Invector's Input

            //            vCamera =  vThirdPersonCamera.instance;   //Get Invector Camera
            if (Camera.main != null) vCamera = Camera.main.GetComponent<vThirdPersonCamera>();

            if (vCamera == null)
                vCamera = FindObjectOfType<vThirdPersonCamera>();
        }

        protected virtual void OnEnable()
        {
            if (Rider)
            {
                Rider.OnStartMounting.AddListener(OnStartMounting);
                Rider.OnEndMounting.AddListener(OnEndMounting);

                Rider.OnStartDismounting.AddListener(OnStartDismounting);
                Rider.OnEndDismounting.AddListener(OnEndDismounting);
            }
        }

        protected virtual void OnDisable()
        {
            if (Rider)
            {
                Rider.OnStartMounting.RemoveListener(OnStartMounting);
                Rider.OnEndMounting.RemoveListener(OnEndMounting);

                Rider.OnStartDismounting.RemoveListener(OnStartDismounting);
                Rider.OnEndDismounting.RemoveListener(OnEndDismounting);
            }

        }

        #region Event Listeners
/// <summary> Turn off the HUD while Mounting </summary>
        public virtual void TurnOffHUD()
        {
            var vtgA = ((MonoBehaviour)(Rider.Montura)).GetComponentInChildren<vTriggerGenericAction>();   //Find All 

            if (vtgA) vtgA.OnPlayerExit.Invoke(gameObject);                                                   //Turn off the hud when Mounting  
        }

        /// <summary>This will be invoked from the Event Rider.OnStartMounting</summary>
        public virtual void OnStartMounting()
        {
            VInput.SetLockBasicInput(true);         //Lock the Input since the horse is taking the command
            vCamera.RemoveLockTarget();                        //Unlock the Fixation if It has a Target

            VInput.ignoreTpCamera = true;

            if (vCamera.targetCamera != null && !string.IsNullOrEmpty(ride)) 
            vCamera.ChangeState(ride);                             //Change Camera State to default

            TurnOffHUD();
            v_TCP.isSprinting = false;                          //This will Stop Draining the Stamina if my any chance you mount the horse while sprinting is on
            v_TCP.enabled = false;

            var HeadTrack = GetComponent<vHeadTrack>(); 
            if (HeadTrack) VInput.onLateUpdate -= HeadTrack.UpdateHeadTrack; //Disable head tracking

            v_TCP.StopCharacter(); //Important (Remove all movement)
        }

        

        /// <summary>This will be invoked from the Event Rider.OnEndMounting</summary>
        public virtual void OnEndMounting()
        {
            if (!Rider.StartMounted)
            {
                VInput.SetLockBasicInput(true);                            //Make sure that the Invector's Input is Locked
                Rider.StartMounted.Value = false;
            }
            v_TCP.enabled = true;

            Rider.MainCollider.isTrigger = isTrigger;
        }

        /// <summary>This will be invoked from the Event Rider.OnStartDismounting</summary>
        public virtual void OnStartDismounting()
        {
            VInput.ignoreTpCamera = false;
            if ( !string.IsNullOrEmpty(Default))  vCamera.ChangeState(Default);                             //Change Camera State to default

            transform.rotation = Quaternion.FromToRotation(transform.up, -Physics.gravity)* transform.rotation; //Align the Character with the Global -Gravity
        }

        /// <summary>This will be invoked from the Event Rider.OnEndDismounting </summary>
        public virtual void OnEndDismounting()
        {
            if (VInput) VInput.SetLockBasicInput(false);                    //Unlocks the Invector's Input

            var HeadTrack = GetComponent<vHeadTrack>();
            if (HeadTrack) VInput.onLateUpdate += HeadTrack.UpdateHeadTrack;

            Rider.Anim.updateMode = AnimatorUpdateMode.Fixed; //Force to be back on Update Physics

            VInput.changeCameraState = false;                        /// DEACTIVATE custom camera state on the controller after dismounting
        }
        #endregion 
    }
}
