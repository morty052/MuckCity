using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityUtils;

public class WeaponsCache : Browsable
{
    [SerializeField] Transform LongGunSlot, LongGunSlot2, ShortGunSlot, ShortGunSlot2;


    // void ReplaceEquippedGun(Gun gun)
    // {
    //     if (gun._gunLength == Gun.GunLength.LONG)
    //     {
    //         Gun longGun = Player.Instance.GetComponent<GunSelector>()._longGun;
    //         if (longGun != null)
    //         {
    //             longGun.transform.SetParent(LongGunSlot);
    //             longGun.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
    //         }
    //     }

    //     else
    //     {
    //         Gun shortGun = Player.Instance.GetComponent<GunSelector>()._shortGun;
    //         shortGun.transform.SetParent(LongGunSlot);
    //         shortGun.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
    //     }
    // }
    public override void Exit()
    {
        _activeItemIndex = 0;
    }
    public override void Select()
    {
        // Gun gun = _inventory[_activeItemIndex].GetComponentInChildren<Gun>();
        // Player.Instance.GetComponent<GunSelector>().AddGunToPlayerInventory(gun);
    }
}
