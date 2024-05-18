using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
	private static SceneLoader instance;
	private static bool isQuit;

    private void Awake()
	{
		instance = this;
	}

	public static void LoadScene(string sceneName)
	{
		if (isQuit) return;

		instance?.StartCoroutine(LoadSceneAsync(sceneName, true, LoadSceneMode.Single, null));
	}

	public static void LoadSceneAdditive(string sceneName, System.Action OnLoadFinishCallback, System.Action<float> OnProgress = null)
	{
		LoadSceneAdditive(sceneName, true, OnLoadFinishCallback, OnProgress);
	}

	public static void LoadSceneAdditive(string sceneName, bool isNeedActive)
	{
		if (isQuit) return;

		LoadSceneAdditive(sceneName, isNeedActive, null);
	}

	public static void LoadSceneAdditive(string sceneName)
	{
		LoadSceneAdditive(sceneName, true, null);
	}

	public static void LoadSceneAdditive(string sceneName, bool isNeedActive, System.Action OnLoadFinishCallback, System.Action<float> OnProgress = null)
	{
		if (isQuit) return;

		instance?.StartCoroutine(LoadSceneAsync(sceneName, isNeedActive, LoadSceneMode.Additive, OnLoadFinishCallback, OnProgress));
	}

	private static IEnumerator LoadSceneAsync(string sceneName, bool isNeedActive, LoadSceneMode loadSceneMode, System.Action OnLoadFinishCallback, System.Action<float> OnProgress = null)
	{
		AsyncOperation async = SceneManager.LoadSceneAsync(sceneName, loadSceneMode);
        while(async.isDone == false)
        {
			OnProgress?.Invoke(async.progress);
			yield return null;
        }
		Scene scene = SceneManager.GetSceneByName(sceneName);
		if(isNeedActive) SceneManager.SetActiveScene(scene);
        OnLoadFinishCallback?.Invoke();
	}

	public static void UnloadScene(string sceneName, System.Action OnUnloadFinishCallback = null)
	{
		instance?.StartCoroutine(UnloadSceneAsync(sceneName, OnUnloadFinishCallback));
	}

	private static IEnumerator UnloadSceneAsync(string sceneName, System.Action callback)
	{
		Scene unloadScene = SceneManager.GetSceneByName(sceneName);
		AsyncOperation async = SceneManager.UnloadSceneAsync(unloadScene);

		while (async.isDone == false)
		{
			yield return null;
		}
		callback?.Invoke();
	}

	private void OnApplicationQuit()
	{
		StopAllCoroutines();
		isQuit = true;
	}
}
