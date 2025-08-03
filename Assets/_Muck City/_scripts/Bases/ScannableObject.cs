using UnityEngine;

public class ScannableObject : MonoBehaviour, IScannableObject
{
    public GameObject GameObject => gameObject;

    public virtual void OnScan()
    {

        Debug.Log($"<color=cyan> Entity {transform.name} Has been Scanned </color>");
    }
}
