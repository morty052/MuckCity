using UnityEngine;

[System.Serializable]
public struct CutSceneData
{
    public string _name;
    public Pos _spawnPosition;
    public TimelinePlayer _cutScenePlayer;

    public CutSceneData(Pos spawnPosition, TimelinePlayer cutScenePlayer, string name)
    {
        _spawnPosition = spawnPosition;
        _cutScenePlayer = cutScenePlayer;
        _name = name;
    }
}
