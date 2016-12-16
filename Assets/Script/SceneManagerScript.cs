using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.UI;

public class SceneManagerScript : MonoBehaviour {

	private string prevScene;
	private string nextScene;
    public Text resultText;

    private AudioManager audioManager;
	void Start() {
		prevScene = PlayerPrefs.GetString (Prefs.PREV_SCENE);
        audioManager = AudioManager.Instance;
        if (PlayerPrefs.GetInt(Prefs.GAME_RESULT) == 1)
        {
			audioManager.PlaySound ("gameWon");
            resultText.text = "You Won!";
        } else
        {
			audioManager.PlaySound ("gameLoss");
            resultText.text = "You Lost!";
        }
        nextScene = GameStageHelper.GetNextScene(prevScene);
	}

	public void NextScene() {
        if (nextScene != null)
        {
            SceneManager.LoadScene(nextScene, LoadSceneMode.Single);
        }
		
	}

	public void RetryScene() {
        if (prevScene != null)
        {
            SceneManager.LoadScene(prevScene, LoadSceneMode.Single);
        }
	}
}
