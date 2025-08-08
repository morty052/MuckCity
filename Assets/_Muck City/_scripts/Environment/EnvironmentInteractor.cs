using UnityEngine;
using DG.Tweening;
using System.Collections;
using System;


public class EnvironmentInteractor
{
    private static EnvironmentInteractor _instance;

    public static LightHelper LightHelper => Instance._lightHelper;
    public static SoundHelper SoundHelper => Instance._soundHelper;

    private LightHelper _lightHelper;
    private SoundHelper _soundHelper;

    private EnvironmentInteractor()
    {
        _lightHelper = new LightHelper();
        _soundHelper = new SoundHelper();
    }

    private static EnvironmentInteractor Instance
    {
        get
        {
            if (_instance == null)
                _instance = new EnvironmentInteractor();
            return _instance;
        }
    }
}



public class LightHelper
{
    public void Toggle(Light light)
    {
        if (light != null)
            light.enabled = !light.enabled;
    }

    public Tween TweenColor(Light light, Color targetColor, float duration)
    {
        if (light == null) return null;
        return DOTween.To(() => light.color, x => light.color = x, targetColor, duration);
    }

    public Tween TweenIntensity(Light light, float targetIntensity, float duration)
    {
        if (light == null) return null;
        return DOTween.To(() => light.intensity, x => light.intensity = x, targetIntensity, duration);
    }

    public Coroutine Flicker(Light light, float minIntensity, float maxIntensity, float speed, float duration, Action OnComplete = null)
    {
        if (light == null) return null;
        return CoroutineRunner.Instance.StartCoroutine(FlickerRoutine(light, minIntensity, maxIntensity, speed, duration, OnComplete));
    }

    public void StopFlicker(Coroutine routine)
    {
        if (routine != null)
            CoroutineRunner.Instance.StopCoroutine(routine);
    }

    private IEnumerator FlickerRoutine(Light light, float minIntensity, float maxIntensity, float speed, float duration, Action OnComplete = null)
    {
        float timer = 0f;
        while (timer < duration)
        {
            timer += Time.deltaTime;
            light.intensity = UnityEngine.Random.Range(minIntensity, maxIntensity);
            yield return new WaitForSeconds(UnityEngine.Random.Range(0.05f, speed));
        }

        OnComplete?.Invoke();
    }
}




public class SoundHelper
{
    public void PlayRandomClip(AudioSource source, AudioClip[] clips)
    {
        if (source == null || clips == null || clips.Length == 0) return;
        source.clip = clips[UnityEngine.Random.Range(0, clips.Length)];
        source.Play();
    }

    public void PlayWithPitch(AudioSource source, AudioClip clip, float pitch)
    {
        if (source == null || clip == null) return;
        source.pitch = pitch;
        source.clip = clip;
        source.Play();
    }
}
