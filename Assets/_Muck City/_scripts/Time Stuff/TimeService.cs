using System;
using UnityEngine;

public class TimeService
{
    readonly TimeSettings _settings;
    DateTime _currentTime;

    readonly TimeSpan _sunRiseTime;
    readonly TimeSpan _sunSetTime;

    readonly Observable<bool> _isDayTime;
    readonly Observable<int> _currentHour;

    public static event Action OnSunRise = delegate { };
    public static event Action OnSunSet = delegate { };
    public static event Action<int> OnHourChange = delegate { };

    public DateTime CurrentTime => _currentTime;

    public TimeService(TimeSettings timeSettings)
    {
        this._settings = timeSettings;
        _currentTime = DateTime.Now + TimeSpan.FromHours(_settings._startHour);
        _sunRiseTime = TimeSpan.FromHours(_settings._sunRiseHour);
        _sunSetTime = TimeSpan.FromHours(_settings._sunSetHour);

        _isDayTime = new Observable<bool>(IsDayTime());
        _currentHour = new Observable<int>(_currentTime.Hour);

        _isDayTime.ValueChanged += day => (day ? OnSunRise : OnSunSet)?.Invoke();
        _currentHour.ValueChanged += _ => OnHourChange.Invoke(_currentHour);
    }

    public void UpdateTime(float deltaTime)
    {
        _currentTime = _currentTime.AddSeconds(deltaTime * _settings._timeMultiplier);
        _isDayTime.Value = IsDayTime();
        _currentHour.Value = _currentTime.Hour;
    }

    bool IsDayTime() => _currentTime.TimeOfDay > _sunRiseTime && _currentTime.TimeOfDay < _sunSetTime;

    public float CalculateSunAngle()
    {
        bool isDay = IsDayTime();
        float startDegree = isDay ? 0 : 180;
        TimeSpan start = isDay ? _sunRiseTime : _sunSetTime;
        TimeSpan end = isDay ? _sunSetTime : _sunRiseTime;


        TimeSpan totalTime = CalculateDifference(start, end);
        TimeSpan elapsedTime = CalculateDifference(start, _currentTime.TimeOfDay);

        double percentage = elapsedTime.TotalMinutes / totalTime.TotalMinutes;
        return Mathf.Lerp(startDegree, startDegree + 180, (float)percentage);

    }


    TimeSpan CalculateDifference(TimeSpan from, TimeSpan to)
    {
        TimeSpan difference = to - from;
        return difference.TotalHours < 0 ? difference + TimeSpan.FromHours(24) : difference;
    }




}
