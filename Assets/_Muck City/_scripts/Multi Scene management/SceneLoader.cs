using System;
using System.Threading.Tasks;
using Systems.SceneManagement;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

namespace Systems.SceneManagement
{
    public class SceneLoader : MonoBehaviour
    {
        [SerializeField] Image _loadingBar;
        [SerializeField] float _fillSpeed = 0.5f;
        [SerializeField] Canvas _loadingCanvas;
        [SerializeField] Camera _loadingCam;

        [SerializeField, Space(5)] SceneGroup[] _sceneGroups;

        float _targetProgress;
        bool _isLoading;

        public readonly SceneGroupManager _manager = new();

        void Awake()
        {
            _manager.OnSceneLoaded += sceneName => Debug.Log($"Loaded {sceneName}");
            _manager.OnSceneUnLoaded += sceneName => Debug.Log($"UnLoaded {sceneName}");
            _manager.OnSceneGroupLoaded += () => Debug.Log($"Scene group loaded");

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

