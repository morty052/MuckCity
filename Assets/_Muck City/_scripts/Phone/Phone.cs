using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DG.Tweening;
using Invector.vCharacterController;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public enum PhonePosition
{
    RIGHT,
    CENTER,
    LANDSCAPE
}

public enum AppNames
{
    DELIVERY_APP = 0,
    MESSENGER_APP = 1,
}

public class Phone : SpecialEquipment, IBrowsable
{
    public static Phone Instance { get; private set; }

    // [TabGroup("Inputs")]
    // public GenericInput _quickRejectInput = new("C", "Y", "Y");

    #region"components"
    [TabGroup("Components")]
    [SerializeField] Transform _homeScreenTransform;

    [TabGroup("Components")]
    [SerializeField] Transform _appScreensParent;
    [TabGroup("Components")]
    [SerializeField] Transform _statusBarTransform;

    [TabGroup("Components")]
    [SerializeField] Image _appIconPrefab;

    [TabGroup("Components")]
    public GameObject _phoneModel;

    [TabGroup("Components")]
    public Camera _phoneCamera;

    [SerializeField, TabGroup("Components")]
    private GameObject _externalCanvas;

    [TabGroup("Components")]
    public StatusBar _statusBar;

    [TabGroup("Components")]
    public ActionPrompt _actionPrompt;

    #endregion

    #region "Settings"
    [TabGroup("Settings")]
    public bool _canQuickAcceptReject = false;

    [TabGroup("Settings")]
    [SerializeField] Pos _centerPos;

    [TabGroup("Settings")]
    [SerializeField] Pos _rightPos;
    #endregion

    #region "Events"
    [TabGroup("Events")]
    public UnityEvent OnPhoneRing;
    [TabGroup("Events")]
    public UnityEvent OnPhoneRingEnd;
    [TabGroup("Events")]
    public UnityEvent OnPhoneCallStart;
    [TabGroup("Events")]
    public UnityEvent OnPhoneCallEnd;
    [TabGroup("Events")]
    public UnityEvent OnStartInstantMessage;
    [TabGroup("Events")]
    public UnityEvent<Chat> OnReceiveInstantMessage;
    [TabGroup("Events")]
    public UnityEvent<int> OnSendInstantMessage;
    [TabGroup("Events")]
    public UnityEvent OnEndInstantMessage;
    [TabGroup("Events")]
    public UnityEvent OnGenericButtonPress;
    #endregion

    #region "Debug"
    [TabGroup("Debug")]
    public bool _isPhoneActive = false;

    [TabGroup("Debug")]
    public DeliveryData? _currentDelivery;

    [TabGroup("Debug")]
    [SerializeField] PhoneApp _currentApp;

    [TabGroup("Debug")]
    [SerializeField] List<PhoneApp> _installedAppsPrefabs;

    [TabGroup("Debug")]
    [SerializeField] List<PhoneApp> _installedApps;


    [TabGroup("Debug")]
    [SerializeField] private List<PhoneApp> Routes = new();

    [TabGroup("Debug")]
    [SerializeField] private List<AppIcon> _appIcons = new();

    [TabGroup("Debug")]
    [SerializeField] private int _selectedAppIndex = 0;

    [TabGroup("Debug")]
    [SerializeField] private PhoneApp _activeRoute;
    [TabGroup("Debug")]

    [SerializeField, TabGroup("Debug")] private bool _debug;



    #endregion

    [HideInInspector]
    public NotificationSystem _notificationSystem;
    public Action<string> OnInstallApp;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            _statusBar = new(_statusBarTransform);
            _notificationSystem = GetComponent<NotificationSystem>();
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }


    void Start()
    {
        SetupAppsAndIcons();
        SetPhonePos(PhonePosition.CENTER, false);
    }

    // void OnEnable()
    // {
    //     if (IsAppInstalled<DeliveryApp>())
    //     {
    //         GameEventsManager.OnDeliveryAddedEvent += HandleNewDeliveryAdded;
    //     }
    // }



    // void Update()
    // {
    //     if (_canQuickAcceptReject)
    //     {
    //         if (_quickAcceptInput.GetButtonDown())
    //         {
    //             _canQuickAcceptReject = false;
    //             _notificationSystem.HideNotification();
    //             if (_currentDelivery.Value._chat != null)
    //             {
    //                 OnReceiveInstantMessage?.Invoke(_currentDelivery.Value._chat);
    //                 StartInstantMessage();
    //             }
    //         }

    //         // if (_quickRejectInput.GetButtonDown())
    //         // {

    //         // }
    //     }
    // }

    // public void QuickInspectDelivery()
    // {
    //     _canQuickAcceptReject = false;
    //     _notificationSystem.HideNotification();
    //     if (_currentDelivery.Value._chat != null)
    //     {
    //         OnReceiveInstantMessage?.Invoke(_currentDelivery.Value._chat);
    //         StartInstantMessage();
    //     }
    // }
    void OnDisable()
    {
        for (int i = 0; i < _installedApps.Count; i++)
        {
            _installedApps[i].OnDisablePhone();
        }
    }
    [Button, TabGroup("Debug")]
    public void StartInstantMessage()
    {
        ShowPhone(true);
        OnStartInstantMessage?.Invoke();
    }

    [Button, TabGroup("Debug")]
    void DebugInstantMessage(Chat chat)
    {
        OnReceiveInstantMessage?.Invoke(chat);
        StartInstantMessage();
    }
    public void ReceiveInstantMessage(Chat chat)
    {
        OnReceiveInstantMessage?.Invoke(chat);
        // StartInstantMessage();
    }

    public override void Init()
    {
        transform.SetParent(Player.Instance.transform, false);
        transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
        Player.Instance.UpdatePhone(this);
    }

    [Button, TabGroup("Debug")]
    void InitCall()
    {
        ShowPhone(false);
        SetPhonePos(PhonePosition.RIGHT);
        OnPhoneRing?.Invoke();
    }

    void ShowPhone(bool useBlur)
    {
        Player.Instance?.TogglePhone(useBlur);
    }

    // void AcceptDelivery()
    // {
    //     GameEventsManager.Instance.OnDeliveryAccepted(_currentDelivery.Value);
    // }


    [Button, TabGroup("Debug")]
    public void SetPhonePos(PhonePosition pos, bool hideUi = true)
    {
        if (pos == PhonePosition.RIGHT)
        {
            _phoneModel.transform.SetLocalPositionAndRotation(_rightPos.position, Quaternion.Euler(_rightPos.rotation));
        }
        if (pos == PhonePosition.CENTER)
        {
            _phoneModel.transform.SetLocalPositionAndRotation(_centerPos.position, Quaternion.Euler(_centerPos.rotation));
            if (hideUi)
            {
                HudManager.Instance.OnToggleUi();
            }
        }
    }

    public void ToggleUseInput()
    {
        _isPhoneActive = !_isPhoneActive;
    }


    void UseHomePageNavigation(Inputs input)
    {
        switch (input)
        {
            case Inputs.LEFT:
                _appIcons[_selectedAppIndex].ToggleActive(false);
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
                _appIcons[_selectedAppIndex].ToggleActive(true);
                break;
            case Inputs.RIGHT:
                _appIcons[_selectedAppIndex].ToggleActive(false);
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
                _appIcons[_selectedAppIndex].ToggleActive(true);
                break;
            case Inputs.SELECT:
                SelectApp();
                break;
            case Inputs.BACK:
                GoToHomePage();
                break;
            default:
                break;
        }

        // _selectedApp.ToggleActive(false);
        // _selectedApp = _appIcons[_selectedAppIndex];
        // _selectedApp.ToggleActive(true);
    }

    void RelayInputToCurrentApp(Inputs input)
    {
        switch (input)
        {
            case Inputs.UP:
                _currentApp.OnUpPressed();
                break;
            case Inputs.DOWN:
                _currentApp.OnDownPressed();
                break;
            case Inputs.LEFT:
                _currentApp.OnLeftPressed();
                break;
            case Inputs.RIGHT:
                _currentApp.OnRightPressed();
                break;
            case Inputs.SELECT:
                _currentApp.OnSelectPressed();
                break;
            case Inputs.BACK:
                _currentApp.OnBackPressed();
                break;
            case Inputs.ACCEPT:
                _currentApp.OnAcceptPressed();
                break;
            case Inputs.REJECT:
                _currentApp.OnRejectPressed();
                break;
            default:
                break;
        }
    }

    // private void HandleNewDeliveryAdded(DeliveryData data)
    // {

    //     // DeliveryApp deliveryApp = _installedApps.Find(app => app is DeliveryApp) as DeliveryApp;

    //     if (IsAppInstalled<DeliveryApp>(out PhoneApp deliveryApp))
    //     {
    //         ((DeliveryApp)deliveryApp).HandleNewDelivery(data);
    //     }
    //     else
    //     {
    //         Debug.LogWarning("No DeliveryApp found in installed apps.");
    //     }
    //     _currentDelivery = data;
    //     _canQuickAcceptReject = true;
    //     _notificationSystem.ShowNotification("New order", data._deliveryFee.ToString(), "Reply", ResetDeliveryState);
    // }
    public bool IsAppInstalled<T>(out PhoneApp app) where T : PhoneApp
    {
        app = _installedApps.Find(x => x is T);
        return app != null;
    }
    public bool IsAppInstalled<T>() where T : PhoneApp
    {
        PhoneApp app = _installedApps.Find(x => x is T);
        return app != null;
    }

    private void ResetDeliveryState()
    {
        _canQuickAcceptReject = false;
        _currentDelivery = null;
    }

    [Button, TabGroup("Debug")]
    void SetupAppsAndIcons()
    {
        foreach (PhoneApp app in _installedAppsPrefabs)
        {
            PhoneApp phoneApp;
            if (app._type == AppType.IN_BUILT)
            {
                phoneApp = app;
            }
            else
            {
                phoneApp = Instantiate(app, _appScreensParent);
            }

            //* GET APP ICON
            Sprite sprite = phoneApp.AppIcon.IconSprite;
            //* INSTANTIATE APP ICON
            Image image = Instantiate(_appIconPrefab, _homeScreenTransform);
            image.sprite = sprite;

            //* ADD APP ICON TO APP ICONS LIST
            _appIcons.Add(image.GetComponent<AppIcon>());

            //* MAKE APP ICON OPEN APP ON CLICK
            CreateButton(image, phoneApp.ID);

            //* Allow Phone App To Set itself up even if disabled
            phoneApp.Init(_notificationSystem, _externalCanvas, _actionPrompt);

            RectTransform rectTransform = phoneApp.GetComponent<RectTransform>();


            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.one;
            rectTransform.offsetMin = Vector2.zero;
            rectTransform.offsetMax = Vector2.zero;

            rectTransform.localScale = Vector3.one;
            rectTransform.localRotation = Quaternion.identity;
            rectTransform.localPosition = Vector3.zero;

            //* DISABLE APP MAIN SCREEN TO AVOID OVERLAP
            phoneApp._appMainScreen.gameObject.SetActive(false);
            _installedApps.Add(phoneApp);

            //* ADD APPS TO ROUTES
            Routes.Add(phoneApp);

            //* Highlight first App in list
            _appIcons[_selectedAppIndex].ToggleActive(true);
        }
    }

    [Button, TabGroup("Debug")]
    public void ShowAlert(string content, string promptText = null)
    {
        _notificationSystem.ShowNotification(null, content, promptText, ResetDeliveryState);
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
    public void SetApp(AppNames id, bool showPhone = false)
    {
        //* Exit out of current app if already in app
        _activeRoute?._appMainScreen.gameObject.SetActive(false);
        PhoneApp phoneApp = _installedApps.Find(x => x.ID == id);
        _selectedAppIndex = Routes.IndexOf(phoneApp);
        _activeRoute = Routes[_selectedAppIndex];
        _activeRoute._appMainScreen.gameObject.SetActive(true);
        _currentApp = _installedApps[_selectedAppIndex];

        _statusBar.ToggleBackButton();
        if (_debug)
        {
            Debug.Log($"Selected app: {_currentApp.ID}");
        }

        if (showPhone)
        {
            ShowPhone(true);
        }

    }

    //! EDITOR EVENT FUNCTION
    public void GoBack()
    {
        _currentApp.OnBackPressed();
        if (_currentApp == null)
        {
            _statusBar.ToggleBackButton();
        }
    }
    public void GoToHomePage()
    {
        _activeRoute._appMainScreen.gameObject.SetActive(false);
        _currentApp = null;
        _selectedAppIndex = 0;
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
        OnInstallApp?.Invoke(app.AppName);
    }

    public void OnButtonPress(Inputs input)
    {
        // if (!_isPhoneActive) return;
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

    private void CreateButton(Image image, AppNames phoneAppID)
    {
        Button button = image.GetComponent<Button>();
        button.onClick.AddListener(() => { SetApp(phoneAppID); });
    }

    // public void UseActionPrompt(string promptOne, string promptTwo)
    // {
    //     _externalCanvas.SetActive(true);
    //     _actionPrompt.UseActionPrompt(promptOne, promptTwo);
    //     _actionPrompt.gameObject.SetActive(true);
    // }

    // public void DisposeActionPrompt()
    // {
    //     _externalCanvas.SetActive(false);
    //     _actionPrompt.gameObject.SetActive(false);
    // }
}

public class StatusBar
{
    public Transform _statusbar;
    public GameObject _backButton;
    public StatusBar(Transform statusbar)
    {
        _statusbar = statusbar;
        _backButton = _statusbar.Find("Back Button").gameObject;
    }

    public void ToggleBackButton()
    {
        _backButton.SetActive(!_backButton.activeSelf);
    }
}

