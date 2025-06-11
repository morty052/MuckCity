using UnityEngine;
using UnityEngine.SceneManagement;
public class MainMenu : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] SceneField _persistentScene;
    [SerializeField] SceneField _lastSavedScene;
    public void StartGame()
    {
        SceneManager.LoadSceneAsync(_persistentScene);
        AsyncOperation op = SceneManager.LoadSceneAsync(_lastSavedScene, LoadSceneMode.Additive);
        op.completed += (operation) => { SceneManager.SetActiveScene(SceneManager.GetSceneByName(_lastSavedScene)); };
    }

    void LoadScene()
    {

    }
}
