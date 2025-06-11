using TMPro;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.UI;

public class MessagePrefab : MonoBehaviour
{
    public Message _message;
    public TextMeshProUGUI _title;
    public TextMeshProUGUI _bodyPreview;
    [SerializeField] private Image _contactPhoto;



    public void InitMessage(Message message)
    {
        _title.text = message._title;
        _bodyPreview.text = message._body;
        _message = message;
    }

}
