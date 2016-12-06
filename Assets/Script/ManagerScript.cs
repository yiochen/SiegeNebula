﻿using UnityEngine;
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

	public List<AbstractPlanet> playerPlanets;
	public List<AbstractPlanet> enemyPlanets;
	public int playerResources;
	public int enemyResources;


	//For Testing
	[SerializeField]
	private int playerSoldierCount;
	[SerializeField]
	private int enemySoldierCount;
	[SerializeField]
	private SoldierStats globalPlayerSoldiersStats;
	[SerializeField]
	private SoldierStats globalEnemySoldiersStats;
	[SerializeField]
	private int playerLevel;
	[SerializeField]
	private int enemyLevel;

	private AbstractPlanet[] planets;
	private Text[] textBoxes;
	private AbstractPlanet selectedPlanet;


	// Use this for initialization
	void Start () {
		planets = planetContainer.GetComponentsInChildren<AbstractPlanet>();
		playerPlanets.Capacity = planets.Length;
		enemyPlanets.Capacity = planets.Length;
		PlanetAssignment ();
		textBoxes = slideManager.GetComponentsInChildren<Text> ();
		QuerySoldiers ();
		SetPlanetStarRanking ();
		UpdateUIStats ();

		SetStats (ref globalPlayerSoldiersStats, playerLevel);
		SetStats (ref globalEnemySoldiersStats, enemyLevel);
	}

	// Update is called once per frame
	void Update () {
		SetPlanetStarRanking ();
		UpdateUIStats ();
	}

	void QuerySoldiers() {
		ShipScript[] ships = shipContainer.GetComponentsInChildren<ShipScript> ();
		int pSold = 0;
		int eSold = 0;
		foreach (AbstractPlanet ap in planets) {
			eSold += ap.enemySoldiers;
			pSold += ap.playerSoldiers;
		}
		foreach (ShipScript ship in ships) {
			switch (ship.shipOwnership) {
			case AbstractPlanet.Ownership.Player:
				pSold += ship.soldiersOnBoard;
				break;
			case AbstractPlanet.Ownership.Enemy:
				eSold += ship.soldiersOnBoard;
				break;
			case AbstractPlanet.Ownership.Neutral:
				break;
			}
		}
		playerSoldierCount = pSold;
		enemySoldierCount = eSold;
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
		foreach (AbstractPlanet planet in planets) {
			switch (planet.planetOwnership) {
			case AbstractPlanet.Ownership.Player:
				playerPlanets.Add (planet);
				break;
			case AbstractPlanet.Ownership.Enemy:
				enemyPlanets.Add (planet);
				break;
			case AbstractPlanet.Ownership.Neutral:
				break;
			}
		}
	}

	public void ChangeSelection(AbstractPlanet planet) {
		this.selectedPlanet = planet;

        ContextualMenuManagerScript.Instance.ActivateForPlanet(planet);
	}

	void SetPlanetStarRanking() {
		int numPlanets = planets.Length;
		for (int i = 0; i < numPlanets; i++) {
			AbstractPlanet ps = planets[i];
			//Turn off ranking stars if there is no ownership
			if (ps.planetOwnership == AbstractPlanet.Ownership.Neutral) {
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

	int AbsolutePlanetStrength(AbstractPlanet ps) {
		int skulls = 0;
		switch (ps.planetOwnership) {
		case AbstractPlanet.Ownership.Player:
			skulls = ps.playerSoldiers / GamePlay.SOLDIERS_PER_SKULL;
			return (ps.playerSoldiers == 0 ? 0 : skulls + 1);
		case AbstractPlanet.Ownership.Enemy:
			skulls = ps.enemySoldiers / GamePlay.SOLDIERS_PER_SKULL;
			return (ps.enemySoldiers == 0 ? 0 : skulls + 1);
		default:
			return 0;
		}
	}

	Color SkullColor(AbstractPlanet.Ownership ownership, int skulls) {
		if (skulls > 5)
			return Color.magenta;

		switch (ownership) {
		case AbstractPlanet.Ownership.Player:
			return Color.yellow;
		case AbstractPlanet.Ownership.Enemy:
			return Color.blue;
		default:
			return Color.white; //Shouldn't happen
		}
	}
	/**
	 * The planet variable should store the current post-capture "new" ownership
	 **/
	public void CapturePlanet(AbstractPlanet.Ownership previousOwner, AbstractPlanet planet) {
		switch (planet.planetOwnership) {
		case AbstractPlanet.Ownership.Player:
			if (previousOwner == AbstractPlanet.Ownership.Enemy) {
				enemyPlanets.Remove (planet);
				playerPlanets.Add (planet);
			} else if (previousOwner == AbstractPlanet.Ownership.Neutral) {
				playerPlanets.Add (planet);
			} else { //This condition shouldn't happen
				if (!playerPlanets.Contains (planet))
					playerPlanets.Add (planet);
			}
			break;
		case AbstractPlanet.Ownership.Enemy:
			if (previousOwner == AbstractPlanet.Ownership.Player) {
				playerPlanets.Remove (planet);
				enemyPlanets.Add (planet);
			} else if (previousOwner == AbstractPlanet.Ownership.Neutral) {
				enemyPlanets.Add (planet);
			} else { //This condition shouldn't happen
				if (!enemyPlanets.Contains (planet))
					enemyPlanets.Add (planet);
			}
			break;
		case AbstractPlanet.Ownership.Neutral: //This condition shouldn't happen
			if (enemyPlanets.Contains (planet))
				enemyPlanets.Remove (planet);

			if (playerPlanets.Contains (planet))
				playerPlanets.Remove (planet);
			break;
		}
	}

	public AbstractPlanet GetSelectedPlanet() {
		return selectedPlanet;
	}

	public SoldierStats GetPlayerStats() {
		return globalPlayerSoldiersStats;
	}

	public SoldierStats GetEnemyStats() {
		return globalEnemySoldiersStats;
	}

	public void LevelPlayer() {
		if(playerLevel <= 2)
			playerLevel++;

		SetStats (ref globalPlayerSoldiersStats, playerLevel);
	}

	public void LevelEnemy() {
		if (enemyLevel <= 2)
			enemyLevel++;

		SetStats (ref globalEnemySoldiersStats, enemyLevel);
	}

	void SetStats(ref SoldierStats stats, int level) {
		switch (level) {
		case 0:
			stats.defense = 2;
			stats.defenseMod = 0;
			stats.attackMod = 0;
			break;
		case 1:
			stats.defense = 2;
			stats.defenseMod = 1;
			stats.attackMod = 0;
			break;
		case 2:
			stats.defense = 2;
			stats.defenseMod = 2;
			stats.attackMod = 0;
			break;
		default:
			stats.defense = 2;
			stats.defenseMod = 3;
			stats.attackMod = 1;
			break;
		}
	}

	public void PlayerTakeDamage(int damage) {
		playerSoldierCount -= damage;
	}

	public void EnemyTakeDamage(int damage) {
		enemySoldierCount -= damage;
	}

	public void TrainSoldier(AbstractPlanet planet) {
		switch (planet.planetOwnership) {
		case AbstractPlanet.Ownership.Player:
			if (playerResources >= GamePlay.SOLDIER_COST) {
				playerSoldierCount += GamePlay.SOLDIER_UNIT;
				playerResources -= GamePlay.SOLDIER_COST;
				planet.playerSoldiers += GamePlay.SOLDIER_UNIT;
			}
			break;
		case AbstractPlanet.Ownership.Enemy:
			if (enemyResources >= GamePlay.SOLDIER_COST) {
				enemySoldierCount += GamePlay.SOLDIER_UNIT;
				enemyResources -= GamePlay.SOLDIER_COST;
				planet.enemySoldiers += GamePlay.SOLDIER_UNIT;
			}
			break;
		case AbstractPlanet.Ownership.Neutral:
			break;
		}
	}

	public void MineResources (AbstractPlanet.Ownership owner) {
		switch (owner) {
		case AbstractPlanet.Ownership.Player:
			playerResources += GamePlay.RESOURCE_RATE;
			break;
		case AbstractPlanet.Ownership.Enemy:
			enemyResources += GamePlay.RESOURCE_RATE;
			break;
		case AbstractPlanet.Ownership.Neutral:
			break;

		}

	}
}
