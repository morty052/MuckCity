using System;
using System.Collections;
using UnityEngine;

public class SoundsManager : MonoBehaviour
{
    public static SoundsManager Instance { get; private set; }

    [SerializeField] private AudioSource _soundFxObj;


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


    public void PlayClip(AudioClip clip, Transform spawnPoint, float volume)
    {
        Debug.Log("playing" + clip.name);
        AudioSource audioSource = Instantiate(_soundFxObj, spawnPoint.position, Quaternion.identity);

        audioSource.clip = clip;

        audioSource.volume = volume;

        audioSource.Play();

        float clipLength = audioSource.clip.length;

        Destroy(audioSource.gameObject, clipLength);
    }

    public void PlayClipWithEventAtEnd(AudioClip clip, Transform spawnPoint, float volume, Action OnComplete)
    {
        Debug.Log("playing" + clip.name);
        AudioSource audioSource = Instantiate(_soundFxObj, spawnPoint.position, Quaternion.identity);

        audioSource.clip = clip;

        audioSource.volume = volume;

        audioSource.Play();

        float clipLength = audioSource.clip.length;

        Destroy(audioSource.gameObject, clipLength);

        StartCoroutine(DelayedInvoke(clipLength, OnComplete));

    }

    public void PlayRandomClip(AudioClip[] clip, Transform spawnPoint, float volume)
    {
        int rand = UnityEngine.Random.Range(0, clip.Length);
        AudioSource audioSource = Instantiate(_soundFxObj, spawnPoint.position, Quaternion.identity);

        audioSource.clip = clip[rand];

        audioSource.volume = volume;

        audioSource.Play();

        float clipLength = audioSource.clip.length;

        Destroy(audioSource.gameObject, clipLength);
    }

    protected IEnumerator DelayedInvoke(float delay, Action action)
    {
        yield return new WaitForSeconds(delay);
        action?.Invoke();
    }
}
