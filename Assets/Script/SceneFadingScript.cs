using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class SceneFadingScript : Singleton<SceneFadingScript>
{

    public Texture2D fadeOutTexture;
    public float fadeSpeed = 0.8f;

    private int drawDepth = -1000;
    private float alpha = 1.0f;
    private int fadeDir = -1;

    void OnGUI()
    {
        alpha += fadeDir * fadeSpeed * Time.deltaTime;

        alpha = Mathf.Clamp01(alpha);

        GUI.color = new Color(GUI.color.r, GUI.color.g, GUI.color.b, alpha);
        GUI.depth = drawDepth;
        GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), fadeOutTexture);
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    public float BeginFade(int direction)
    {
        fadeDir = direction;
        return 1.0f / fadeSpeed;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        BeginFade(-1);
    }

    public float FadeOut()
    {
        return BeginFade(1);
    }
}
