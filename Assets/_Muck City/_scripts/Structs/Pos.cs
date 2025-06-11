using UnityEngine;

[System.Serializable]
public struct Pos
{
    public Vector3 position;
    public Vector3 rotation;

    // Update is called once per frame
    public Pos(Vector3 position, Vector3 rotation)
    {
        this.position = position;
        this.rotation = rotation;
    }
}
