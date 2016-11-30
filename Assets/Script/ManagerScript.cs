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

public class ManagerScript : Singleton<ManagerScript> {

	public int numberOfPlanets;
	public List<PlanetScript> playerPlanets;
	public List<PlanetScript> enemyPlanets;
	public int playerResources;
	public int enemyResources;

	public SoldierUnit playerSoldiers;
	public SoldierUnit enemySoldiers;

	private int enemyEngineers;
	private int playerEngineers;

	public PlanetScript selectedPlanet;

	// Use this for initialization
	void Start () {
		playerPlanets.Capacity = numberOfPlanets;
		enemyPlanets.Capacity = numberOfPlanets;
	}

	// Update is called once per frame
	void Update () {

	}

	public void ChangeSelection(PlanetScript planet) {

		for (int i = 0; i < selectedPlanet.adjacentPlanet.Length; i++) {
			PlanetScript ps = selectedPlanet.adjacentPlanet [i];
			//Deactivate star ranking for non-adjacent planets
			ps.rankingScript.SetActive (false);
		}


		for (int i = 0; i < planet.adjacentPlanet.Length; i++) {
			PlanetScript ps = planet.adjacentPlanet [i];

			//Activate star ranking for adjacent planets
			ps.rankingScript.SetActive (true);

			//Determine Relative Soldier Strength
			ps.rankingScript.currentRank = RelativePlanetStrength (planet, ps);
		}
        if (planet.rankingScript)
        {
            planet.rankingScript.SetActive(false);
        }


		this.selectedPlanet = planet;
	}

	public int RelativePlanetStrength(PlanetScript basePlanet, PlanetScript comparePlanet) {
		int soldierDiff = comparePlanet.playerSoldiers.soldierCount - basePlanet.playerSoldiers.soldierCount;

		float percDiff = basePlanet.playerSoldiers.soldierCount == 0 ? 0 : soldierDiff / (float)Mathf.Max(basePlanet.playerSoldiers.soldierCount, comparePlanet.playerSoldiers.soldierCount);

		if (percDiff >= 0.3f)
			return 5;
		else if (percDiff > 0.1f)
			return 4;
		else if (percDiff >= -0.1f && percDiff <= 0.1f)
			return 3;
		else if (percDiff < -0.1f)
			return 2;
		else if (percDiff <= -0.3f)
			return 1;
		return 1;
	}

	public PlanetScript GetSelectedPlanet() {
		return selectedPlanet;
	}

	public void TrainSoldier(PlanetScript planet) {
		switch (planet.planetOwnership) {
		case PlanetScript.Ownership.Player:
			if (playerResources >= GamePlay.SOLDIER_COST) {
				playerSoldiers.soldierCount++;
				playerResources -= GamePlay.SOLDIER_COST;
				planet.playerSoldiers.soldierCount++;
			}
			break;
		case PlanetScript.Ownership.Enemy:
			if (enemyResources >= GamePlay.SOLDIER_COST) {
				enemySoldiers.soldierCount++;
				enemyResources -= GamePlay.SOLDIER_COST;
				planet.enemySoldiers.soldierCount++;
			}
			break;
		case PlanetScript.Ownership.Neutral:
			break;
		}
	}

	public void TrainEngineer(PlanetScript planet) {
		switch (planet.planetOwnership) {
		case PlanetScript.Ownership.Player:
			if (playerResources >= GamePlay.ENGINEER_COST) {
				playerEngineers++;
				playerResources -= GamePlay.ENGINEER_COST;
				planet.playerEngineerCount++;
			}
			break;
		case PlanetScript.Ownership.Enemy:
			if (enemyResources >= GamePlay.ENGINEER_COST) {
				enemyEngineers++;
				enemyResources -= GamePlay.ENGINEER_COST;
				planet.enemyEngineerCount++;
			}
			break;
		case PlanetScript.Ownership.Neutral:
			break;
		}

	}

	public void AddToResourceCount (int val, PlanetScript.Ownership owner) {
		switch (owner) {
		case PlanetScript.Ownership.Player:
			playerResources += val;
			break;
		case PlanetScript.Ownership.Enemy:
			enemyResources += val;
			break;
		case PlanetScript.Ownership.Neutral:
			break;

		}
	
	}

	public int GetPlayerSoldierCount() {
		return playerSoldiers.soldierCount;
	}

	public int GetPlayerEngineerCount() {
		return playerEngineers;
	}

	public int GetPlayerResourceCount() {
		return playerResources;
	}

	public int GetEnemySoldierCount() {
		return enemySoldiers.soldierCount;
	}

	public int GetEnemyEngineerCount() {
		return enemyEngineers;
	}

	public int GetEnemyResourceCount() {
		return enemyResources;
	}


}
