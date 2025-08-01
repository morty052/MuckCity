using System;
using System.Linq;
using System.Threading.Tasks;
using Eflatun.SceneReference;
using Sirenix.OdinInspector;
using Systems.SceneManagement;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Systems.SceneManagement
{
    public class SceneLoader : MonoBehaviour
    {
        public static SceneLoader Instance { get; private set; }
        [SerializeField] Image _loadingBar;
        [SerializeField] float _fillSpeed = 0.5f;
        [SerializeField] Canvas _loadingCanvas;
        [SerializeField] Camera _loadingCam;

        // public SceneData _gamePlayScene;

        [SerializeField, Space(5)] SceneGroup[] _sceneGroups;

        float _targetProgress;
        bool _isLoading;

        Scene _activeScene;

        public bool _debug = false;
        private bool _playerIsInTransition;

        public event Action<string> OnSceneLoaded = delegate { };
        public event Action<string> OnSceneUnLoaded = delegate { };
        public static event Action<SceneGroup> OnSceneGroupLoaded = delegate { };

        public readonly SceneGroupManager _manager = new();

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                _manager.OnSceneLoaded += HandleOnSceneLoaded;
                _manager.OnSceneUnLoaded += HandleOnSceneUnloaded;
                _manager.OnSceneGroupLoaded += HandleOnSceneGroupLoaded;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void HandleOnSceneLoaded(string sceneName)
        {
            if (_debug)
            {
                Debug.Log($"Loaded {sceneName}");
            }

            OnSceneLoaded?.Invoke(sceneName);
        }

        private void HandleOnSceneUnloaded(string sceneName)
        {
            if (_debug)
            {
                Debug.Log($"UnLoaded {sceneName}");
            }
        }

        private void HandleOnSceneGroupLoaded(SceneGroup sceneGroup)
        {
            if (_debug)
            {
                Debug.Log($"Scene group loaded");
            }
            if (_playerIsInTransition)
            {
                //* FIND THE LEVELS RESPAWN POINT
                GameObject respawnPoint = GameObject.FindGameObjectWithTag("RespawnPoint");


                //* Remove player parent if any
                Player.Instance.transform.SetParent(null);

                //* RESET TRANSITION
                _playerIsInTransition = false;

                //* UPDATE DEFAULT SPAWN POINT TO LEVELS RESPAWN POINT
                GameManager.Instance.SetSpawnPoint(respawnPoint);

                //* PARENT PLAYER TO POINT
                Player.Instance.transform.SetParent(GameManager.Instance.SpawnPoint);

                //* MAKE SURE PLAYER PLACED CORRECTLY
                Player.Instance.transform.localPosition = Vector3.zero;


                //* ACTIVATE PLAYER
                Player.Instance.gameObject.SetActive(true);
            }

            //* PRIORITIZE SETTING REALM SCENE AS ACTIVE SCENE IF IT EXISTS
            Scene realmScene = SceneManager.GetSceneByName(_manager.ActiveSceneGroup.FindSceneByName(SceneType.Realm));

            if (realmScene.IsValid())
            {
                SceneManager.SetActiveScene(realmScene);
            }
            else
            {
                //* FALL BACK TO SETTING SCENE DATA MARKED AS ACTIVE AS ACTIVE SCENE IF IT EXISTS
                Scene activeScene = SceneManager.GetSceneByName(_manager.ActiveSceneGroup.FindSceneByName(SceneType.ActiveScene));

                if (activeScene.IsValid())
                {
                    SceneManager.SetActiveScene(activeScene);
                }

                else
                {
                    //* FALL BACK TO SETTING ENVIRONMENT SCENE AS ACTIVE SCENE
                    Scene environmentScene = SceneManager.GetSceneByName(_manager.ActiveSceneGroup.FindSceneByName(SceneType.Environment));
                    if (environmentScene.IsValid())
                    {
                        SceneManager.SetActiveScene(environmentScene);
                    }
                }
            }
            OnSceneGroupLoaded?.Invoke(sceneGroup);
        }

        async void Start()
        {
            await LoadSceneGroup(0);
        }

        void Update()
        {
            if (!_isLoading) return;
            float currentFillAmount = _loadingBar.fillAmount;
            float progressDifference = Mathf.Abs(currentFillAmount - _targetProgress);

            float dynamicFillSpeed = progressDifference * _fillSpeed;

            _loadingBar.fillAmount = Mathf.Lerp(currentFillAmount, _targetProgress, Time.deltaTime * dynamicFillSpeed);
        }


        public void AddSceneToStack(SceneReference sceneRef, SceneType sceneType)
        {
            SceneData sceneData = new(sceneRef, sceneType);
            SceneGroup scene = new()
            {
                GroupName = sceneRef.Name,
                Scenes = new() { sceneData }
            };
            _sceneGroups = _sceneGroups.Append(scene).ToArray();
        }

        public async Task LoadSceneGroup(int index)
        {
            _loadingBar.fillAmount = 0f;
            _targetProgress = 1f;

            if (index < 0 || index >= _sceneGroups.Length)
            {
                Debug.LogError($"invalid scene group index {index}");
            }

            LoadingProgress progress = new();
            progress.Progressed += target => _targetProgress = MathF.Max(target, _targetProgress);

            EnableLoadingCanvas();
            await _manager.LoadScenes(_sceneGroups[index], progress);
            EnableLoadingCanvas(false);
            // _loadingCam.gameObject.SetActive(false);
        }
        public async Task LoadSceneGroup(SceneGroup sceneGroup, bool useLoadingScreen = false, bool useTransition = false)
        {
            _loadingBar.fillAmount = 0f;
            _targetProgress = 1f;



            LoadingProgress progress = new();
            progress.Progressed += target => _targetProgress = MathF.Max(target, _targetProgress);

            if (useLoadingScreen)
            {
                EnableLoadingCanvas();
            }
            if (useTransition)
            {
                Player.Instance.gameObject.SetActive(false);
                EnableLoadingCanvas();
                _playerIsInTransition = true;
            }
            await _manager.LoadScenes(sceneGroup, progress);
            if (useLoadingScreen)
            {
                EnableLoadingCanvas(false);
            }
        }

        [Button]
        public async Task LoadSceneGroup(string groupName, string activeSceneGroupName)
        {
            //* STORE THE CURRENT ACTIVE SCENE
            SceneGroup sceneGroupToUnload = _sceneGroups.FirstOrDefault(x => x.GroupName == activeSceneGroupName);
            SceneGroup sceneGroup = _sceneGroups.FirstOrDefault(x => x.GroupName == groupName);

            if (sceneGroup == null)
            {
                Debug.LogError($"invalid scene group name {groupName}");
            }

            float time = 0;
            AsyncOperation loading = SceneManager.LoadSceneAsync(sceneGroup.Scenes[0].Name, LoadSceneMode.Additive);
            while (!loading.isDone)
            {
                await Task.Delay(100);
                time++;
            }

            Debug.Log("done in " + time);

            //*MAKE SCENE ACTIVE
            Scene loadedScene = SceneManager.GetSceneByName(sceneGroup.Scenes[0].Name);

            if (loadedScene.IsValid())
            {
                SceneManager.SetActiveScene(loadedScene);
                Debug.Log("Scene '" + loadedScene.name + "' is now active.");
            }
            else
            {
                Debug.LogWarning("Could not find or activate the loaded scene.");
            }

            await UnloadActiveScene();

        }



        async Task UnloadActiveScene()
        {
            // Get the active scene
            // Scene activeScene = SceneManager.GetActiveScene();

            // Confirm it's valid before unloading
            if (_activeScene.IsValid())
            {
                AsyncOperation unloading = SceneManager.UnloadSceneAsync(_activeScene);
                while (!unloading.isDone)
                {
                    await Task.Delay(100);
                }

                Debug.Log("Scene '" + _activeScene.name + "' has been unloaded.");
            }
            else
            {
                Debug.LogWarning("No valid active scene found to unload.");
            }
        }

        void EnableLoadingCanvas(bool enable = true)
        {
            _isLoading = enable;
            _loadingCanvas.gameObject.SetActive(enable);
            _loadingBar.gameObject.SetActive(enable);
        }
    }

    public class LoadingProgress : IProgress<float>
    {
        public event Action<float> Progressed;

        const float ratio = 1f;

        public void Report(float value)
        {
            Progressed?.Invoke(value / ratio);
        }
    }
}

