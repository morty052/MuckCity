using System.Collections.Generic;
using UnityEngine;

public class SoundClipPlayer : MonoBehaviour
{
    [SerializeField] List<AudioClip> clips;
    public void PlayClip(ShopItemSO shopItemSO)
    {
        Debug.Log("PlayClip for feel here" + shopItemSO.name);
    }
}
