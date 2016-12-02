using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneManagerScript : MonoBehaviour {

	private int prevScene;
	private int nextScene;

	void Start() {
		prevScene = PlayerPrefs.GetInt (Prefs.PREV_SCENE);
		if (SceneManager.sceneCount - 1 == prevScene)
			nextScene = prevScene;
		else
			nextScene = prevScene + 1;
	}

	public void NextScene() {
		SceneManager.LoadScene (nextScene, LoadSceneMode.Single);
	}

	public void RetryScene() {
		SceneManager.LoadScene (prevScene, LoadSceneMode.Single);
	}
}
