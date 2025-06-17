using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

public class Lift : MonoBehaviour
{

    [Header("Points **WARNING ORDER FROM LOW TO HIGH**")]
    [Tooltip("Points lift will move to **WARNING ORDER FROM LOW TO HIGH**")]
    [SerializeField] private Transform[] _points;
    [SerializeField] private bool _togglesAreas;
    [SerializeField] private Transform _toggleAreaOff;
    [SerializeField] private Transform _toggleAreaOn;

    [SerializeField] private float _timeToToggleAreaOff;

    [SerializeField] private float _speed = 1f;
    public Transform _centerPoint;

    [SerializeField] int _activePointIndex = 0;


    public void Move(System.Action OnComplete = null)
    {
        int pointToMoveTo = (_activePointIndex + 1) % _points.Length;
        _activePointIndex = pointToMoveTo;
        transform.DOMove(_points[pointToMoveTo].transform.position, _speed)
        .SetEase(Ease.Linear)
        .OnComplete(() => OnComplete?.Invoke());

        if (_togglesAreas)
        {
            Invoke(nameof(ToggleAreaOff), _timeToToggleAreaOff);
        }
    }


    [Button]
    void DebugLift()
    {
        int pointToMoveTo = (_activePointIndex + 1) % _points.Length;
        _activePointIndex = pointToMoveTo;
        transform.position = _points[pointToMoveTo].transform.position;
    }

    void ToggleAreaOff()
    {
        _toggleAreaOff.gameObject.SetActive(false);
        _toggleAreaOn.gameObject.SetActive(true);
    }

}
