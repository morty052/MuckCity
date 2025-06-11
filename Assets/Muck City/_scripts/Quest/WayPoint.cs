using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Waypoint : MonoBehaviour
{
    public static Waypoint Instance { get; private set; }
    public static Action<string> OnMarkerReached;

    public Image _img;

    public Vector3 _target;


    [SerializeField] bool _isWayPointing;

    public TextMeshProUGUI _metres;
    public string _eventTitle;


    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }

        else
        {
            Destroy(gameObject);
        }
    }
    public void Init(Vector3 position)
    {
        _target = position;
        _isWayPointing = true;
        _img.gameObject.SetActive(true);

    }


    public void Update()
    {
        if (!_isWayPointing) return;
        DisplayQuestMarker();
        CheckIfReached();
    }

    void CheckIfReached()
    {
        if (Vector3.Distance(_target, Player.Instance.transform.position) < 2f)
        {
            _img.gameObject.SetActive(false);
            _isWayPointing = false;
            OnMarkerReached?.Invoke(_eventTitle);
        }
        else
        {
            Debug.Log(Vector3.Distance(_target, Player.Instance.transform.position));
        }

    }

    void DisplayQuestMarker()
    {
        float minX = _img.GetPixelAdjustedRect().width / 2;
        float maxX = Screen.width - minX;

        float minY = _img.GetPixelAdjustedRect().height / 2;
        float maxY = Screen.height - minY;

        Vector2 pos = Camera.main.WorldToScreenPoint(_target);

        // Debug.Log(Vector3.Dot(transform.position, Player.Instance.transform.forward));

        if (Vector3.Dot(_target - transform.position, transform.forward) < 0)
        {
            // Debug.Log("has gone behind");
            //Target is behind the player
            if (pos.x < Screen.width / 2)
            {
                pos.x = maxX;
            }
            else
            {
                pos.x = minX;
            }

        }

        pos.x = Mathf.Clamp(pos.x, minX, maxX);
        pos.y = Mathf.Clamp(pos.y, minY, maxY);

        _img.transform.position = pos;
        _metres.text = ((int)Vector3.Distance(_target, Player.Instance.transform.position)).ToString() + " M";
    }
}
