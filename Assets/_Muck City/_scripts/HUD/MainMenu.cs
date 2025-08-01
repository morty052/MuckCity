using System.Threading.Tasks;
using Eflatun.SceneReference;
using Systems.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
public class MainMenu : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    [SerializeField] SceneData _startingGameScene;
    [SerializeField] SceneData _gamePlayScene;
    [SerializeField] SceneData _lastSavedScene;

    public bool _debug = false;
    public async void StartGame()
    {
        // AsyncOperation op = SceneManager.LoadSceneAsync(_lastSavedScene, LoadSceneMode.Additive);
        // op.completed += (operation) => { SceneManager.SetActiveScene(SceneManager.GetSceneByName(_lastSavedScene)); };
        await LoadLastSavedScene();
    }

    void Awake()
    {
        GetLastSavedGame();
    }
    void GetLastSavedGame()
    {
        if (ES3.KeyExists("SAVED_GAME"))
        {
            Debug.Log("Found Saved Scenes");
            SceneData sceneData = (SceneData)ES3.Load("SAVED_GAME");
            _lastSavedScene = sceneData;
        }

        else
        {
            if (_debug)
            {

                Debug.Log($"<color=yellow> No  Saved Scene found </color>");
            }
            _lastSavedScene = _startingGameScene;
        }
    }

    async Task LoadLastSavedScene()
    {
        SceneGroup sceneToLoad = new()
        {
            GroupName = _lastSavedScene.Name,
            Scenes = new() { _lastSavedScene, _gamePlayScene }
        };
        // SceneLoader.Instance.AddSceneToStack(_sceneToLoad.Scenes[0].Reference, SceneType.Environment);
        await SceneLoader.Instance.LoadSceneGroup(sceneToLoad, true);
    }
}
