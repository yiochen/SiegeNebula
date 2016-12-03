using UnityEngine;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

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

	public GameObject planetContainer;
    public GameObject shipContainer;
    public PathManagerScript pathManager;
	public SlideManagerScript slideManager;

	public List<PlanetScript> playerPlanets;
	public List<PlanetScript> enemyPlanets;
	public int playerResources;
	public int enemyResources;

	public SoldierUnit playerSoldiers;
	public SoldierUnit enemySoldiers;

	private int enemyEngineers;
	private int playerEngineers;
	private PlanetScript[] planets;
	private Text[] textBoxes;
    
	private PlanetScript selectedPlanet;


	// Use this for initialization
	void Start () {
		planets = planetContainer.GetComponentsInChildren<PlanetScript>();
		playerPlanets.Capacity = planets.Length;
		enemyPlanets.Capacity = planets.Length;
		PlanetAssignment ();
		textBoxes = slideManager.GetComponentsInChildren<Text> ();
		UpdateUIStats ();
	}

	// Update is called once per frame
	void Update () {
		UpdateUIStats ();
	}

	void UpdateUIStats() {
		for (int i = 0; i < textBoxes.Length; i++) {
			Text t = textBoxes [i];
			if (t.name == "Condition") {
				if (playerPlanets.Count == 0 || enemyPlanets.Count == 0) GameEnd (t);
			} else if (t.name == "PlayerStats") {
				textBoxes[i].text = UpdatePlayerStats(t.text.Split ('\n'));
			} else if (t.name == "EnemyStats") {
				textBoxes[i].text = UpdateEnemyStats(t.text.Split ('\n'));
			}
		}
	}

	string UpdatePlayerStats(string[] stats) {
		StringBuilder sb = new StringBuilder ();
		//Title
		sb.Append (stats [0]);
		sb.AppendLine ();
		//Resources
		sb.Append (updateStat(stats [1], playerResources));
		sb.AppendLine ();
		//Planets
		sb.Append (updateStat(stats [2], playerPlanets.Count));
		return sb.ToString ();
	}

	string UpdateEnemyStats(string[] stats) {
		StringBuilder sb = new StringBuilder ();
		//Title
		sb.Append (stats [0]);
		sb.AppendLine ();
		//Planets
		sb.Append (updateStat(stats [1], enemyPlanets.Count));
		return sb.ToString ();
	}

	string updateStat(string str, int stat) {
		int startPos = str.IndexOf (":") + 1;
		string deleted = str.Remove (startPos);
		string inserted = deleted.Insert (startPos, " " + stat);
		return inserted;
	}

	void GameEnd(Text t) {
		slideManager.Next ();
		if (playerPlanets.Count == 0)
			t.text = "You Lost!";
		else
			t.text = "You Won!";
		StartCoroutine (SceneChange ());
	}

	IEnumerator SceneChange() {
		yield return new WaitForSeconds (2.0f);
		//Store this scenes Index
		PlayerPrefs.SetInt(Prefs.PREV_SCENE, SceneManager.GetActiveScene().buildIndex);
		//Need to create a next scene
		SceneManager.LoadScene (0, LoadSceneMode.Single);
	}

	void PlanetAssignment() {
		foreach (PlanetScript planet in planets) {
			switch (planet.planetOwnership) {
			case PlanetScript.Ownership.Player:
				playerPlanets.Add (planet);
				break;
			case PlanetScript.Ownership.Enemy:
				enemyPlanets.Add (planet);
				break;
			case PlanetScript.Ownership.Neutral:
				break;
			}
		}
	}

	public void ChangeSelection(PlanetScript planet) {

        if (selectedPlanet)
        {
            for (int i = 0; i < selectedPlanet.adjacentPlanet.Length; i++)
            {
                PlanetScript ps = selectedPlanet.adjacentPlanet[i];
                //Deactivate star ranking for non-adjacent planets

                ps.rankingScript.SetActive(false);
            }
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

        ContextualMenuManagerScript.Instance.ActivateForPlanet(planet);
	}

	public int RelativePlanetStrength(PlanetScript basePlanet, PlanetScript comparePlanet) {
		int soldierDiff = comparePlanet.enemySoldiers.soldierCount - basePlanet.playerSoldiers.soldierCount;

		float percDiff = basePlanet.playerSoldiers.soldierCount == 0 ? 0 : soldierDiff / (float)Mathf.Max(basePlanet.playerSoldiers.soldierCount, comparePlanet.enemySoldiers.soldierCount);

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
	/**
	 * The planet variable should store the current post-capture "new" ownership
	 **/
	public void CapturePlanet(PlanetScript.Ownership previousOwner, PlanetScript planet) {
		switch (planet.planetOwnership) {
		case PlanetScript.Ownership.Player:
			if (previousOwner == PlanetScript.Ownership.Enemy) {
				enemyPlanets.Remove (planet);
				playerPlanets.Add (planet);
			} else if (previousOwner == PlanetScript.Ownership.Neutral) {
				playerPlanets.Add (planet);
			} else { //This condition shouldn't happen
				if (!playerPlanets.Contains (planet))
					playerPlanets.Add (planet);
			}
			break;
		case PlanetScript.Ownership.Enemy:
			if (previousOwner == PlanetScript.Ownership.Player) {
				playerPlanets.Remove (planet);
				enemyPlanets.Add (planet);
			} else if (previousOwner == PlanetScript.Ownership.Neutral) {
				enemyPlanets.Add (planet);
			} else { //This condition shouldn't happen
				if (!enemyPlanets.Contains (planet))
					enemyPlanets.Add (planet);
			}
			break;
		case PlanetScript.Ownership.Neutral: //This condition shouldn't happen
			if (enemyPlanets.Contains (planet))
				enemyPlanets.Remove (planet);

			if (playerPlanets.Contains (planet))
				playerPlanets.Remove (planet);
			break;
		}
	}

	public PlanetScript GetSelectedPlanet() {
		return selectedPlanet;
	}

	public void TrainSoldier(PlanetScript planet) {
		switch (planet.planetOwnership) {
		case PlanetScript.Ownership.Player:
			if (playerResources >= GamePlay.SOLDIER_COST) {
				playerSoldiers.soldierCount += GamePlay.SOLDIER_UNIT;
				playerResources -= GamePlay.SOLDIER_COST;
				planet.playerSoldiers.soldierCount += GamePlay.SOLDIER_UNIT;
			}
			break;
		case PlanetScript.Ownership.Enemy:
			if (enemyResources >= GamePlay.SOLDIER_COST) {
				enemySoldiers.soldierCount += GamePlay.SOLDIER_UNIT;
				enemyResources -= GamePlay.SOLDIER_COST;
				planet.enemySoldiers.soldierCount += GamePlay.SOLDIER_UNIT;
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
