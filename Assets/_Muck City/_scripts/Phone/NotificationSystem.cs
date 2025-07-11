using System;
using System.Threading.Tasks;
using DG.Tweening;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

public class NotificationSystem : MonoBehaviour
{
    public Transform _notificationCanvas;
    public TextMeshProUGUI _notificationTitle;
    public TextMeshProUGUI _notificationContent;

    [TabGroup("Settings")]
    [SerializeField] float _alertShownXPos = 0f;

    [TabGroup("Settings")]
    [SerializeField] float _alertHiddenXPos = 1000f;


    // void Start()
    // {
    //     _notificationCanvas.transform.position = new Vector3(_alertHiddenXPos, _notificationCanvas.transform.position.y, _notificationCanvas.transform.position.z);
    // }

    public void ToggleBackButton()
    {

    }

    public async void DelayedInvoke(Action callback, int delay)
    {
        await Task.Delay(delay * 1000);
        callback.Invoke();
    }

    public void ShowNotification(string title, string content, Action callback = null)
    {
        _notificationTitle.text = title;
        _notificationContent.text = content;
        _notificationCanvas.gameObject.SetActive(true);
        _notificationCanvas.gameObject.transform.DOScale(Vector3.one, 1f);
        DelayedInvoke(() => { HideNotification(); callback?.Invoke(); }, 3);
    }

    public void HideNotification()
    {
        _notificationCanvas.gameObject.transform.DOScale(Vector3.zero, 1f).OnComplete(() => _notificationCanvas.gameObject.SetActive(false));
    }
}

