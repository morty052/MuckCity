using Invector.vCharacterController;
using Invector.vItemManager;
using Invector.vShooter;
using MalbersAnimations;
using MalbersAnimations.Utilities;
using MalbersAnimations.Weapons;
using System.Collections;
using UnityEngine;

namespace Invector.CharacterController
{
    /// <summary> This is the link between Invector and Malbers Assets</summary>
    public class InvectorHAPLinkShooter : InvectorHAPLinkMelee
    {
        vShooterManager v_ShooterManager;           //Gets the Reference for the vShooterManager
        vShooterWeapon V_ShooterWeapon;
        MShootable Malbers_Gun;
        MBow Malbers_Bow;
        public string AimRight = "RideAimRight";    //Camera State for Aim Right
        public string AimLeft = "RideAimLeft";      //Camera Statte for Aim Left
        int TotalAmmo;
        Aim Aimer;

        vShooterMeleeInput ShooterInput => (VInput as vShooterMeleeInput);

        protected override GameObject Weapon
        {
            set
            {
                base.Weapon = value;

                V_ShooterWeapon = value ? value.GetComponent<vShooterWeapon>() : null;
                Malbers_Gun = value ? value.GetComponent<MShootable>() : null;
                Malbers_Bow = value ? value.GetComponent<MBow>() : null;
            }
        }


        void Awake() { InitializeLink(); }

        protected override void InitializeLink()
        {
            base.InitializeLink();

            v_ShooterManager = GetComponent<vShooterManager>();                         //Gets the Invector's Melee Manager   
            Aimer = GetComponent<Aim>();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            Aimer.OnAimSide.AddListener(OnAimSide);                               //Add Listener for the Aiming Side
        }

        /// <summary>  Remove All Listeners of all subcribed events  </summary>
        protected override void OnDisable()
        {
            base.OnDisable();
            Aimer.OnAimSide.RemoveListener(OnAimSide);
        }

        public override void OnStartMounting()
        {
            base.OnStartMounting();

            if (v_ShooterManager.rWeapon) Weapon = v_ShooterManager.rWeapon.gameObject;             //Store the weapon that is on the Right Hand
            else if (v_ShooterManager.lWeapon) Weapon = v_ShooterManager.lWeapon.gameObject;        //Store the weapon that is on the Left  Hand

            if (Weapon && riderCombat)                                           //Update the weapons for the Shooter   
            {
                if (IsCompatible)
                {
                    riderCombat.Equip_Fast(Weapon);                //If do has the ImWeapon Interface LetKnow the RiderCombat that can Use the weapon
                }
                else
                {
                    Weapon.SetActive(false);                                    //Hide the Weapon if is not compatible
                    OnMountWithWeapon.Invoke();                                 //Let everybondy knows that is mounting with a weapon on the Hand    
                }
            }

            UpdateLinkAmmo();                                                           //Update ammo  for the IGun if there's A weapon on the Hand

            v_ShooterManager.useLeftIK = false;                                         //Disable the IK on the Left Hand
            v_ShooterManager.useRightIK = false;                                         //Disable the IK on the Left Hand
            ShooterInput.ignoreIK = true;
            ShooterInput.aimInput.useInput = false; //Deactivate the Aiming, since HAP is doing it
            ShooterInput.shotInput.useInput = false; //Deactivate the shooting, since HAP is doing it

            Animator_on_Bow(false);
        }

        public override void OnEndMounting()
        {
            base.OnEndMounting();
            Animator_on_Bow(false);


            UpdateLinkAmmo();
        }

        public override void OnStartDismounting()
        {
            base.OnStartDismounting();
            riderCombat.Aim_Set(false);                                              //Set the Action to None in case it was aiming
            v_ShooterManager.useLeftIK = true;                                      //Enable the IK on the Left Hand
            v_ShooterManager.useRightIK = true;                                         //Disable the IK on the Left Hand
            Animator_on_Bow(true);
        }

        public override void OnEndDismounting()
        {
            base.OnEndDismounting();
            ShooterInput.ignoreIK = false;
            ShooterInput.aimInput.useInput = true; //Enable back the Aiming.
            ShooterInput.shotInput.useInput = true; //Enable back the shooting.


            if (!IsCompatible)
            {
                Weapon.SetActive(true);
            }
        }

        /// <summary>Invoked by RiderCombat.OnAimSide so change the camera AimMode </summary>
        /// <param name="side"> 0: Ride State, 1:AimRight, 2: AimLeft</param>
        public virtual void OnAimSide(int side)
        {
            //   Debug.Log("aiming = " + side);
            ShooterInput.controlAimCanvas.SetActiveAim(true);

            switch (side)
            {
                case 0:
                    vCamera.ChangeState(ride);
                    break;
                case 1:
                    vCamera.ChangeState(AimRight);
                    break;
                case -1:
                    vCamera.ChangeState(AimLeft);
                    break;
                default:
                    break;
            }
        }

        public override void Equip_from_Instatiate_RightHand(GameObject weapon)
        {
            base.Equip_from_Instatiate_RightHand(weapon);
            UpdateLinkAmmo();                                                           //Update ammo  for the IGun if there's A weapon on the Hand
        }

        public override void Equip_from_Instatiate_LeftHand(GameObject weapon)
        {
            base.Equip_from_Instatiate_LeftHand(weapon);
            UpdateLinkAmmo();                                   //Update ammo  for the IGun if there's A weapon on the Hand

            if (Rider.IsRiding) Animator_on_Bow(false);        //Disable the Bow Animator in order to the HAP BOW to be used
        }

        public override void OnEquipFromInventory(vEquipArea equipArea, vItem item)
        {
            base.OnEquipFromInventory(equipArea, item);
            UpdateLinkAmmo();                                                           //Update ammo  for the IGun if there's A weapon on the Hand
        }

        /// <summary>Disable/Enable the Animator for the INVECTORS BOW...</summary>
        void Animator_on_Bow(bool value)
        {
            if (Weapon && Weapon.GetComponent<MBow>())
            {
                v_ShooterManager.UpdateTotalAmmo();
                v_ShooterManager.ReloadWeapon();
                var weaponHasAnimator = Weapon.GetComponent<Animator>();
                if (weaponHasAnimator) weaponHasAnimator.enabled = value; //If the weapons has an animator disable/enable it
            }
        }

        public override void OnWeaponAction(int weaponAction)
        {
            base.OnWeaponAction(weaponAction);

            if (CurrentWeaponAction == WA.Reload)
            {
                //Debug.Log("Reload Weapon WITH THE LINK SCRIPT");
                //Reload the Weapon in the Shooter Manager
                StartCoroutine(AfterReloadTime(Malbers_Gun.m_ReloadTime.Value));
                UpdateLinkAmmo();
            }

            if (CurrentWeaponAction == WA.Fire_Projectile)                          //Means is about to release the proyectile 
            {
                var mBow = Weapon.GetComponent<MBow>();
                if (mBow)
                {
                    V_ShooterWeapon.powerCharge = mBow.ChargedNormalized; //Pass the Bow tension to the power Charge
                    Destroy(mBow.ProjectileInstance);
                }

                OnAttackShooter();
            }
        }

        IEnumerator AfterReloadTime(float reloadtime)
        {
            yield return new WaitForSeconds(reloadtime);
            yield return null; //add extra frame
            UpdateLinkAmmo();

            //INTERRUPT AIMING
            mWeapon.IsReloading = false;
            mWeapon.CheckAim();

            mWeapon.ResetCharge();
        }

        /// <summary> Invoked by the Rider Combat.OnAttack </summary>
        public void OnAttackShooter()
        {
            if (inventoryIsOpen) return;    //Do NOTHING  if the Inventory is Open

            if (V_ShooterWeapon)                                                              //If the Equiped weapon is a ShooterWeapon Shoot!
            {
                //  Debug.Log("SHOOOT WITH INVECTOR!");

                var camT = vCamera.transform;
                Vector3 aimPosition = camT.position + camT.forward * 100f;
                //var ShooterMI = VInput as vShooterMeleeInput;


                if (v_ShooterManager.raycastAimTarget && V_ShooterWeapon.raycastAimTarget)
                {
                    var vOrigin = vCamera.transform.position;

                    Ray ray = new(vOrigin, vCamera.transform.forward);

                    if (Physics.Raycast(ray, out RaycastHit hit, Camera.main.farClipPlane, v_ShooterManager.damageLayer)) //This code is from Invector since they shoot differntly 
                    {
                        if (hit.collider.transform.IsChildOf(transform)) //If I hit myself
                        {
                            var collider = hit.collider;
                            var hits = Physics.RaycastAll(ray, Camera.main.farClipPlane, v_ShooterManager.damageLayer);
                            var dist = Camera.main.farClipPlane;
                            for (int i = 0; i < hits.Length; i++)
                            {
                                if (hits[i].distance < dist && hits[i].collider.gameObject != collider.gameObject && !hits[i].collider.transform.IsChildOf(transform))
                                {
                                    dist = hits[i].distance;
                                    hit = hits[i];
                                }
                            }
                        }

                        if (hit.collider) aimPosition = hit.point;
                    }

                    Debug.DrawLine(vOrigin, aimPosition, Color.blue, 2);

                }
                else         //For the Pistol!!!!
                {
                    Ray ray = new(V_ShooterWeapon.aimReference.position, V_ShooterWeapon.aimReference.forward);
                    if (Physics.Raycast(ray, out RaycastHit hit, 100f, v_ShooterManager.damageLayer))
                    {
                        aimPosition = hit.point;
                    }
                    Debug.DrawLine(V_ShooterWeapon.aimReference.position, aimPosition, Color.red, 2);
                }


                v_ShooterManager.Shoot(aimPosition, false);                            //SHOOT WITH INVECTOR INSTEAD OF MALBERS!!! FALSE TO HIP DISPERSION!!!

                if (Malbers_Bow) v_ShooterManager.ReloadWeapon();
                UpdateLinkAmmo();                                                     //Update ammo for the IGun if there's A weapon on the Hand
                return;
            }
        }

        /// <summary>This will link the Invector ShooterWeapon and HAP IGUN Values and is Invoked by the Shooter Weapon</summary>
        public void UpdateLinkAmmo()
        {
            if (Weapon && V_ShooterWeapon)
            {
                TotalAmmo = v_ShooterManager.WeaponAmmo(V_ShooterWeapon).count;

                // Debug.Log("Total Ammo: " + TotalAmmo);

                if (V_ShooterWeapon && Malbers_Gun != null)
                {
                    Malbers_Gun.TotalAmmo = TotalAmmo;                                                      //Total Ammo of that Gun
                    Malbers_Gun.AmmoInChamber = V_ShooterWeapon.ammoCount + 1;                                    //Total Ammo in the Chamber  
                    Malbers_Gun.ChamberSize = V_ShooterWeapon.clipSize;
                }

                if (Malbers_Bow)
                {
                    //  Debug.Log("BOW: " + V_ShooterWeapon.clipSize);
                    Malbers_Bow.AmmoInChamber = 1;
                    Malbers_Bow.Enabled = v_ShooterManager.WeaponAmmo(V_ShooterWeapon).count > 0;
                }
            }
            else
            {
                TotalAmmo = 0;
            }
        }

        void LateUpdate()
        {
            if (Rider && Rider.IsMountingDismounting)
            {
                NoStaminaLockCombat();
                v_ShooterManager.UpdateTotalAmmo();

                if (riderCombat.Aim)
                {
                    ShooterInput.ignoreIK = true;
                    ShooterInput.controlAimCanvas.SetActiveAim(true); //need to Enable the Aim Values.
                }
            }
        }

        /// <summary>Lock Combat if there's no Stamina</summary>
        protected override void NoStaminaLockCombat()
        {
            if (riderCombat.CombatMode && riderCombat.Weapon)
            {
                base.NoStaminaLockCombat();
                if (riderCombat.Weapon.ID != 1) riderCombat.Active = true;
            }
        }
    }
}
