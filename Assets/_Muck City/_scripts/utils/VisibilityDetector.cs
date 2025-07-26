using UnityEngine;
using UnityEngine.Events;

public class VisibilityDetector : MonoBehaviour
{
    [SerializeField] UnityEvent OnVisible;
    [SerializeField] UnityEvent OnInvisible;

    void OnBecameVisible()
    {
        Debug.Log("<color=cyan> SPOTTED BY CAM </color>");
        OnVisible?.Invoke();
    }
    void OnBecameInvisible()
    {
        Debug.Log("<color=orange> WENT OUT OF VIEW  CAM </color>");
        OnInvisible?.Invoke();
    }
}
