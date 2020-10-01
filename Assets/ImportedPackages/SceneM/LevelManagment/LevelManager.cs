using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SceneM
{
    public static class LevelManager
    {
        #region Attributes
        public static LevelInfo CurrantLevel { get; private set; }

        public static Action<Func<float>, MainSceneInfo> OnStartLoadingMainScene;

        private static List<int> loadedScenesInLevel = new List<int>();

        #endregion

        #region Scene Managment
        public static void LoadScene(MySceneInfo sceneInfo)
        {
            if (sceneInfo == null)
            {
                Debug.Log("SceneInfo is null");
                return;
            }
            if (sceneInfo is MainSceneInfo msi) 
                LoadMainScene(msi);
            else if (sceneInfo is AdditiveSceneInfo asi)
            {
                LoadAdditiveScene(asi);
            }
        }

        public static void UnloadScene(MySceneInfo sceneInfo)
        {
            if (sceneInfo == null)
            {
                Debug.Log("SceneInfo is null");
                return;
            }

            if (TryGetActiveScene(sceneInfo, out Scene scene))
            {
                SceneDataRepository.SaveData(scene);
                GetUnload(scene);
            }
            else
            {
                Debug.Log("Scene " + sceneInfo.name + " was not loaded");
            }
        }
        #endregion

        #region Level Managment

        public static void LoadLevel(LevelInfo levelInfo)
        {
            SaveAllData();

            if (CurrantLevel != null && !CurrantLevel.IsGlobal)
            {
                SceneDataRepository.DeleteAllLevelData(loadedScenesInLevel.ToArray());
                Inventory.DeleteAllLevelData();
            }

            loadedScenesInLevel.Clear();
            CurrantLevel = levelInfo;

            LoadMainSceneRaw(levelInfo.StartScene);

        } 
        #endregion

        #region Load Scene Methods
        private static void LoadMainScene(MainSceneInfo sceneInfo)
        {
            SaveAllData();
            LoadMainSceneRaw(sceneInfo);
        }

        private static void LoadMainSceneRaw(MainSceneInfo sceneInfo)
        {
            AsyncOperation operation = null;

            OnStartLoadingMainScene?.Invoke(() => {
                if (operation == null)
                    return 0;
                else
                {
                    if (operation.isDone)
                    {
                        return 1;
                    }
                    else
                        return operation.progress;
                }
            }, sceneInfo);
            if (sceneInfo.Transition != null && sceneInfo.Transition.StartDurration > 0)
            {
                TimeSystem.Delay(sceneInfo.Transition.StartDurration, t => LoadMainSceneRaw2(sceneInfo, out operation));
            }
            else
                LoadMainSceneRaw2(sceneInfo, out operation);
        }

        private static void LoadMainSceneRaw2(MainSceneInfo sceneInfo,out AsyncOperation operation)
        {
            operation = GetLoad(sceneInfo, LoadSceneMode.Single);

            operation.completed += (o) => OnLoadSceneCompleted(sceneInfo);

            var additional = sceneInfo.AdditionalStartScenes.Distinct(asi => asi.ScenePath);
            foreach (AdditiveSceneInfo asi in additional)
            {
                LoadAdditiveScene(asi);
            }
        }

        private static void LoadAdditiveScene(AdditiveSceneInfo sceneInfo)
        {
            if (SceneManager.GetSceneByPath(sceneInfo.ScenePath).IsValid())
                return;
            //Debug.Log("Load AdditiveScene: valid: "+SceneManager.GetSceneByPath(sceneInfo.ScenePath).IsValid() + " path: " + sceneInfo.ScenePath );

            AsyncOperation operation = GetLoad(sceneInfo,LoadSceneMode.Additive);
            operation.completed += (o) => OnLoadSceneCompleted(sceneInfo);
        }

        private static void OnLoadSceneCompleted(MySceneInfo sceneInfo)
        {
            if (TryGetActiveScene(sceneInfo, out Scene scene))
            {
                SceneDataRepository.LoadData(scene);
                loadedScenesInLevel.Add(scene.buildIndex);

                if(sceneInfo is MainSceneInfo mainSceneInfo && mainSceneInfo.StartCheckpoint != null)
                {
                    CheckpointManager.SetStartCheckpoint(mainSceneInfo.StartCheckpoint);
                }
            }
            else
                Debug.LogError("Something went wrong when loading scene");
        }
        #endregion

        #region Helper Methods
        private static void SaveAllData()
        {
            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                SceneDataRepository.SaveData(SceneManager.GetSceneAt(i));
            }
        }

        #region Scenes
        private static AsyncOperation GetLoad(MySceneInfo sceneInfo,LoadSceneMode loadMode)
        {
            return SceneManager.LoadSceneAsync(sceneInfo.ScenePath, loadMode);
        }

        private static AsyncOperation GetUnload(Scene scene)
        {
            return SceneManager.UnloadSceneAsync(scene);
        }

        private static bool TryGetActiveScene(MySceneInfo sceneInfo, out Scene scene)
        {
            scene = SceneManager.GetSceneByPath(sceneInfo.ScenePath);
            return scene != null && scene.isLoaded;
        }  
        #endregion
        #endregion

    } 
}
