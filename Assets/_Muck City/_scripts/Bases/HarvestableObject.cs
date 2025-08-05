using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.VFX;
using System.Threading.Tasks;



public class HarvestableObject : MonoBehaviour, IHarvestableObject
{
    public GameObject GameObject => gameObject;


    public bool CanHarvest => true;


    public void OnHarvest()
    {
        gameObject.SetActive(false);
    }
}

