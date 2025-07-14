using System;
using System.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;



public class NotificationSystem : MonoBehaviour
{
    public Transform _notificationCanvas;
    public Notification _notificationObject;

    public async void DelayedInvoke(Action callback, int delay)
    {
        await Task.Delay(delay * 1000);
        callback.Invoke();
    }

    public void ShowNotification(Sprite icon, string content, string promptText = null, Action callback = null)
    {
        //* ACTIVATE NOTIFICATION CANVAS
        _notificationCanvas.gameObject.SetActive(true);

        //* UPDATE NOTIFICATION DETAILS
        _notificationObject.SetNotification(icon, content, promptText);

        //* SCALE OUT AFTER DELAY AND CALL CALLBACK IF ANY
        DelayedInvoke(() => { HideNotification(); callback?.Invoke(); }, 3);
    }


    public void HideNotification()
    {
        //* STOPS EXTERNAL CANVAS FROM BEING HIDDEN WHEN NOTIFICATION IS CLOSED BEFORE ITS REGULAR SCREEN TIME
        if (!_notificationObject.gameObject.activeSelf) return;
        _notificationObject.HideNotification(() => _notificationCanvas.gameObject.SetActive(false));
    }
}

