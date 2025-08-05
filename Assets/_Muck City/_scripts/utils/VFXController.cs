using System;
using System.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.VFX;

public class VFXController : MonoBehaviour
{
    [SerializeField] VisualEffect _effect;

    public Transform _lerpTarget;

    public float _timeToScrubFor = 3f;

    public float _attractionSpeed = 1.5f;

    public float _attractionDelay = 3;

    public DelveBuddy _delveBuddy;

    public float _playRate = 1;
    private bool _lerping;

    private bool _canUse = false;

    bool triggeredOnreachedTimeEvent = false;

    public AnimationCurve _lerpCurve;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        _effect = GetComponent<VisualEffect>();
    }

    public void SetLerpTarget(Transform transform)
    {
        _lerpTarget = transform;
    }



    public void LerpToTarget()
    {
        ABUtils.StartLerp(_effect.transform, _lerpTarget.transform, _attractionSpeed, 0, _lerpCurve, ResetParticle);
    }


    public void ResetParticle()
    {
        _effect.Reinit();
        _effect.Stop();
        _effect.gameObject.SetActive(false);
        Debug.Log("Particle reset");
    }


    [Button]
    public async Task PlayManualVfx(float timeToWaitBeforeEvent = 0, Action OnReachedEventTime = null)
    {
        if (!Application.isPlaying && _delveBuddy == null)
        {
            _delveBuddy = GameObject.FindFirstObjectByType<DelveBuddy>();
        }

        if (!Application.isPlaying)
        {
            _effect.transform.position = _lerpTarget.transform.position;
        }

        float elapsedTime = 0;
        _effect.Play();
        _effect.gameObject.SetActive(true);

        while (elapsedTime < _timeToScrubFor)
        {
            elapsedTime += Time.deltaTime;
            _effect.AdvanceOneFrame();
            Debug.Log(" time: " + elapsedTime);
            if (elapsedTime > timeToWaitBeforeEvent)
            {
                if (!triggeredOnreachedTimeEvent)
                {
                    Debug.Log("reached event time: " + elapsedTime);
                    OnReachedEventTime?.Invoke();
                    triggeredOnreachedTimeEvent = true;
                }
            }
            await Task.Yield(); // yield control to avoid blocking the main thread
        }

    }

    [Button]
    public async Task AdvanceVfxForFrames(float frames)
    {
        float framesToPlay = frames;
        while (framesToPlay > 0)
        {
            _effect.AdvanceOneFrame();
            framesToPlay--;
            await Task.Yield();
        }
    }
}
