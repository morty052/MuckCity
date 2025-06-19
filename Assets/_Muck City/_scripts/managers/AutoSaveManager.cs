using System;
using System.Collections.Generic;
using UnityEngine;


public enum SaveAble
{
    PLAYER,
    CREDITS,
    DISCOVERED_RECIPES

}


public struct PlayerSaveData
{
    public BackPack _hotStorage;
    public Pos _position;

    public PlayerSaveData(Player player)
    {
        _hotStorage = player._hotStorage;
        _position = new(player.transform.position, player.transform.rotation.eulerAngles);
    }
}

public struct SocialCreditData
{
    public int _socialCredit;
    public string _socialCreditTextValue;


    public SocialCreditData(int socialCredit, string socialCreditTextValue)
    {
        _socialCredit = socialCredit;
        _socialCreditTextValue = socialCreditTextValue;
    }

}

public static class AutoSaveManager
{
    public static Action OnShouldAutoSave;

    private static List<SaveAble> _saveables = new()
    {
        SaveAble.PLAYER,
        SaveAble.CREDITS,
        SaveAble.DISCOVERED_RECIPES
    };

    // Update is called once per frame
    public static void Autosave(SaveAble saveAble, object saveData)
    {

        ES3.Save(saveAble.ToString(), saveData);
    }
    public static object Load(SaveAble saveAble)
    {
        if (!ES3.KeyExists(saveAble.ToString()))
        {
            Debug.Log("No save data for " + saveAble.ToString());
            return null;
        }
        object data = ES3.Load(saveAble.ToString());
        return data;
    }

    public static void AutoSaveEvent()
    {
        OnShouldAutoSave?.Invoke();
    }

    public static bool ShouldAutoSave(SaveAble saveAble)
    {
        if (!_saveables.Contains(saveAble)) return false;
        return true;
    }

    public static void DisableSaveable(SaveAble saveAble)
    {
        _saveables.Remove(saveAble);
        ES3.Save("SAVEABLES", _saveables);
    }
    public static void AddSaveable(SaveAble saveAble)
    {
        if (_saveables.Contains(saveAble)) return;
        _saveables.Add(saveAble);
        ES3.Save("SAVEABLES", _saveables);
    }
}
