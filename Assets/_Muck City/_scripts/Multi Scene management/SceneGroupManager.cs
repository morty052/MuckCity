using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Systems.SceneManagement
{
    public class SceneGroupManager
    {
        public event Action<string> OnSceneLoaded = delegate { };
        public event Action<string> OnSceneUnLoaded = delegate { };
        public event Action OnSceneGroupLoaded = delegate { };

        SceneGroup ActiveSceneGroup;

        public async Task LoadScenes(SceneGroup group, IProgress<float> progress, bool reloadDupScenes = false)
        {
            ActiveSceneGroup = group;
            var loadedScenes = new List<string>();

            await UnloadScenes();
            int sceneCount = SceneManager.sceneCount;

            for (int i = 0; i < sceneCount; i++)
            {
                loadedScenes.Add(SceneManager.GetSceneAt(i).name);
            }

            var totalScenesToLoad = ActiveSceneGroup.Scenes.Count;

            var operationsGroup = new AsyncOperationsGroup(totalScenesToLoad);

            for (int i = 0; i < totalScenesToLoad; i++)
            {
                var sceneData = group.Scenes[i];
                if (reloadDupScenes == false && loadedScenes.Contains(sceneData.Name)) continue;

                var operation = SceneManager.LoadSceneAsync(sceneData.Reference.Path, LoadSceneMode.Additive);

                operationsGroup.Operations.Add(operation);

                OnSceneLoaded?.Invoke(sceneData.Name);
            }

            //* WAIT UNTIL ALL ASYNCOPERATIONS IN THE GROUP ARE DONE
            while (!operationsGroup.IsDone)
            {
                progress?.Report(operationsGroup.Progress);
                await Task.Delay(100);
            }

            Scene activeScene = SceneManager.GetSceneByName(ActiveSceneGroup.FindSceneByName(SceneType.ActiveScene));

            if (activeScene.IsValid())
            {
                SceneManager.SetActiveScene(activeScene);
            }

            OnSceneGroupLoaded?.Invoke();
        }

        private async Task UnloadScenes()
        {
            //* CREATE EMPTY LIST OF SCENE NAMES
            var scenes = new List<string>();

            //* STORE ACTIVE SCENE NAME
            var activeScene = SceneManager.GetActiveScene().name;

            //* GET TOTAL SCENE COUNT
            int sceneCount = SceneManager.sceneCount;

            //* LOOP OVER ALL OPEN SCENES
            for (int i = sceneCount - 1; i > 0; i--)
            {
                //* Get the scene at index
                var sceneAt = SceneManager.GetSceneAt(i);
                //*skip if scene is not loaded
                if (!sceneAt.isLoaded) continue;

                //* Get the scene name
                var sceneName = sceneAt.name;
                //*skip if its the active scene or boot strapper
                if (sceneName.Equals(activeScene) || sceneName == "BootStrapper") continue;

                //* add eligible scenes to list
                scenes.Add(sceneName);
            }

            //* CREATE AN ASYNC_OPERATIONS_GROUP
            var operationGroup = new AsyncOperationsGroup(scenes.Count);

            foreach (var scene in scenes)
            {
                var operation = SceneManager.UnloadSceneAsync(scene);
                if (operation == null) continue;

                operationGroup.Operations.Add(operation);

                OnSceneUnLoaded?.Invoke(scene);
            }

            //* WAIT UNTIL ALL ASYNC OPERATIONS IN THE GROUP ARE DONE
            while (!operationGroup.IsDone)
            {
                await Task.Delay(100); //* DELAY TO AVOID TIGHT LOOP
            }
        }
    }

    public readonly struct AsyncOperationsGroup
    {
        public readonly List<AsyncOperation> Operations;

        public float Progress => Operations.Count == 0 ? 0 : Operations.Average(o => o.progress);
        public bool IsDone => Operations.All(o => o.isDone);

        public AsyncOperationsGroup(int initialCapacity)
        {
            Operations = new List<AsyncOperation>(initialCapacity);
        }
    }

}
