using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public class HUDScript : MonoBehaviour {

    public Text playerResource;
    public Text playerSoldier;
    public Text playerPlanet;
    public Text enemyPlanet;

	// Update is called once per frame
	void Update () {
        ManagerScript manager = ManagerScript.Instance;
        playerResource.text = FormatNumber(manager.playerResources);
        playerPlanet.text = FormatNumber(manager.playerPlanets.Count);
        playerSoldier.text = FormatNumber(manager.GetPlayerSoliderCount());

        enemyPlanet.text = FormatNumber(manager.enemyPlanets.Count);

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
