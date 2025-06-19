using System;
using System.Collections.Generic;
using UnityEngine;




public class VisitTownCenter : QuestStep
{

    void OnEnable()
    {

    }
    void OnDisable()
    {

    }

    void Start()
    {

        Debug.Log($"<color=green>Started Quest");
        DoorSwitch doorTrigger = GetQuestItem<DoorSwitch>("Room Door");
        Debug.Log($"<color=orange> Detecting Object from quest {doorTrigger.name}");
    }



}
