using System;
using System.Collections.Generic;
using DG.Tweening;
using Invector.vCharacterController;
using TMPro;
using UnityEngine;
using UnityEngine.UI;



public class Phone : MonoBehaviour
{
    public static Phone Instance { get; private set; }


    [Header("EDITOR VARIABLES")]
    public GenericInput _quickAcceptInput = new("C", "Y", "Y");
    public GenericInput _quickRejectInput = new("C", "Y", "Y");

    [SerializeField] Transform _homeScreenTransform;
    [SerializeField] Transform _appScreensParent;

    [SerializeField] Image _appIconPrefab;

    [SerializeField] private GameObject _newOrderAlert;
    [SerializeField] private TextMeshProUGUI _newOrderDeliveryFeeText;

    [SerializeField] float _alertHiddenXPos = 1000f;
    [SerializeField] float _alertShownXPos = 0f;

    [Header("RUNTIME VARIABLES")]

    public bool _canQuickAcceptReject = false;
    [SerializeField] PhoneApp _currentApp;
    [SerializeField] List<PhoneApp> _installedAppsPrefabs;
    [SerializeField] List<PhoneApp> _installedApps;

    [SerializeField] private List<PhoneApp> Routes = new();
    [SerializeField] private List<AppIcon> _appIcons = new();

    [SerializeField] private int _selectedAppIndex = 0;
    [SerializeField] private PhoneApp _activeRoute;

    [SerializeField] private MessagesScreen _messagesScreen;

    public List<Message> _messages = new();

    public DeliveryData? _currentDelivery;


    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }


    void Start()
    {
        _newOrderAlert.transform.position = new Vector3(_alertHiddenXPos, _newOrderAlert.transform.position.y, _newOrderAlert.transform.position.z);
        SetupAppIcons();
    }



    void OnEnable()
    {
        // GameEventsManager.OnDisplayPhone += OnDisplayPhone;
        // GameEventsManager.OnHidePhone += OnHidePhone;
        // GameEventsManager.OnMessageReceived += HandleNewMessage;
        PhoneNavigation.OnButtonPress += OnPhoneButtonPress;
        GameEventsManager.OnDeliveryAddedEvent += ShowNewDeliveryAlert;
    }

    void OnDisable()
    {
        // GameEventsManager.OnDisplayPhone -= OnDisplayPhone;
        // GameEventsManager.OnHidePhone -= OnHidePhone;
        // GameEventsManager.OnMessageReceived -= HandleNewMessage;
        PhoneNavigation.OnButtonPress -= OnPhoneButtonPress;
        GameEventsManager.OnDeliveryAddedEvent -= ShowNewDeliveryAlert;
    }

    void Update()
    {
        if (_canQuickAcceptReject)
        {
            if (_quickAcceptInput.GetButtonDown())
            {
                _canQuickAcceptReject = false;
                _newOrderAlert.SetActive(false);
                GameEventsManager.Instance.OnDeliveryAccepted(_currentDelivery.Value);
            }

            if (_quickRejectInput.GetButtonDown())
            {

            }
        }
    }

    void OnPhoneButtonPress(PhoneInputs input)
    {
        if (_currentApp == null)
        {
            UseHomePageNavigation(input);
            return;
        }
        else
        {
            RelayInputToCurrentApp(input);
        }
    }


    void UseHomePageNavigation(PhoneInputs input)
    {
        switch (input)
        {
            case PhoneInputs.LEFT:
                // MOVE TO PREVIOUS APP IF CAN GO BACK
                if (_selectedAppIndex > 0)
                {
                    _selectedAppIndex--;
                }
                else
                {
                    //MOVE TO LAST APP IF CANT GO BACK
                    _selectedAppIndex = _appIcons.Count - 1;
                }
                break;
            case PhoneInputs.RIGHT:
                if (_selectedAppIndex == _appIcons.Count - 1)
                {
                    //MOVE TO FIRST APP IF CANT GO FORWARD
                    _selectedAppIndex = 0;
                }

                else
                {
                    //MOVE TO NEXT APP IF CAN GO FORWARD
                    _selectedAppIndex++;
                }
                break;
            case PhoneInputs.SELECT:
                SelectApp();
                break;
            case PhoneInputs.BACK:
                GoToHomePage();
                break;
            default:
                break;
        }

        // _selectedApp.ToggleActive(false);
        // _selectedApp = _appIcons[_selectedAppIndex];
        // _selectedApp.ToggleActive(true);
    }

    void RelayInputToCurrentApp(PhoneInputs input)
    {
        switch (input)
        {
            case PhoneInputs.UP:
                _currentApp.OnUpPressed();
                break;
            case PhoneInputs.DOWN:
                _currentApp.OnDownPressed();
                break;
            case PhoneInputs.LEFT:
                _currentApp.OnLeftPressed();
                break;
            case PhoneInputs.RIGHT:
                _currentApp.OnRightPressed();
                break;
            case PhoneInputs.SELECT:
                _currentApp.OnSelectPressed();
                break;
            case PhoneInputs.BACK:
                _currentApp.OnBackPressed();
                break;
            case PhoneInputs.ACCEPT:
                _currentApp.OnAcceptPressed();
                break;
            case PhoneInputs.REJECT:
                _currentApp.OnRejectPressed();
                break;
            default:
                break;
        }
    }

    private void ShowNewDeliveryAlert(DeliveryData data)
    {
        _currentDelivery = data;
        _newOrderDeliveryFeeText.text = data._deliveryFee.ToString();
        _newOrderAlert.SetActive(true);
        _newOrderAlert.transform.DOMoveX(_alertShownXPos, 0.5f).onComplete = () => { _canQuickAcceptReject = true; };
        Invoke(nameof(HideDeliveryAlert), 3f);

        DeliveryApp deliveryApp = _installedApps.Find(app => app is DeliveryApp) as DeliveryApp;
        if (deliveryApp != null)
        {

            deliveryApp.HandleNewDelivery(data);
        }
        else
        {
            Debug.LogWarning("No DeliveryApp found in installed apps.");
        }
    }

    private void HideDeliveryAlert()
    {
        _newOrderAlert.transform.DOMoveX(_alertHiddenXPos, 0.5f).OnComplete(() =>
        {
            _canQuickAcceptReject = false;
            _newOrderAlert.SetActive(false);
            _currentDelivery = null;
        });
    }

    void SetupAppIcons()
    {
        foreach (PhoneApp app in _installedAppsPrefabs)
        {
            PhoneApp phoneApp = Instantiate(app, _appScreensParent);
            Sprite sprite = phoneApp.AppIcon.IconSprite;
            Image image = Instantiate(_appIconPrefab, _homeScreenTransform);
            image.sprite = sprite;

            // AppScreen appScreen = Instantiate(app._appMainScreen, _appScreensParent);
            // appScreen.transform.SetParent(_appScreensParent);

            RectTransform rectTransform = phoneApp.GetComponent<RectTransform>();


            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.one;
            rectTransform.offsetMin = Vector2.zero;
            rectTransform.offsetMax = Vector2.zero;

            rectTransform.localScale = Vector3.one;
            rectTransform.localRotation = Quaternion.identity;
            rectTransform.localPosition = Vector3.zero;
            phoneApp._appMainScreen.gameObject.SetActive(false);
            _installedApps.Add(phoneApp);
            Routes.Add(phoneApp);
        }
    }


    private void SelectApp()
    {
        if (_activeRoute != null)
        {
            _activeRoute._appMainScreen.gameObject.SetActive(false);
        }
        _activeRoute = Routes[_selectedAppIndex];
        _activeRoute._appMainScreen.gameObject.SetActive(true);
        _currentApp = _installedApps[_selectedAppIndex];
        Debug.Log($"Selected app: {_currentApp.AppName}");
    }

    public void GoToHomePage()
    {
        _activeRoute._appMainScreen.gameObject.SetActive(false);
        _currentApp = null;
        _selectedAppIndex = 0;
        // _activeRoute = Routes[0];
        // _activeRoute.gameObject.SetActive(true);
    }

    private void HandleNewMessage(Message message)
    {
        _messagesScreen.HandleNewMessage(message);
    }

    public void InstallApp(PhoneApp app)
    {
        app.transform.SetParent(_appScreensParent);
        _installedAppsPrefabs.Add(app);
        Sprite sprite = app.AppIcon.IconSprite;
        Image image = Instantiate(_appIconPrefab, _homeScreenTransform);
        image.sprite = sprite;

        // AppScreen appScreen = Instantiate(app._appMainScreen, _appScreensParent);
        // appScreen.transform.SetParent(_appScreensParent);

        RectTransform rectTransform = app.GetComponent<RectTransform>();


        rectTransform.anchorMin = Vector2.zero;
        rectTransform.anchorMax = Vector2.one;
        rectTransform.offsetMin = Vector2.zero;
        rectTransform.offsetMax = Vector2.zero;

        rectTransform.localScale = Vector3.one;
        rectTransform.localRotation = Quaternion.identity;
        rectTransform.localPosition = Vector3.zero;
        app._appMainScreen.gameObject.SetActive(false);
        _installedApps.Add(app);
        Routes.Add(app);
    }

    // void Start()
    // {
    //     _selectedApp = _appIcons[_selectedAppIndex];
    //     _selectedApp.ToggleActive(true);
    //     _activeRoute = Routes[0];
    // }


    // void Update()
    // {
    //     if (Input.GetKeyDown(KeyCode.LeftArrow))
    //     {
    //         UpdateAppSelector(true);

    //     }

    //     if (Input.GetKeyDown(KeyCode.RightArrow))
    //     {
    //         UpdateAppSelector(false);
    //     }

    //     if (Input.GetKeyDown(KeyCode.Mouse0))
    //     {
    //         SelectApp();
    //     }
    //     if (Input.GetKeyDown(KeyCode.Space))
    //     {
    //         GoToHomePage();
    //     }
    // }



    // private void UpdateAppSelector(bool isPrev)
    // {
    //     if (!isPrev)
    //     {
    //         if (_selectedAppIndex == _appIcons.Count - 1)
    //         {
    //             //MOVE TO FIRST APP IF CANT GO FORWARD
    //             _selectedAppIndex = 0;
    //         }

    //         else
    //         {
    //             //MOVE TO NEXT APP IF CAN GO FORWARD
    //             _selectedAppIndex++;
    //         }
    //     }
    //     else
    //     {
    //         // MOVE TO PREVIOUS APP IF CAN GO BACK
    //         if (_selectedAppIndex > 0)
    //         {
    //             _selectedAppIndex--;
    //         }
    //         else
    //         {
    //             //MOVE TO LAST APP IF CANT GO BACK
    //             _selectedAppIndex = _appIcons.Count - 1;
    //         }
    //     }

    //     _selectedApp.ToggleActive(false);
    //     _selectedApp = _appIcons[_selectedAppIndex];
    //     _selectedApp.ToggleActive(true);

    // }




}
