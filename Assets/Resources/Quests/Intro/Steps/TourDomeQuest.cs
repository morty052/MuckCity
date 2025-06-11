using System;
using UnityEngine;

public class TourDomeQuest : QuestStep
{

    void OnEnable()
    {
        Waypoint.OnMarkerReached += HandleMarkerReached;
    }

    void OnDisable()
    {
        Waypoint.OnMarkerReached -= HandleMarkerReached;
    }

    private void HandleMarkerReached(string markerTitle)
    {
        Debug.Log("Marker Reached" + markerTitle);
    }

    private void OnAnnouncementComplete()
    {
        Debug.Log("announcement complete");
    }
}
