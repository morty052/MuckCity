using UnityEngine;
using UnityEngine.SceneManagement;

public class BootStrapper : MonoBehaviour
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static async void Init()
    {
        Debug.Log("BootStrapper Started");
        await SceneManager.LoadSceneAsync("BootStrapper", LoadSceneMode.Single);
    }
}
