using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoadTrigger : MonoBehaviour
{
    [SerializeField] SceneField _sceneToLoad;
    [SerializeField] SceneField _sceneToUnload;

    [SerializeField] Transform _spawnPoint;

    [SerializeField] string _exitText;

    [SerializeField] bool _autoLoad = false;
    [SerializeField] bool _unloadPrev = true;
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (_sceneToLoad != null && _autoLoad)
            {
                SceneManager.LoadScene(_sceneToLoad, LoadSceneMode.Additive);
            }
        }
    }

    public void ShowExitText()
    {
        HudManager.Instance.ShowInteractPrompt(_exitText);
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            HudManager.Instance.HideInteractPrompt();
        }
    }

    public void LoadScene()
    {
        if (_sceneToLoad != null)
        {
            HudManager.Instance.HideInteractPrompt();
            Locations newScene = GameEventsManager.Instance.SceneNameToLocation(_sceneToLoad.SceneName);
            GameEventsManager.Instance.OnSceneLoadStart(_sceneToLoad, _sceneToUnload);
        }
    }

}
