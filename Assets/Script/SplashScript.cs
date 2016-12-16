using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class SplashScript : MonoBehaviour {

    public float waitTime = 1.0f;
    public SceneFadingScript fade;
	// Use this for initialization
	void Start () {
        StartCoroutine("NextLevel");
	}
	
	IEnumerator NextLevel()
    {

        yield return new WaitForSeconds(waitTime);
        
        yield return new WaitForSeconds(fade.FadeOut());
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
