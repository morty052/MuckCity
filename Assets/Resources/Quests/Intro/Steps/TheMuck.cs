using System;
using UnityEngine;

public class TheMuck : QuestStep
{

    Mountable _dodge;

    void Start()
    {
        DogPound dogPound = FindFirstObjectByType<DogPound>();
        QuestItemStruct itemData = FindQuestItemByName("Dog Pound");
        dogPound._roverIsInPound = true;
        AddQuestItemToObject(dogPound, itemData);

        Rack rack = GetQuestItem<Rack>("Gun Rack", true);
        _dodge = GetQuestItem<Mountable>("Dodge", true);
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
