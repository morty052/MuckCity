using System;
using UnityEngine;

public class TheMuck : QuestStep
{

    Mountable _dodge;

    void Start()
    {
        DogPound dogPound = FindFirstObjectByType<DogPound>();
        QuestItemRef itemData = FindQuestItemByName("Dog Pound");
        dogPound._roverIsInPound = true;
        AddQuestItemToObject(dogPound, itemData._questItemStruct);

        Rack rack = (Rack)GetQuestItem<Rack>("Gun Rack", true);
        _dodge = (Mountable)GetQuestItem<Mountable>("Dodge", true);
        _dodge._playerCanMount = false;
    }

    public override void OnQuestItemInteracted(string questItemTag)
    {
        switch (questItemTag)
        {
            case "Dog Pound":
                CompleteObjective("Dog Pound");
                break;
            case "Buy Nail Gun":
                Debug.Log("Buying Nail Gun for quest");
                break;
            default:
                break;
        }
        RemoveInteractionListener(questItemTag);
    }
}
