using DG.Tweening;
using UnityEngine;

public class Lift : MonoBehaviour
{

    [Header("Points **WARNING ORDER FROM LOW TO HIGH**")]
    [Tooltip("Points lift will move to **WARNING ORDER FROM LOW TO HIGH**")]
    [SerializeField] private Transform[] _points;
    [SerializeField] private float _speed = 1f;
    public Transform _centerPoint;

    [SerializeField] int _activePointIndex = 0;


    public void Move(System.Action OnComplete = null)
    {
        int pointToMoveTo = (_activePointIndex + 1) % _points.Length;
        _activePointIndex = pointToMoveTo;
        transform.DOMove(_points[pointToMoveTo].transform.position, _speed)
        .OnComplete(() => OnComplete?.Invoke());
    }

}
