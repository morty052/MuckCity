using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;


public interface IBrowsable
{
    public void OnButtonPress(Inputs button);
}

public class Rack : Interactable, IBrowsable
{
    [SerializeField]
    CinemachineCamera _cam;
    [SerializeField] private int _selectedItemIndex;
    [SerializeField] private List<NpcSO> _appIcons;

    public override void Interact()
    {
        _cam.gameObject.SetActive(true);
        Player.Instance.HideModel();
        HideInteractionPrompt();
        Player.Instance.UseAltControls(true, this);
    }

    public void OnButtonPress(Inputs input)
    {
        switch (input)
        {
            case Inputs.LEFT:
                // MOVE TO PREVIOUS APP IF CAN GO BACK
                if (_selectedItemIndex > 0)
                {
                    _selectedItemIndex--;
                }
                else
                {
                    //MOVE TO LAST APP IF CANT GO BACK
                    _selectedItemIndex = _appIcons.Count - 1;
                }
                break;
            case Inputs.RIGHT:
                if (_selectedItemIndex == _appIcons.Count - 1)
                {
                    //MOVE TO FIRST APP IF CANT GO FORWARD
                    _selectedItemIndex = 0;
                }

                else
                {
                    //MOVE TO NEXT APP IF CAN GO FORWARD
                    _selectedItemIndex++;
                }
                break;
            case Inputs.SELECT:
                break;
            case Inputs.BACK:
                break;
            default:
                break;
        }
    }
}
