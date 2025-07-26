using System.Threading.Tasks;
using Eflatun.SceneReference;
using Sirenix.OdinInspector;
using Systems.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoadTrigger : MonoBehaviour
{

    [SerializeField] SceneReference _sceneToLoad;
    [SerializeField] SceneReference _sceneToUnload;

    [SerializeField] bool _autoLoad = false;
    [SerializeField] bool _unloadPrev = true;


    void OnTriggerEnter(Collider other)
    {

    }


    void OnTriggerExit(Collider other)
    {

    }

    [Button]
    public void LoadScene()
    {
        SceneLoader.Instance.AddSceneToStack(_sceneToLoad, SceneType.Environment);
    }

}
