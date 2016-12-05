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
		SetPlanetStarRanking ();
		UpdateUIStats ();
	}

	// Update is called once per frame
	void Update () {
		SetPlanetStarRanking ();
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
		sb.Append (UpdateStat(stats [1], playerResources));
		sb.AppendLine ();
		//Planets
		sb.Append (UpdateStat(stats [2], playerPlanets.Count));
		return sb.ToString ();
	}

	string UpdateEnemyStats(string[] stats) {
		StringBuilder sb = new StringBuilder ();
		//Title
		sb.Append (stats [0]);
		sb.AppendLine ();
		//Planets
		sb.Append (UpdateStat(stats [1], enemyPlanets.Count));
		return sb.ToString ();
	}

	string UpdateStat(string str, int stat) {
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
		this.selectedPlanet = planet;

        ContextualMenuManagerScript.Instance.ActivateForPlanet(planet);
	}

	void SetPlanetStarRanking() {
		int numPlanets = planets.Length;
		for (int i = 0; i < numPlanets; i++) {
			PlanetScript ps = planets[i];
			//Turn off ranking stars if there is no ownership
			if (ps.planetOwnership == PlanetScript.Ownership.Neutral) {
				ps.rankingScript.SetActive (false);
				continue;
			} else
				ps.rankingScript.SetActive (true);
			//Determine Soldier Strength
			int skulls = AbsolutePlanetStrength(ps);
			ps.rankingScript.currentRank = Mathf.Min (skulls, 5);
			//Determine star color
			ps.rankingScript.activeColor = SkullColor(ps.planetOwnership, skulls);
		}
	}

	int AbsolutePlanetStrength(PlanetScript ps) {
		int skulls = 0;
		switch (ps.planetOwnership) {
		case PlanetScript.Ownership.Player:
			skulls = ps.playerSoldiers.soldierCount / GamePlay.SOLDIERS_PER_SKULL;
			return (ps.playerSoldiers.soldierCount == 0 ? 0 : skulls + 1);
		case PlanetScript.Ownership.Enemy:
			skulls = ps.enemySoldiers.soldierCount / GamePlay.SOLDIERS_PER_SKULL;
			return (ps.enemySoldiers.soldierCount == 0 ? 0 : skulls + 1);
		default:
			return 0;
		}
	}

	Color SkullColor(PlanetScript.Ownership ownership, int skulls) {
		if (skulls > 5)
			return Color.magenta;

		switch (ownership) {
		case PlanetScript.Ownership.Player:
			return Color.yellow;
		case PlanetScript.Ownership.Enemy:
			return Color.blue;
		default:
			return Color.white; //Shouldn't happen
		}
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

	public void MineResources (PlanetScript.Ownership owner) {
		switch (owner) {
		case PlanetScript.Ownership.Player:
			playerResources += GamePlay.RESOURCE_RATE;
			break;
		case PlanetScript.Ownership.Enemy:
			enemyResources += GamePlay.RESOURCE_RATE;
			break;
		case PlanetScript.Ownership.Neutral:
			break;

		}

	}

	public int GetPlayerSoldierCount() {
		return playerSoldiers.soldierCount;
	}

	public int GetPlayerResourceCount() {
		return playerResources;
	}

	public int GetEnemySoldierCount() {
		return enemySoldiers.soldierCount;
	}

	public int GetEnemyResourceCount() {
		return enemyResources;
	}


}
