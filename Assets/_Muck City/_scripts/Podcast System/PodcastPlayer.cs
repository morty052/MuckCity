using System;
using UnityEngine;

public class PodcastPlayer : MonoBehaviour, IFindable
{
    public AudioClip _clip;

    public GameObject GameObject => gameObject;

    public bool IsQuestItem { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

    public void PlayPodcast()
    {
        SoundsManager.Instance.PlayClip(_clip, transform, 1);
    }
    public void PlayPodcast(AudioClip clip)
    {
        SoundsManager.Instance.PlayClip(clip, transform, 0.6f);
    }

    public void RemoveInteractionListener(Action<string> action)
    {
        throw new NotImplementedException();
    }

    public void SetupInteractionListener(Action<string> action)
    {
        throw new NotImplementedException();
    }
}
