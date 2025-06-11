using UnityEngine;

public class FollowPlayer : MonoBehaviour
{

    [SerializeField] private float _followHeight = 40f;

    void LateUpdate()
    {
        transform.position = new Vector3(Player.Instance.transform.position.x, _followHeight, Player.Instance.transform.position.z);
    }

}
