using System;
using System.Collections.Generic;
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


    [Header("EDITOR VARIABLES")]

    [TabGroup("Inputs")]
    public GenericInput _quickAcceptInput = new("C", "Y", "Y");

    [TabGroup("Inputs")]
    public GenericInput _quickRejectInput = new("C", "Y", "Y");

    [TabGroup("Components")]
    [SerializeField] Transform _homeScreenTransform;

    [TabGroup("Components")]
    [SerializeField] Transform _appScreensParent;

    [TabGroup("Components")]
    [SerializeField] Image _appIconPrefab;

    [TabGroup("Components")]
    public GameObject _phoneModel;

    [TabGroup("Components")]
    public Camera _phoneCamera;

    [TabGroup("Components")]
    [SerializeField] private GameObject _newOrderAlert;

    [TabGroup("Components")]
    [SerializeField] private TextMeshProUGUI _newOrderDeliveryFeeText;


    public Action<string> OnInstallApp;
    [TabGroup("Settings")]
    [SerializeField] float _alertHiddenXPos = 1000f;

    [TabGroup("Settings")]
    [SerializeField] float _alertShownXPos = 0f;

    [TabGroup("Settings")]
    public bool _canQuickAcceptReject = false;

    [TabGroup("Settings")]
    [SerializeField] Pos _centerPos;

    [TabGroup("Settings")]
    [SerializeField] Pos _rightPos;

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
    [SerializeField] private bool _isTexting;



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
        SetupAppsAndIcons();
        SetPhonePos(PhonePosition.CENTER);
    }



    void OnEnable()
    {

        GameEventsManager.OnDeliveryAddedEvent += HandleNewDeliveryAdded;
    }

    void OnDisable()
    {
        GameEventsManager.OnDeliveryAddedEvent -= HandleNewDeliveryAdded;
    }

    void Update()
    {
        if (_canQuickAcceptReject)
        {
            if (_quickAcceptInput.GetButtonDown())
            {
                _canQuickAcceptReject = false;
                _newOrderAlert.SetActive(false);
                if (_currentDelivery.Value._chat != null)
                {
                    OnReceiveInstantMessage?.Invoke(_currentDelivery.Value._chat);
                    StartInstantMessage();
                }
            }

            if (_quickRejectInput.GetButtonDown())
            {

            }
        }
    }

    [Button, TabGroup("Debug")]
    void StartInstantMessage()
    {
        // SetApp(AppNames.MESSENGER_APP);
        ShowPhone(true);
        OnStartInstantMessage?.Invoke();
        _isTexting = true;
    }
    [Button, TabGroup("Debug")]
    void DebugInstantMessage(Chat chat)
    {
        OnReceiveInstantMessage?.Invoke(chat);
        StartInstantMessage();
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
        if (Player.Instance != null)
        {
            Player.Instance.ShowPhone(useBlur);
        }
    }

    void AcceptDelivery()
    {
        GameEventsManager.Instance.OnDeliveryAccepted(_currentDelivery.Value);
    }


    [Button, TabGroup("Debug")]
    public void SetPhonePos(PhonePosition pos)
    {
        if (pos == PhonePosition.RIGHT)
        {
            _phoneModel.transform.SetLocalPositionAndRotation(_rightPos.position, Quaternion.Euler(_rightPos.rotation));
        }
        if (pos == PhonePosition.CENTER)
        {
            _phoneModel.transform.SetLocalPositionAndRotation(_centerPos.position, Quaternion.Euler(_centerPos.rotation));
            HudManager.Instance.OnToggleUi();
        }
    }

    public void ToggleUseInput()
    {
        _isPhoneActive = !_isPhoneActive;
    }

    // void OnPhoneButtonPress(Inputs input)
    // {
    //     if (!_isPhoneActive) return;
    //     if (_currentApp == null)
    //     {
    //         UseHomePageNavigation(input);
    //         return;
    //     }
    //     else
    //     {
    //         RelayInputToCurrentApp(input);
    //     }
    // }


    void UseHomePageNavigation(Inputs input)
    {
        switch (input)
        {
            case Inputs.LEFT:
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
            case Inputs.RIGHT:
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
            case Inputs.SELECT:
                SelectApp();
                break;
            case Inputs.BACK:
                GoToHomePage();
                break;
            case Inputs.ACCEPT:
                if (_isTexting)
                {
                    OnSendInstantMessage?.Invoke(0);
                }
                break;
            case Inputs.REJECT:
                if (_isTexting)
                {
                    OnSendInstantMessage?.Invoke(1);
                }
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

    private void HandleNewDeliveryAdded(DeliveryData data)
    {

        DeliveryApp deliveryApp = _installedApps.Find(app => app is DeliveryApp) as DeliveryApp;
        if (deliveryApp != null)
        {

            deliveryApp.HandleNewDelivery(data);
        }
        else
        {
            Debug.LogWarning("No DeliveryApp found in installed apps.");
        }
        _currentDelivery = data;
        _newOrderDeliveryFeeText.text = data._deliveryFee.ToString();
        _newOrderAlert.SetActive(true);
        _newOrderAlert.transform.DOMoveX(_alertShownXPos, 0.5f).onComplete = () => { _canQuickAcceptReject = true; };
        Invoke(nameof(HideDeliveryAlert), 3f);

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

            Sprite sprite = phoneApp.AppIcon.IconSprite;
            Image image = Instantiate(_appIconPrefab, _homeScreenTransform);
            image.sprite = sprite;
            CreateButton(image, phoneApp.ID);

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
    private void SetApp(AppNames id)
    {
        if (_activeRoute != null)
        {
            //* Exit out of current app if already in app
            _activeRoute._appMainScreen.gameObject.SetActive(false);
        }
        PhoneApp phoneApp = _installedApps.Find(x => x.ID == id);
        _selectedAppIndex = Routes.IndexOf(phoneApp);
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
        button.onClick.AddListener(() => { Debug.Log($"Selected App is {phoneAppID}"); });
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

