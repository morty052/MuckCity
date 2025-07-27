using UnityEngine;
using UnityEngine.Events;

public class VisibilityDetector : MonoBehaviour
{
    [SerializeField] UnityEvent OnVisible;
    [SerializeField] UnityEvent OnInvisible;

    public bool _debug = false;

    void OnBecameVisible()
    {
        OnVisible?.Invoke();
        if (_debug) Debug.Log("<color=cyan> SPOTTED BY CAM </color>");
    }
    void OnBecameInvisible()
    {
        OnInvisible?.Invoke();
        if (_debug) Debug.Log("<color=orange> WENT OUT OF VIEW  CAM </color>");
    }
}
