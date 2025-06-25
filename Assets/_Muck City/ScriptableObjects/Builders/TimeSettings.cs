using UnityEngine;

[CreateAssetMenu(fileName = "Time Settings", menuName = "TimeSettings")]
public class TimeSettings : ScriptableObject
{
    public float _timeMultiplier = 2000;
    public float _startHour = 12;

    public float _sunRiseHour = 6;

    public float _sunSetHour = 18;
}
