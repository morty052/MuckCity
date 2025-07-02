using UnityEngine;

public class DogPound : Shop
{
    [SerializeField] private Pos _frontDesk;
    [SerializeField] private Pos _dogDepositPos;
    [SerializeField] private Pos _roverSpotPos;

    public bool _isAtCounter;
    public bool _roverIsInPound;


    // public void ExitShop()
    // {
    //     Debug.Log("Player entered dog pound");
    //     _UI.SetActive(true);
    // }

    public override void PrepareInteraction()
    {
        if (_shopUi.activeSelf) return;
        _isAtCounter = true;
        HudManager.Instance.ShowInteractPrompt(_frontDesk.position, "Dog Pound");
        Player.Instance.SetInteractableObject(this);
    }

    public override void Interact()
    {
        if (_isAtCounter)
        {
            OpenShop();
            Player.Instance.EnterShop(this);
        }

        else
        {
            if (!_roverIsInPound)
            {
                DropRover();
            }

            else
            {
                GetRover();
            }
        }
        HideInteractionPrompt();
    }

    public void ShowInteractionPrompt()
    {
        _interactionPrompt = "Dog Pound";
        HudManager.Instance.ShowInteractPrompt(_frontDesk.position, _interactionPrompt);
    }
    public void ShowGetRoverPrompt()
    {
        if (_roverIsInPound)
        {
            _interactionPrompt = "Get Rover";
        }
        else
        {
            _interactionPrompt = "Drop Rover";
        }
        HudManager.Instance.ShowInteractPrompt(_dogDepositPos.position, _interactionPrompt);
        Player.Instance.SetInteractableObject(this);
        _isAtCounter = false;
    }

    public override void HideInteractionPrompt()
    {
        HudManager.Instance.HideInteractPrompt();
    }

    public void GetRover()
    {
        Dog.Instance._hasAccessToPlayer = true;
        _roverIsInPound = false;
    }
    public void DropRover()
    {
        Dog.Instance._hasAccessToPlayer = false;
        Dog.Instance.MoveToPoint(_roverSpotPos.position);
        _roverIsInPound = true;
        Debug.Log($"<color=purple>Dropping rover in pound</color>");
    }
}
