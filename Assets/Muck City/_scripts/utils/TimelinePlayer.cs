using System;
using System.Threading;
using System.Threading.Tasks;
using Invector;
using Invector.vCamera;
using Invector.vCharacterController;
using UnityEngine;
using UnityEngine.Playables;

public class TimelinePlayer : MonoBehaviour
{
    PlayableDirector _playableDirector;
    [SerializeField] bool _isPlaying = false;
    public bool _enablePlayOnAwake = false;
    [SerializeField] string _cutsceneName;
    public float _delay = 1f;
    readonly CancellationTokenSource cts = new();
    public event Action<string> OnCutSceneEnded;
    public event Action<string> OnCutSceneStarted;

    public vThirdPersonCameraState _startingCamState;



    // Start is called once before the first execution of Update after the MonoBehaviour is created


    void OnEnable()
    {
        if (_enablePlayOnAwake)
        {
            GameEventsManager.OnGameLoadEndEvent += PlayTimeline;
        }
    }

    void OnDisable()
    {
        if (_enablePlayOnAwake)
        {
            GameEventsManager.OnGameLoadEndEvent -= PlayTimeline;
        }
    }
    void Awake()
    {
        _playableDirector = GetComponent<PlayableDirector>();
    }


    void OnDestroy()
    {
        cts.Cancel();

        // if (_playableDirector != null && _enablePlayOnAwake)
        // {
        //     _playableDirector.played -= OnAutoPlay;
        // }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !_isPlaying)
        {
            _isPlaying = true;
            PlayTimeline();
        }
    }

    // Update is called once per frame
    async void PlayTimeline()
    {
        if (_playableDirector != null)
        {

            GameEventsManager.Instance.OnCutSceneStarted(this);
            OnCutSceneStarted?.Invoke(_cutsceneName);
            _playableDirector.Play();
            double timelineLength = GetTimeLineLength();
            await DelayedInvoke((float)timelineLength, EndCutScene);
            Destroy(gameObject);
        }
        else
        {
            Debug.LogError("PlayableDirector is not assigned or not found.");
        }
    }

    async void OnAutoPlay(PlayableDirector director = null)
    {
        vThirdPersonCamera cam = Player.Instance._vThirdPersonCamera;

        GameEventsManager.Instance.OnCutSceneStarted(this);
        double timelineLength = GetTimeLineLength();
        await DelayedInvoke((float)timelineLength, EndCutScene);
        Destroy(gameObject);
    }


    double GetTimeLineLength()
    {
        if (_playableDirector != null)
        {
            Debug.Log("Timeline length: " + _playableDirector.duration);
            return _playableDirector.duration;
        }
        else
        {
            Debug.LogError("PlayableDirector is not assigned or not found.");
            return 0;
        }
    }

    void EndCutScene()
    {
        OnCutSceneEnded?.Invoke(_cutsceneName);
        GameEventsManager.Instance.OnCutSceneEnded();
    }

    async Task DelayedInvoke(float delay, Action action)
    {
        await Task.Delay((int)(delay * 1000));
        action?.Invoke();
    }
}
