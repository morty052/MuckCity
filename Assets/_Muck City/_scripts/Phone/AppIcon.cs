using UnityEngine;

public class AppIcon : MonoBehaviour
{
    [SerializeField] private GameObject _indicator;
    public Sprite IconSprite;
    public PhoneAppID AppID;

    public void ToggleActive(bool state)
    {
        _indicator.SetActive(state);
    }
}
