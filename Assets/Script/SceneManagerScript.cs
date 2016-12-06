using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.UI;

public class SceneManagerScript : MonoBehaviour {

	private int prevScene;
	private int nextScene;
    public Text resultText;
    
	void Start() {
		prevScene = PlayerPrefs.GetInt (Prefs.PREV_SCENE);
        if (PlayerPrefs.GetInt(Prefs.GAME_RESULT) == 1)
        {
            resultText.text = "You Won!";
        } else
        {
            resultText.text = "You Lost!";
        }
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
