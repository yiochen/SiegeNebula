using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class TutorialScript : MonoBehaviour {

    public SlideManagerScript slideManager;

	public void Next()
    {
        if (!slideManager.Next())
        {
            PlayerPrefs.SetString(Prefs.PREV_SCENE, SceneManager.GetActiveScene().name);
            PlayerPrefs.SetInt(Prefs.GAME_RESULT, 1);
            SceneManager.LoadScene(GameStageHelper.NEXT_SCENE, LoadSceneMode.Single);
        }
    }
}
