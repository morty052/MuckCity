using System.Collections;
using System.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;

public class PodDoor : MonoBehaviour
{
    public Transform _doorModel;
    public bool _isOpen;
    [SerializeField] UnityEvent OnExitDoor;
    [SerializeField] UnityEvent OnEnterDoor;

    void OnTriggerEnter()
    {
        if (_isOpen) return;
        bool playerIsAheadOfDoor = ABUtils.IsAhead(transform, Player.Instance.transform);

        if (playerIsAheadOfDoor)
        {
            _doorModel.transform.DOLocalRotate(new Vector3(0, -90, 0), 1f).OnComplete(() => _isOpen = true); //Rotate to (-90)
        }

        else
        {
            _doorModel.transform.DOLocalRotate(new Vector3(0, 90, 0), 1f).OnComplete(() => _isOpen = true); //Rotate to (90)
        }

    }



    void OnTriggerExit()
    {
        if (_isOpen)
        {
            StartCoroutine(CloseDoor());
        }
    }

  

    IEnumerator CloseDoor()
    {
        yield return new WaitForSeconds(1.5f);
        while (Vector3.Distance(transform.position, Player.Instance.transform.position) < 1.8f) yield return null;
        _doorModel.transform.DOLocalRotate(new Vector3(0, 0, 0), 1f).OnComplete(() => _isOpen = false);
       
       bool playerIsAheadOfDoor = ABUtils.IsAhead(transform, Player.Instance.transform);
        if (playerIsAheadOfDoor)
        {
            OnExitDoor?.Invoke();
        }
        else
        {
           OnEnterDoor?.Invoke();
        }
    }
}
