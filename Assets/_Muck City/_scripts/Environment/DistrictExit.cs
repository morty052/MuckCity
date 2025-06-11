using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

public class DistrictExit : MonoBehaviour
{

    [SerializeField] Transform _rightDoor;
    [SerializeField] Transform _leftDoor;

    // [SerializeField] Transform _backMarker;
    [SerializeField] float _rightOpenPos;
    [SerializeField] float _leftOpenPos;
    [SerializeField] float _rightClosePos;
    [SerializeField] float _leftClosePos;
    [SerializeField] float _openRate;



    [SerializeField] District _district;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    void Awake()
    {
        _district = GetComponentInParent<District>();
    }


    void HandleExitOrEntry()
    {
        CloseBigGate();
        if (IsPlayerAheadOfPos())
        {
            Debug.Log("Player is ahead of the exit gate");
            _district.TogglePlayerPresence(false);
            // _playerIsInCompound = false;

        }

        else
        {
            Debug.Log("Player is behind the exit gate");
            _district.TogglePlayerPresence(true);
            // _playerIsInCompound = true;
        }
    }

    void OnTriggerEnter(Collider other)
    {

        if (!Player.Instance.IsInVehicle)
        {
            Debug.Log(" Player On Foot ");
            OpenBigGate(true);

        }

        else
        {
            Debug.Log(" Player In Vehicle ");
            OpenBigGate();
        }


    }


    void OnTriggerExit(Collider other)
    {
        HandleExitOrEntry();
    }


    bool IsPlayerAheadOfPos()
    {
        Vector3 playerDirection = (Player.Instance.transform.position - transform.position).normalized;
        Vector3 playerForward = Player.Instance.transform.forward;
        float dot = Vector3.Dot(playerDirection, transform.forward);

        return dot > 0;
    }


    [Button]
    void OpenBigGate(bool isHalfWay = false)
    {
        if (!isHalfWay)
        {
            _rightDoor.transform.DOLocalMoveX(_rightOpenPos, _openRate);
            _leftDoor.transform.DOLocalMoveX(_leftOpenPos, _openRate);
        }

        else
        {
            _rightDoor.transform.DOLocalMoveX(_rightOpenPos - 3f, _openRate);
            _leftDoor.transform.DOLocalMoveX(_leftOpenPos + 3f, _openRate);
        }
    }
    void CloseBigGate()
    {
        _rightDoor.transform.DOLocalMoveX(_rightClosePos, _openRate - 1);
        _leftDoor.transform.DOLocalMoveX(_leftClosePos, _openRate - 1);
    }
}
