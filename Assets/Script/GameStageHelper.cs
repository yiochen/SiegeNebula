using UnityEngine;
using System.Collections;

public class GameStageHelper: MonoBehaviour {
    public const string NEXT_SCENE = "next";

    public static readonly string[] LEVELS = { "tutorial", "tut1", "tut2", "lvl0", "lvl1" };
	
    public static string GetNextScene(string currentScene)
    {
        for (int i = 0; i<LEVELS.Length-1; i++)
        {
            if (LEVELS[i].Equals(currentScene))
            {
                return LEVELS[i + 1];
            }
        }
        return null;
    }
    


}
