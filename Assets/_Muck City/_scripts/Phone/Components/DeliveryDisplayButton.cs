using TMPro;
using UnityEngine;

public class DeliveryDisplayButton : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _deliveryFreeText;

    public DeliveryData _data;

    public int _deliveryFee = 0;



    public void Init(DeliveryData data)
    {
        _deliveryFee = data._deliveryFee;
        _deliveryFreeText.text = data._deliveryFee.ToString();
        _data = data;
    }
}
