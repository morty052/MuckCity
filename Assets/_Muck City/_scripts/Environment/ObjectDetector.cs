
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;


[System.Serializable]
public class ObjectDetector
{
    public float _radius = 3f;

    public Vector3 _position;

    LayerMask _interactionLayerMask;

    public ObjectDetector(LayerMask layerMask)
    {
        _interactionLayerMask = layerMask;
    }

    [Button("Snap")]
    void Snap()
    {
        _position = Selection.activeGameObject.transform.position;
    }


#nullable enable
    public T? DetectObject<T>() where T : IInteractable
    {

        T? component = default;
        Collider[] hitColliders = Physics.OverlapSphere(_position, _radius, _interactionLayerMask);

        if (hitColliders.Length == 0)
        {
            Debug.Log($"<color=red>No colliders found for {typeof(T)}</color>");
            return component;
        }

        else
        {

            component = hitColliders[0].GetComponent<T>();
            Debug.Log($"<color=green> found  {typeof(T)} {hitColliders.Length}</color>");
            return component;
        }

    }


    public T? DetectObject<T>(Vector3 position, float radius = 0) where T : IInteractable
    {

        T? component = default;
        Collider[] hitColliders = Physics.OverlapSphere(position, radius == 0 ? _radius : radius, _interactionLayerMask);

        if (hitColliders.Length == 0)
        {
            Debug.Log($"<color=red>No colliders found for {typeof(T)}</color>");
            return component;
        }

        else
        {
            component = hitColliders[0].GetComponent<T>();
            foreach (var item in hitColliders)
            {
                Debug.Log($" {item.name}");
            }
            Debug.Log($"<color=green> found  {typeof(T)} {component.GameObject.name}</color>");
            return component;
        }

    }


#nullable disable




}

