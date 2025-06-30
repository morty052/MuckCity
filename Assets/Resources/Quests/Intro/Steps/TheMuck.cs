using System;
using UnityEngine;

public class TheMuck : QuestStep
{

    void Start()
    {
        DogPound dogPound = FindFirstObjectByType<DogPound>();
        QuestItemStruct itemData = FindQuestItemByName("Dog Pound");
        dogPound._roverIsInPound = true;
        AddQuestItemToObject(dogPound, itemData);
    }
}
