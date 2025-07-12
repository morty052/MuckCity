using System;
using System.Threading.Tasks;
using DG.Tweening;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class NotificationSystem : MonoBehaviour
{
    public Transform _notificationCanvas;
    public Transform _notificationObject;
    public Transform _promptObject;
    public Image _promptButton;
    public TextMeshProUGUI _notificationTitle;
    public TextMeshProUGUI _notificationContent;
    public TextMeshProUGUI _promptText;


    public async void DelayedInvoke(Action callback, int delay)
    {
        await Task.Delay(delay * 1000);
        callback.Invoke();
    }

    public void ShowNotification(string title, string content, string promptText = null, Action callback = null)
    {
        //* ACTIVATE NOTIFICATION CANVAS
        _notificationCanvas.gameObject.SetActive(true);

        //*UPDATE NOTIFICATION TITLEs
        _notificationTitle.text = title;

        //* UPDATE NOTIFICATION CONTENT
        _notificationContent.text = content;

        //* SHOW PROMPT WITH NOTIFICATION IF AVAILABLE
        if (promptText != null)
        {
            _promptText.text = promptText;
            _promptObject.gameObject.SetActive(true);
        }

        //* SHOW MAIN NOTIFICATION OBJECT
        _notificationObject.gameObject.SetActive(true);
        //* SCALE IN
        _notificationObject.gameObject.transform.DOScale(Vector3.one, 0.3f);
        //* SCALE OUT AFTER DELAY AND CALL CALLBACK IF ANY
        DelayedInvoke(() => { HideNotification(); callback?.Invoke(); }, 3);
    }

    public void HideNotification()
    {
        _notificationObject.gameObject.transform.DOScale(Vector3.zero, 0.3f).OnComplete(() =>
        {
            if (_promptObject.gameObject.activeSelf)
            {
                _promptObject.gameObject.SetActive(false);
            }
            _notificationObject.gameObject.SetActive(false);
        });
    }
}

