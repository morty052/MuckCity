using UnityEngine;

public class PhoneApp : Tradeable
{
    [SerializeField] private string _appName;
    [SerializeField] private AppIcon _appIcon;

    public AppScreen _appMainScreen;

    public string AppName => _appName;
    public AppIcon AppIcon => _appIcon;

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

    // public void OnSell()
    // {
    //     throw new System.NotImplementedException();
    // }
}
