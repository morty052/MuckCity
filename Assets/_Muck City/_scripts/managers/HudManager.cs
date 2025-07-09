using System;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.DualShock;
using UnityEngine.InputSystem.XInput;
using UnityEngine.UI;


public enum GamePadType
{
    PLAYSTATION,
    XBOX,
    GENERIC,
    NONE
}
public class HudManager : MonoBehaviour
{

    public static HudManager Instance { get; private set; }

    [SerializeField] private GameObject _overlay;

    [SerializeField] public GameObject _interactionPrompt;
    [SerializeField] private GameObject _loadingScreen;
    [SerializeField] private TextMeshProUGUI _interactionPromptText;

    [SerializeField] private GameObject _tutorialPrompt;
    [SerializeField] private TextMeshProUGUI _tutorialPromptTitle;
    [SerializeField] private TextMeshProUGUI _tutorialPromptDescription;
    [SerializeField] private Image _tutorialPromptImage;

    // private float startTime;

    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private TextMeshProUGUI _statusText;

    [SerializeField] private GameObject _phone;
    [SerializeField] private GameObject _ui;

    [SerializeField] ActionText _actionText;



    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }

        else
        {
            Destroy(gameObject);
        }
    }

    void OnEnable()
    {
        // GameEventsManager.OnGameLoadStartEvent += () => ToggleLoadingScreen(true);
        // GameEventsManager.OnGameLoadEndEvent += () => ToggleLoadingScreen(false);
        GameEventsManager.OnDisplayPhone += OnDisplayPhone;
        GameEventsManager.OnHidePhone += OnHidePhone;
        GameEventsManager.OnToggleUiEvent += OnToggleUi;
        GameEventsManager.OnSceneLoadStartEvent += (location) => ToggleLoadingScreen(true);
        GameEventsManager.OnSceneLoadEndEvent += () => ToggleLoadingScreen(false);


    }

    void OnDisable()
    {
        // GameEventsManager.OnGameLoadStartEvent -= () => ToggleLoadingScreen(true);
        // GameEventsManager.OnGameLoadEndEvent -= () => ToggleLoadingScreen(false);
        GameEventsManager.OnDisplayPhone -= OnDisplayPhone;
        GameEventsManager.OnHidePhone -= OnHidePhone;
        GameEventsManager.OnToggleUiEvent -= OnToggleUi;
        GameEventsManager.OnSceneLoadStartEvent -= (location) => ToggleLoadingScreen(true);
        GameEventsManager.OnSceneLoadEndEvent -= () => ToggleLoadingScreen(false);
    }

    private void OnToggleUi()
    {
        _ui.SetActive(!_ui.activeSelf);
    }

    void UseStatusText(string text, Color color = default)
    {
        if (color != default)
        {
            _statusText.color = color;
        }
        _statusText.text = text;
        Invoke(nameof(HideStatusText), 3f);
    }

    void HideStatusText()
    {
        _statusText.text = "";
        _statusText.color = Color.white;
    }


    public void ToggleUiVisibility()
    {

    }
    private void OnSocialCreditUpdated(int amount, bool isDeduction)
    {
        if (isDeduction)
        {
            UseStatusText($"- {amount}", Color.red);
        }

        else
        {
            UseStatusText($"+ {amount}", Color.green);
        }
    }

    private void OnDisplayPhone()
    {
        _phone.SetActive(true);
    }
    private void OnHidePhone()
    {
        _phone.SetActive(false);
    }



    public void ShowInteractPrompt(string promptText = null)
    {
        // Debug.Log(promptText);
        if (promptText != null)
        {
            _interactionPromptText.text = promptText;
        }

        _interactionPrompt.SetActive(true);
    }
    public void ShowInteractPrompt(Vector3 position, string promptText = null)
    {

        if (promptText != null)
        {
            _interactionPromptText.text = promptText;
        }

        _actionText.transform.position = position;
        _actionText.SetText(promptText);
        _actionText.gameObject.SetActive(true);
    }
    public void HideInteractPrompt()
    {

        _interactionPrompt.SetActive(false);
        _interactionPromptText.text = "Interact";
        _actionText.gameObject.SetActive(false);

    }
    public void ToggleLoadingScreen(bool state)
    {

        _loadingScreen.SetActive(state);
    }

    public void ToggleTutorialPrompt(bool state, string title = null, string description = null, Sprite image = null)
    {
        if (state)
        {
            _tutorialPromptTitle.text = title;
            _tutorialPromptDescription.text = description;
            // _tutorialPromptImage.sprite = image;
            _tutorialPrompt.SetActive(true);
            Invoke(nameof(HidePrompt), 5f);
        }

        else
        {
            _tutorialPrompt.SetActive(false);
        }
    }

    public GamePadType DetectControllerType()
    {
        var gamepad = Gamepad.current;
        if (gamepad != null)
        {
            if (gamepad is DualShockGamepad)
            {
                Debug.Log("PlayStation Controller detected");
                return GamePadType.PLAYSTATION;
            }
            else if (gamepad is XInputController)
            {
                Debug.Log("Xbox Controller detected");
                return GamePadType.XBOX;
            }
            else
                Debug.Log("Generic Gamepad detected");
            return GamePadType.GENERIC;
        }

        return GamePadType.NONE;
    }
    private void HidePrompt()
    {
        _tutorialPrompt.SetActive(false);
    }
}





