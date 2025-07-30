using System;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class TicketValidator : Interactable
{

    [SerializeField] private GameObject _defaultScreen;
    [SerializeField] private GameObject _noTicketScreen;
    [SerializeField] private GameObject _scanningTicketScreen;
    [SerializeField] private GameObject _verifiedTicketUi;
    [SerializeField] private Image _scanningTicketImage;
    [SerializeField] private TextMeshProUGUI _scanningTicketText;
    [SerializeField] private AudioClip _noTicketsFoundAudio;

    [SerializeField] UnityEvent OnValidateSuccessful;
    [SerializeField] UnityEvent OnValidateFail;

    bool IsShowingAlert => _noTicketScreen.gameObject.activeSelf;

    public override void Start()
    {
        base.Start();
        _scanningTicketImage.fillAmount = 0f;
        _scanningTicketScreen.SetActive(false);
        _verifiedTicketUi.SetActive(false);
        _verifiedTicketUi.transform.localScale = Vector3.zero;
        _scanningTicketScreen.transform.localScale = Vector3.zero;
        _noTicketScreen.transform.localScale = Vector3.zero;
    }

    public override void Interact()
    {
        if (!DelveManager.Instance.PlayerHasDelveTicket())
        {
            Debug.Log("No Ticket");
            if (!IsShowingAlert)
            {
                OnValidateFail?.Invoke();
                AlertNoTicket();
            }
            return;
        }
        HideInteractionPrompt();
        StartVerification();
    }

    void StartVerification()
    {
        ABUtils.ScaleOut(_defaultScreen.transform, () =>
               {
                   _scanningTicketScreen.SetActive(true);
                   ABUtils.ScaleIn(_scanningTicketScreen.transform, () =>
                   {
                       _defaultScreen.SetActive(false);
                       Invoke(nameof(Verify), 0.3f);
                   });
               });
    }

    void AlertNoTicket()
    {
        ABUtils.ScaleOut(_defaultScreen.transform, () =>
        {
            _noTicketScreen.SetActive(true);
            ABUtils.ScaleIn(_noTicketScreen.transform);
            GetComponent<SoundClipPlayer>().PlayClip("NO_TICKETS_FOUND", 0.6f);
        });
        Invoke(nameof(ClearAlert), 5.544f);
    }

    void ClearAlert()
    {
        ABUtils.ScaleOut(_noTicketScreen.transform, () =>
                {
                    _noTicketScreen.SetActive(false);
                    ABUtils.ScaleIn(_defaultScreen.transform);
                });
    }

    async void Verify()
    {
        await LerpImageFillAsync(() =>
        {
            OnValidateSuccessful?.Invoke();
            ABUtils.ScaleOut(_scanningTicketScreen.transform, () =>
            {
                _scanningTicketScreen.SetActive(false);
                _verifiedTicketUi.SetActive(true);
                ABUtils.ScaleIn(_verifiedTicketUi.transform, () =>
                {
                    Invoke(nameof(ResetValidator), 0.5f);
                });
            });
        });
    }

    void ResetValidator()
    {
        ABUtils.ScaleOut(_verifiedTicketUi.transform, () =>
        {
            _scanningTicketScreen.SetActive(false);
            _defaultScreen.SetActive(true);
            ABUtils.ScaleIn(_defaultScreen.transform, () =>
            {
                _scanningTicketImage.fillAmount = 0f;
                _scanningTicketScreen.SetActive(false);
                _verifiedTicketUi.SetActive(false);
                _verifiedTicketUi.transform.localScale = Vector3.zero;
                _scanningTicketScreen.transform.localScale = Vector3.zero;
            });
        });


    }
    public async Task LerpImageFillAsync(Action OnImageFilled = null)
    {
        float elapsed = 0f;
        float lerpDuration = 1.5f;

        while (elapsed < lerpDuration)
        {
            elapsed += Time.deltaTime;
            float fillAmount = Mathf.Lerp(0f, 1f, elapsed / lerpDuration);
            _scanningTicketImage.fillAmount = fillAmount;
            await Task.Yield(); // Yield control back to Unity engine
        }

        // Ensure it's fully filled at the end
        _scanningTicketImage.fillAmount = 1f;

        OnImageFilled?.Invoke();
    }
}
