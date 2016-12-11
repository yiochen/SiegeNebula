using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public class HUDScript : MonoBehaviour {

    public Text playerResource;
    public Text playerSoldier;
    public Text playerPlanet;
    public Text enemyPlanet;
    public Text playerLevel;
    public Text enemyLevel;

	// Update is called once per frame
	void Update () {
        ManagerScript manager = ManagerScript.Instance;
        playerResource.text = FormatNumber(manager.playerResources);
        playerPlanet.text = FormatNumber(manager.playerPlanets.Count);
        playerSoldier.text = FormatNumber(manager.GetPlayerSoliderCount());
		playerLevel.text = FormatNumber (manager.GetPlayerLevel ());
        enemyPlanet.text = FormatNumber(manager.enemyPlanets.Count);
		enemyLevel.text = FormatNumber (manager.GetEnemyLevel ());
	}

    string FormatNumber (int value)
    {
        if (value > 10000)
        {
            return "" + (value / 1000);
        } else
        {
            return "" + value;
        }
    }
}
