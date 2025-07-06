using Sirenix.OdinInspector;
#if UNITY_EDITOR
using UnityEditor;
#endif
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

#if UNITY_EDITOR
    [Button("Copy Transform")]
    public void CopyTransform(bool useLocalPosition = false)
    {
        if (Selection.activeGameObject == null)
        {
            Debug.LogError("No transform selected to copy");
            return;
        }
        if (!useLocalPosition)
        {
            position = Selection.activeGameObject.transform.position;
            rotation = Selection.activeGameObject.transform.rotation.eulerAngles;
        }

        else
        {
            position = Selection.activeGameObject.transform.localPosition;
            rotation = Selection.activeGameObject.transform.localRotation.eulerAngles;
        }
    }
#endif
}
