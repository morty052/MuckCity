
using Sirenix.OdinInspector;
using UnityEngine;

public enum AppType
{
    INSTALLABLE,
    IN_BUILT
}
[InlineEditor]
public class PhoneApp : Tradeable, IDoQuickAction
{

    [TabGroup("App Settings")]
    public AppNames ID;
    [TabGroup("App Settings")]
    public AppType _type;
    [TabGroup("App Settings")]
    [SerializeField] private string _appName;
    [TabGroup("App Settings")]
    [SerializeField] private AppIcon _appIcon;
    [TabGroup("App Settings")]
    public AppScreen _appMainScreen;

    [TabGroup("App Settings")]
    public bool _debug;

    public string AppName => _appName;

    public AppIcon AppIcon => _appIcon;

    [HideInInspector]
    protected NotificationSystem _notificationSystem;
    [HideInInspector]
    protected GameObject _externalCanvas;

    public ActionPrompt _actionPrompt;


    public virtual void Init(NotificationSystem notificationSystem, GameObject externalCanvas, ActionPrompt actionPrompt)
    {
        _notificationSystem = notificationSystem;
        _externalCanvas = externalCanvas;
        _actionPrompt = actionPrompt;

        //* FUNCTION CALLED TO ALLOW INDIVIDUAL PHONE APPS DO THEIR UNIQUE SETUP
        OnInit();
    }
    public virtual void OnInit()
    {

    }

    public virtual void OnSelectPressed()
    {
        // This method can be overridden by derived classes to handle selection events
        Debug.Log("On Select pressed");
    }

    public virtual void OnBackPressed()
    {
        // This method can be overridden by derived classes to handle selection events
        Debug.Log("On Back pressed");
        Phone.Instance.GoToHomePage();
    }
    public virtual void OnLeftPressed()
    {
        // This method can be overridden by derived classes to handle selection events
        Debug.Log("On Left pressed");
    }
    public virtual void OnUpPressed()
    {
        // This method can be overridden by derived classes to handle selection events
        Debug.Log("On Up pressed");
    }
    public virtual void OnRightPressed()
    {
        // This method can be overridden by derived classes to handle selection events
        Debug.Log("On Right pressed");
    }
    public virtual void OnDownPressed()
    {
        // This method can be overridden by derived classes to handle selection events
        Debug.Log("On Down pressed");
    }
    public virtual void OnAcceptPressed()
    {
        // This method can be overridden by derived classes to handle selection events
        Debug.Log("On Accept pressed");

    }
    public virtual void OnRejectPressed()
    {
        // This method can be overridden by derived classes to handle selection events
        Debug.Log("On Reject pressed");
    }

    public override void OnBuy(ShopItemSO shopItemSO)
    {
        Phone.Instance.InstallApp(this);
    }

    public virtual void DoQuickAction()
    {
        throw new System.NotImplementedException();
    }

    public virtual void UseQuickAction()
    {
        Player.Instance._activeQuickAction = this;
    }
    protected virtual void DisplayActionPrompt(string promptOne, string promptTwo)
    {
        _externalCanvas.SetActive(true);
        _actionPrompt.UseActionPrompt(promptOne, promptTwo);
        _actionPrompt.gameObject.SetActive(true);
    }
    protected virtual void DisposeActionPrompt(bool hidePhoneToo)
    {
        _externalCanvas.SetActive(false);
        _actionPrompt.gameObject.SetActive(false);
        if (hidePhoneToo)
        {
            Player.Instance.TogglePhone();
        }
    }
}
