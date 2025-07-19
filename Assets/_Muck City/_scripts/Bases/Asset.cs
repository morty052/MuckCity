using UnityEngine;

public enum AssetType
{
    PROPERTY,
    BUSINESS
}

public class Asset : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    [field: SerializeField] public int Price { get; private set; }
}
