using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/**
 * This class managers the global operations of the game.
 * 1. Player resource count
 * 2. Player planets
 * 3. Enemy resource count
 * 4. Enemy planets
 * 5. Enemy AI updates come from here.
 * 6. Whether there is a win condition
 **/

public class ManagerScript : MonoBehaviour {

	public int numberOfPlanets;
	public List<PlanetScript> playerPlanets;
	public List<PlanetScript> enemyPlanets;
	public int playerResources;
	public int enemyResources;

	private int playerSoldiers;
	private int playerEngineers;

	// Use this for initialization
	void Start () {
		playerPlanets.Capacity = numberOfPlanets;
		enemyPlanets.Capacity = numberOfPlanets;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void AddToSoldierCount (int val) {
		playerSoldiers += val;
	}

	public void AddToEngineerCount (int val) {
		playerEngineers += val;
	}

	public void AddToResourceCount (int val) {
		playerResources += val;
	}

	public int GetPlayerSoldierCount() {
		return playerSoldiers;
	}

	public int GetPlayerEngineerCount() {
		return playerEngineers;
	}

}
