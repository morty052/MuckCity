using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;
// using StarterAssets;
using System.Collections;

public class DoorTrigger : MonoBehaviour
{
    [SerializeField] GameObject _door;

    void OnTriggerEnter()
    {

        OpenDoor();

    }
    void OnTriggerExit()
    {
        Invoke(nameof(CloseDoor), 0.5f);
    }

    void CloseDoor()
    {
        _door.transform.DOLocalMoveY(0, 1f);

    }
    void OpenDoor()
    {
        _door.transform.DOLocalMoveY(_door.transform.localPosition.y + 5f, 1f);
    }


}
