using Invector.vCharacterController;
using Invector.vItemManager;
using Invector.vMelee;
using MalbersAnimations;
using MalbersAnimations.Weapons;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

/// <summary>  This is the link between Invector and HAP </summary>
namespace Invector.CharacterController
{
    public class InvectorHAPLinkMelee : InvectorHAPLink
    {
        [HideInInspector] protected MWeaponManager riderCombat;                              //Reference for the RiderCombat
        [HideInInspector] protected vCollectMeleeControl v_CollectMeleeControl;           //Reference for the Invector's vCollectMeleeControl
        [HideInInspector] protected vMeleeManager v_MeleeManager;                         //Reference for the Invector's Melee Manager    
        [HideInInspector] protected vItemManager.vItemManager v_ItemManager;              //Reference for the Invector's Inventory
        public vMeleeCombatInput MeleeInput => VInput as vMeleeCombatInput;

        private EquipPoint equipPointR;
        private EquipPoint equipPointL;

        /// <summary>It allows the rider use weapons while Riding</summary>
        [Space, Tooltip("It allows the rider use weapons while Riding")]
        public bool AllowWeaponsRiding = true;
        public bool DontHitMount = true;


        [Header("Enable/Disable Invector Inputs On Mount")]
        public bool m_crouchInput = false; //No need to Crouch when Riding
        public bool m_strafeInput = false; //No need to strafe when Riding
        public bool m_jumpInput = false; //No need to Jump when Riding
        public bool m_rollInput = false; //No need to roll when Riding
        public bool m_sprintInput = false; //No need to roll when Riding 

        [Header("Enable/Disable Invector Inputs on Dismount")]
        public bool crouchInput = true; //No need to Crouch when Riding
        public bool strafeInput = true; //No need to strafe when Riding
        public bool jumpInput = true; //No need to Jump when Riding
        public bool rollInput = true; //No need to roll when Riding
        public bool sprintInput = true; //No need to roll when Riding 



        /// <summary>Let everybody knows that the Rider mount the animal with a weapon equipped</summary>
        [Header("Events")]
        public UnityEvent OnMountWithWeapon = new UnityEvent();


        public GameObject weapon { get; protected set; }                      //Current Active instantiated gameobject Weapon (Only Right Hand Weapon)
        public MWeapon mWeapon { get; protected set; }
        private vEquipArea ActiveEquipArea;
        private vItem EquippedItem;
        public bool IsCompatible { get; protected set; }
        /// <summary>Is the Invector equipped weapon compatible with HAP?</summary>
        protected bool inventoryIsOpen;                      //State  of the inventory
        protected int CurrentWeaponAction;                    //Well that...

        /// <summary> Current Invector Equipped Weapon Instance on the scene </summary>
        protected virtual GameObject Weapon
        {
            get => weapon;
            set
            {
                weapon = value;
                mWeapon = value ? value.GetComponent<MWeapon>() : null;  //catch the MWeapon
                IsCompatible = mWeapon != null;
            }
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            if (riderCombat)
            {
                riderCombat.OnWeaponAction.AddListener(OnWeaponAction);
                riderCombat.OnEquipWeapon.AddListener(RiderEquipWeapon);
            }

            if (v_ItemManager)
            {
                v_ItemManager.onEquipItem.AddListener(OnEquipFromInventory);                    //Listen to Equip from Inventory
                v_ItemManager.onUnequipItem.AddListener(OnUnEquipFromInventory);                //Listen to Unequip from Inventory     
                v_ItemManager.onOpenCloseInventory.AddListener(onOpenCloseInventory);
            }

            equipPointR?.onInstantiateEquiment.AddListener(Equip_from_Instatiate_RightHand);      //Add Listener for the Right Hand which is the ones that holds the Melee Weapons
            equipPointL?.onInstantiateEquiment.AddListener(Equip_from_Instatiate_LeftHand);      //Add Listener for the Right Hand which is the ones that holds the Melee Weapons
        }

        /// <summary>
        /// Set the Correct Position of the Weapon when its Set Active by Riders Weapon Manager
        /// </summary>
        private void RiderEquipWeapon(GameObject _)
        {
            _.SetActive(false);
            //var Weapon = riderCombat.Weapon;
            //Weapon.transform.parent = Weapon.IsRightHanded ? riderCombat.RightHandEquipPoint : riderCombat.LeftHandEquipPoint;
            //Weapon.transform.localPosition = Weapon.PositionOffset;           //Set the Correct Position
            //Weapon.transform.localEulerAngles = Weapon.RotationOffset;        //Set the Correct Rotation
        }


        ///──────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────
        /// <summary>Remove All Listeners of all subscribed events </summary>
        protected override void OnDisable()
        {
            base.OnDisable();

            if (riderCombat != null)
            {
                riderCombat.OnWeaponAction.RemoveListener(OnWeaponAction);
                riderCombat.OnEquipWeapon.RemoveListener(RiderEquipWeapon);
            }

            if (v_ItemManager)
            {
                v_ItemManager.onEquipItem.RemoveListener(OnEquipFromInventory);
                v_ItemManager.onUnequipItem.RemoveListener(OnUnEquipFromInventory);
            }

            //Add Listener for the Right Hand which is the ones that holds the Melee Weapons
            equipPointR?.onInstantiateEquiment.RemoveListener(Equip_from_Instatiate_RightHand);

            //Add Listener for the Right Hand which is the ones that holds the Melee Weapons
            equipPointL?.onInstantiateEquiment.RemoveListener(Equip_from_Instatiate_LeftHand);
        }



        protected override void InitializeLink()
        {
            base.InitializeLink();

            v_MeleeManager = GetComponent<vMeleeManager>();
            riderCombat = GetComponent<MWeaponManager>();                     //Get the Rider Combat
            v_ItemManager = GetComponent<vItemManager.vItemManager>();                  //Get the Invector's Inventory
            v_CollectMeleeControl = GetComponent<vCollectMeleeControl>();  //Get the Invector's vCollectMeleeControl


            if (v_ItemManager) //If it has an Inventory
            {
                equipPointR = v_ItemManager.equipPoints.Find(p => p.equipPointName == "RightArm");
                equipPointL = v_ItemManager.equipPoints.Find(p => p.equipPointName == "LeftArm");
            }
        }

        #region Event Listeners Rider
        public override void OnStartMounting()
        {
            base.OnStartMounting();

            var drawHide = this.FindComponent<vDrawHideMeleeWeapons>();               //IMPORTANT!  DRAW HIDE WEAPONS HIDE IT AUTOMATICALLY 

            if (drawHide)
            {
                drawHide.hideWeaponsAutomatically = false;
                drawHide.enabled = false;
                drawHide.StopAllCoroutines();
            }

            //If is NOT using the Inventory System  Lock the 'Drop Weapon' (Left Right Key) from the Collect System
            if (v_CollectMeleeControl)
            {
                v_CollectMeleeControl.unequipLeftInput.useInput = false;
                v_CollectMeleeControl.unequipRightInput.useInput = false;
            }

            if (v_MeleeManager)
            {
                if (v_MeleeManager.rightWeapon)
                {
                    Weapon = v_MeleeManager.rightWeapon.gameObject;         //Use the weapon on the Right Hand
                }
                else if (v_MeleeManager.leftWeapon && v_MeleeManager.leftWeapon.meleeType != vMeleeType.OnlyDefense) //Make sure is not the Shield
                {
                    Weapon = v_MeleeManager.leftWeapon.gameObject;         //Use the weapon on the left Hand
                }
            }

            if (Weapon)                                           //Update the weapons for the Shooter   
            {
                if (!IsCompatible || !AllowWeaponsRiding)
                {
                    Weapon.SetActive(false);                                    //Hide the Weapon if is not compatible
                }
                else
                {

                    riderCombat.UnEquip_Fast(); //Clean the weapon first
                    riderCombat.Equip_Fast(Weapon);        //If do has the ImWeapon Interface LetKnow the RiderCombat that can Use the weapon
                    OnMountWithWeapon.Invoke();                         //Let everybondy knows that is mounting with a weapon on the Hand    

                    if (DontHitMount)
                        mWeapon.IgnoreTransform = Rider.Montura.transform; //Don't hit the horse ;
                }
            }

            //Hide the Left Weapon if is a shield
            if (Is_Left_WeaponShield()) v_MeleeManager.leftWeapon.gameObject.SetActive(false);

            if (!AllowWeaponsRiding)
            {
                MeleeInput.SetLockMeleeInput(true);
                riderCombat.Active = false;
            }

            MeleeInput.crouchInput.useInput = m_crouchInput; //No need to Crouch when Riding
            MeleeInput.strafeInput.useInput = m_strafeInput; //No need to strafe when Riding
            MeleeInput.jumpInput.useInput = m_jumpInput; //No need to Jump when Riding
            MeleeInput.rollInput.useInput = m_rollInput; //No need to roll when Riding
            MeleeInput.sprintInput.useInput = m_sprintInput; //No need to roll when Riding 
        }

        public override void OnStartDismounting()
        {
            base.OnStartDismounting();

            transform.rotation = Quaternion.FromToRotation(transform.up, Vector3.up) * transform.rotation;      //Double check for tilting characters

            if (riderCombat)
            {
                if (riderCombat.Weapon)
                {
                    var WOwner = riderCombat.Weapon.Owner;
                    riderCombat.ResetCombat();                                   //Make sure that the Rider Combat is Reseted ... False means that you can keep the Active weapon after dismounting
                    riderCombat.Weapon.Owner = WOwner;
                }

                if (riderCombat.UseHolsters && !riderCombat.CombatMode && Weapon)        //If the Weapon is Stored and you're using holder and there's an Active Weapon 
                    riderCombat.Draw_Weapon();                                           //Draw the weapon on dismounting
            }
            vCamera.RemoveLockTarget();                                                 //if the camera was on LockonTarget Unlock it
        }

        public override void OnEndDismounting()
        {
            base.OnEndDismounting();

            var drawHide = this.FindComponent<vDrawHideMeleeWeapons>();                       //IMPORTANT!  DRAW HIDE WEAPONS HIDE IT AUTOMATICALLY 
            if (drawHide) drawHide.hideWeaponsAutomatically = true;

            if (!AllowWeaponsRiding)
            {
                MeleeInput.SetLockMeleeInput(false);
                riderCombat.Active = false;
            }

            if (mWeapon) mWeapon.IgnoreTransform = null; //Don't hit the horse ;

            //If is NOT using the Inventory System..  Restore the Drop Weapon from the Collect System
            if (v_CollectMeleeControl)
            {
                v_CollectMeleeControl.unequipLeftInput.useInput = true;
                v_CollectMeleeControl.unequipRightInput.useInput = true;
            }

            if (v_MeleeManager && v_MeleeManager.rightWeapon)
                v_MeleeManager.rightWeapon.gameObject.SetActive(true);                              //Show Right Weapon

            if (v_MeleeManager && v_MeleeManager.leftWeapon)
                v_MeleeManager.leftWeapon.gameObject.SetActive(true);                               //Show Left Weapon


            MeleeInput.crouchInput.useInput = crouchInput; //Reset Crouch when not Riding
            MeleeInput.strafeInput.useInput = strafeInput; //Reset strafe when not Riding
            MeleeInput.jumpInput.useInput = jumpInput; // Reset Jump when not Riding
            MeleeInput.rollInput.useInput = rollInput; //Reset roll when not Riding
            MeleeInput.sprintInput.useInput = sprintInput; //Reset sprint when not Riding
        }
        #endregion

        ///──────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────
        /// <summary>This will be invoked from the Event vItemManager.onEquipItem (TO EQUIP THE PREFAB)</summary>
        public virtual void OnEquipFromInventory(vEquipArea equipArea, vItem item)
        {
            if (debug) Debug.Log($"OnEquipFromInventory: [{item.name}] - [OriginalObject.isPrefab? " +
                $"{(item.originalObject != null && item.originalObject.IsPrefab())}]", item.originalObject);

            //If the item equiped is not in the correct Area
            if (item != equipArea.currentEquippedItem) return;

            vMeleeWeapon Item_Melee =
                item && item.originalObject ? item.originalObject.GetComponent<vMeleeWeapon>() : null;
            if (Item_Melee && Item_Melee.meleeType == vMeleeType.OnlyDefense) return;               //If the Weapon is a Shield skip

            if (v_ItemManager.equipments.ContainsKey(item))
            {
                Weapon = v_ItemManager.equipments[item].gameObject; //Get the weapon
            }


            //Only Change Stuff if is Riding
            if (Rider.IsRiding)
            {
                if (!IsCompatible || !AllowWeaponsRiding)
                {
                    //If there's no item OR the Next item is NOT compatible The next Rider Weapon is NULL   
                    //  riderCombat.SmoothEquip = false;
                    riderCombat.Equip_External((MWeapon)null);

                    Weapon = null;
                    if (debug) Debug.Log("If there's no item OR the Next item is NOT compatible The next Rider Weapon is NULL");
                }
                else
                {
                    riderCombat.Unequip_Weapon();
                    riderCombat.Equip_External(Weapon);

                    //var PreWeapon = (item && item.originalObject) ? item.originalObject.GetComponent<MWeapon>() : null;
                    //riderCombat.Draw_Weapon(PreWeapon.HolsterAnim, PreWeapon.WeaponType, PreWeapon.IsRightHanded);  //use the DrawWeapons Animations

                    if (debug) Debug.Log("Set the next weapon: [" + item.originalObject.name + "] as the Item Original Object ", item.originalObject);
                }
            }

            ActiveEquipArea = equipArea;
            EquippedItem = item;
        }

        public virtual void OnUnEquipFromInventory(vEquipArea equipArea, vItem item)
        {
            if (!Rider.IsRiding) return;                                        //Do nothing if the rider is not on the horse    
            if (!riderCombat.CombatMode) return;                                // Do not Unequip any weapon if the rider is not on Combat Mode

            if (ActiveEquipArea == equipArea && item == EquippedItem)                //is trying to Unequip the Equiped Item
            {
                if (debug) Debug.Log("Unequip Same Weapon on the Equip Area");
                // riderCombat.SmoothEquip = false;
                riderCombat.Equip_External((MWeapon)null);
                EquippedItem = null;
                Weapon.SetActive(false);
                Weapon = null;
            }
        }

        ///──────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────
        /// <summary> Link from the Events of the VItem Manager Equip Points (THIS REPLACES THE PREFAB FOR THE INSTANCE) </summary>
        public virtual void Equip_from_Instatiate_RightHand(GameObject weapon)
        {
            Weapon = weapon;
            if (weapon == null) return;

            if (IsCompatible)
            {
                mWeapon.IsEquiped = true;
                mWeapon.IsReloading = false;
            }
            if (!Rider.IsRiding) return;                                        //Do nothing if the rider is not on the horse    

            if (debug) Debug.Log($"Equip_from_Instatiate_RightHand: [{weapon.name}]", weapon);

            if (!AllowWeaponsRiding)
            {
                Weapon.SetActive(false);
                return;
            }


            if (IsCompatible)
            {
                mWeapon.Owner = gameObject;
                mWeapon.IgnoreTransform = Rider.Montura.transform; //Don't hit the horse ;
                riderCombat.Equip_Fast(mWeapon);
                Weapon.SetActive(true); //Reactivate the weapon

            }
            else
                Weapon?.SetActive(false);
        }


        ///──────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────
        /// <summary>Link from the Events of the VItem Manager Equip Points</summary>
        public virtual void Equip_from_Instatiate_LeftHand(GameObject weapon)
        {
            if (debug) Debug.Log($"Equip_from_Instatiate_LeftHand: [{weapon.name}]", weapon);

            Weapon = weapon;

            if (weapon == null) return;

            if (IsCompatible) mWeapon.IsEquiped = true;

            if (!Rider.IsRiding) return;                                        //Do nothing if the rider is not on the horse    

            if (!AllowWeaponsRiding)
            {
                Weapon.SetActive(false);
                return;
            }


            if (IsCompatible)
            {
                riderCombat.Weapon = mWeapon; //IMPORTANT!!!! UPDATE TO THE INSTANTIATED VERSION OF THE WEAPON
                Weapon.SetActive(true); //Reactivate the weapon
            }

            var meleeWeapon = weapon.GetComponent<vMeleeWeapon>();

            if (meleeWeapon && meleeWeapon.meleeType == vMeleeType.OnlyDefense) //If the Weapon on the Left Hand is A Shield Ignore it!!
            {
                weapon.SetActive(false);
            }
        }

        ///──────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────
        /// <summary>Update the current Weapon Action</summary>
        public virtual void OnWeaponAction(int weaponAction)
        {
            CurrentWeaponAction = weaponAction;                   //Update the Current Rider WeaponAction

            if (Weapon && Rider.IsRiding)
            {
                if (!IsCompatible)
                {
                    Weapon.SetActive(false); //Disable it while riding
                }
                else if (riderCombat.UseExternal)
                {
                    if (CurrentWeaponAction == WA.Idle)                                               //Update in the Rider Combat the Active weapon game Object
                    {
                        Weapon.SetActive(true);
                    }
                }
            }
        }

        IEnumerator DisableAttack(float t, vMeleeCombatInput input)
        {
            if (t > 0)
            {
                yield return new WaitForSeconds(t);
                input.OnDisableAttack();
            }
        }


        ///──────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────
        /// <summary>Keep the Input Locked in case the inventory is closed </summary>
        public virtual void onOpenCloseInventory(bool open)
        {
            inventoryIsOpen = open;

            //  riderCombat.Active = !open; //Lock the Rider combat if the inventory is open

            if (Rider.IsRiding)
            {
                if (!open)
                {
                    VInput.SetLockBasicInput(true);                                     //Double check that the Invector's Input is Locked
                    vCamera.ChangeState(ride);                                          //Double Check the camera in the ride state
                    //Rider.Anim.SetFloat(Rider.Montura.Animal.hash_SpeedMultiplier,1);   //Play the Locomotion Animation when the Inventory is Closed

                }
                else
                {
                    Rider.Montura.Animal.StopMoving();
                    // Rider.Anim.SetFloat(Rider.Montura.Animal.hash_SpeedMultiplier,0); //Stop the Locomotion Animation when the Inventory is Open
                    Rider.Montura.Animal.MovementAxisSmoothed = Vector3.zero; //STOP THE HORSE WHEN THE INVECTORY IS OPEN
                }
            }
        }

        void Update()
        {
            if (Rider)
            {
                if (!Rider.IsRiding) return;                //Skip all Below if is not Riding
                LockOnTarget();
                NoStaminaLockCombat();                      //Lock Rider Combat when there's no Stamina
            }
        }


        ///──────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────
        /// <summary>Lock On Target Activation </summary>
        public virtual void LockOnTarget()
        {
            if (MeleeInput && MeleeInput.lockInput)               //Keep the Lock on Target Input Enabled
            {
                MeleeInput.SetLockBasicInput(true);
                riderCombat.Aimer.AimTarget = vCamera.lockTarget;
            }
        }

        ///──────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────
        /// <summary>Lock Combat if there's no Stamina </summary>
        protected virtual void NoStaminaLockCombat()
        {
            if (riderCombat.CombatMode && v_MeleeManager != null)
            {
                bool lockRiderCombat = (v_TCP.currentStamina < v_MeleeManager.GetAttackStaminaCost());

                if (riderCombat.Active == lockRiderCombat) riderCombat.Active = !lockRiderCombat;
            }
        }



        public bool Is_Left_WeaponShield()
        {
            if (v_MeleeManager && v_MeleeManager.leftWeapon && v_MeleeManager.leftWeapon.meleeType == vMeleeType.OnlyDefense) return true;
            return false;
        }

        /// <summary> is the current gameobject  Prefab <returns></returns>
        public bool IsPrefab(GameObject go)
        {
            if (go == null) return false;
            return go.activeInHierarchy;
        }
    }
}
