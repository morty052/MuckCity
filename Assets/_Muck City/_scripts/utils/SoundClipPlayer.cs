using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct ClipData
{
    public AudioClip _clip;
    public string _name;

    public ClipData(AudioClip clip, string name)
    {
        _clip = clip;
        _name = name;
    }
}

public class SoundClipPlayer : MonoBehaviour
{
    [SerializeField] List<ClipData> _clips = new();
    public void PlayClip(string clipName)
    {
        ClipData clipData = _clips.Find(x => x._name == clipName);
        SoundsManager.Instance.PlayClip(clipData._clip, transform, 1);
    }
    public void PlayClip(string clipName, float volume = 1)
    {
        ClipData clipData = _clips.Find(x => x._name == clipName);
        SoundsManager.Instance.PlayClip(clipData._clip, transform, volume);
    }


}
