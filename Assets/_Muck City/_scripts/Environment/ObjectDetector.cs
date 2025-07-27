
using Sirenix.OdinInspector;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityUtils;


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

#if UNITY_EDITOR
    [Button("Snap")]
    void Snap()
    {
        _position = Selection.activeGameObject.transform.position;
    }
#endif

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

            if (hitColliders.Length > 1)
            {
                foreach (var item in hitColliders)
                {
                    item.TryGetComponent(out T itemComponent);
                    if (itemComponent != null)
                    {
                        component = itemComponent;
                        break;
                    }
                }
            }
            else
            {
                component = hitColliders[0].GetComponent<T>();
            }
            Debug.Log($"<color=green> found  {typeof(T)} {component?.GameObject.name}</color>");
            return component;
        }

    }
    public T? DetectFindable<T>(Vector3 position, float radius = 0) where T : IFindable
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
            if (hitColliders.Length > 1)
            {
                Debug.Log($"<color=yellow> found {hitColliders.Length} items </color>");
                foreach (var item in hitColliders)
                {
                    if (item.TryGetComponent(out T itemComponent))
                    {
                        component = itemComponent;
                        Debug.Log($"<color=blue> Selected  {itemComponent.GetType()} {component.GameObject.name} items </color>");
                        if (item.TryGetComponent(out PodcastPlayer interactable))
                        {
                            Debug.Log($" {item.name} {interactable.transform.position.x}{interactable.transform.position.y}{interactable.transform.position.z}");
                            item.PingAndSelect();
                        }
                    }
                    else
                    {

                        if (item.TryGetComponent(out PodcastPlayer interactable))
                        {
                            Debug.Log($" {item.name} {interactable.transform.position.x}{interactable.transform.position.y}{interactable.transform.position.z}");
                            item.PingAndSelect();
                        }
                    }
                }
            }
            Debug.Log($"<color=green> found  {component.GetType()} {component.GameObject.name}</color>");
            return component;
        }

    }


#nullable disable




}

