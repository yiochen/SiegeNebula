using UnityEngine;
using System.Collections;
using System.Collections.Generic;
/**
 * Planet Script will include store all the information
 * related to the planet.
 * This includes:
 * 1. Units (Soldiers, Engineers)
 * 2. Ships
 * 3. Resources
 * 4. Planet State
 * 5. Planet Type
 **/

public class PlanetScript : MonoBehaviour {

	public struct SoldierUnit {
		public int soldierCount;
		public int defense;
		public int defenseMod;
		public int attackMod;
	}

	public enum PlanetType {
		Hybrid, Resource, Soldier, Normal
	};

	public enum Ownership {
		Player, Enemy, Neutral
	};

	public bool isContested;
	public PlanetType type;
	public Ownership planetOwnership;

	public SoldierUnit playerSoldiers;
	public SoldierUnit enemySoldiers;
	public int playerEngineerCount = 0;
	public int resourceCount = 1200;
	public PlanetScript[] adjacentPlanet;
    public PathScript[] adjacentPaths;
	public ManagerScript gameManager;

	public ShipScript[] ships = new ShipScript[2]; // Two ship most right now, one for player, one for enermy
	public ShipScript selectedShip;

	public RankingBarScript rankingScript;

	private float timer;
	private float changeTimer;

	public bool isSelected;
	private bool isTrainingSoldiers;
	private bool isTrainingEngineers;
	private const int SHIP_COST = 300;
	private const int SOLDIER_COST = 10;
	private const int ENGINEER_COST = 3;

	private const float PLANET_TICK = 1.5f;
	private const float PLANET_CHANGE = 5.0f;

	// Use this for initialization
	void Start () {
		timer = 0;

		playerSoldiers.soldierCount = 0;
		playerSoldiers.defense = 0;
		playerSoldiers.defenseMod = 0;
		playerSoldiers.attackMod = 0;

		enemySoldiers.soldierCount = 0;
		enemySoldiers.defense = 0;
		enemySoldiers.defenseMod = 0;
		enemySoldiers.attackMod = 0;


		isSelected = false;
		isTrainingSoldiers = false;
		isTrainingEngineers = false;
	}

	// Update is called once per frame
	void Update () {

		isSelected = gameManager.GetSelectedPlanet().Equals (this);

		PlanetStateChanges ();

		if(isSelected)
			HandleKeyboardInput ();

		timer += Time.deltaTime;
		if (timer >= PLANET_TICK) {
			switch (type) {
			case PlanetType.Hybrid:
				MineResources ();
				CreateSoldiers ();
				CreateEngineers ();
				break;
			case PlanetType.Normal:
				break;
			case PlanetType.Resource:
				MineResources ();
				break;
			case PlanetType.Soldier:
				CreateSoldiers ();
				CreateEngineers ();
				break;
			}
			CreateShip ();
			timer = 0;
		}
    }
	/**
	 * This is mostly for testing.
	 * S produces soldiers
	 * E produces engineers
	 * we want to have visual buttons for creating soldiers/engineers
	**/
	void HandleKeyboardInput() {
		if (Input.GetKeyDown(KeyCode.S)) {
			isTrainingSoldiers = !isTrainingSoldiers;
		} else if (Input.GetKeyDown(KeyCode.E)) {
			isTrainingEngineers = !isTrainingEngineers;
		}
	}
	/**
	 * Selections need to be managed by the GameManager.
	 * We should see visual indication of a selection.
	**/
	void OnMouseDown() {
		gameManager.ChangeSelection (this);
	}

	void PlanetStateChanges() {
		switch (planetOwnership) {
		case Ownership.Enemy:
			if (playerSoldiers.soldierCount > 0 && enemySoldiers.soldierCount > 0) {
				changeTimer = 0;
				isContested = true;
			} else if (enemySoldiers.soldierCount == 0 && playerSoldiers.soldierCount > 0) {
				isContested = false;
				changeTimer += Time.deltaTime;
				if (changeTimer >= PLANET_CHANGE) {
					planetOwnership = Ownership.Neutral;
					changeTimer = 0;
				}
			} else {
				isContested = false;
				changeTimer = 0;
			}
			break;
		case Ownership.Neutral:
			if (playerSoldiers.soldierCount > 0 && enemySoldiers.soldierCount > 0) {
				changeTimer = 0;
				isContested = true;
			} else if (playerSoldiers.soldierCount == 0 && enemySoldiers.soldierCount > 0) {
				isContested = false;
				changeTimer += Time.deltaTime;
				if (changeTimer >= PLANET_CHANGE) {
					planetOwnership = Ownership.Enemy;
					changeTimer = 0;
				}
			} else if (enemySoldiers.soldierCount == 0 && playerSoldiers.soldierCount > 0) {
				isContested = false;
				changeTimer += Time.deltaTime;
				if (changeTimer >= PLANET_CHANGE) {
					planetOwnership = Ownership.Player;
					changeTimer = 0;
				}
			} else {
				isContested = false;
				changeTimer = 0;
			}
			break;
		case Ownership.Player:
			if (playerSoldiers.soldierCount > 0 && enemySoldiers.soldierCount > 0) {
				changeTimer = 0;
				isContested = true;
			} else if (playerSoldiers.soldierCount == 0 && enemySoldiers.soldierCount > 0) {
				isContested = false;
				changeTimer += Time.deltaTime;
				if (changeTimer >= PLANET_CHANGE) {
					planetOwnership = Ownership.Neutral;
					changeTimer = 0;
				}
			} else {
				isContested = false;
				changeTimer = 0;
			}
			break;
		}
	}

	void MineResources() {
		if (resourceCount > playerEngineerCount) {
			resourceCount -= playerEngineerCount;
			gameManager.AddToResourceCount (playerEngineerCount);
		} else if(resourceCount > 0 && resourceCount < playerEngineerCount) {
			gameManager.AddToResourceCount (resourceCount);
			resourceCount = 0;
		}
	}

	void CreateShip() {

	}

	void CreateSoldiers() {
		if (isTrainingSoldiers) {
			if (gameManager.GetResourceCount() >= SOLDIER_COST) {
				gameManager.AddToResourceCount (-SOLDIER_COST);
				gameManager.AddToSoldierCount (1);
				playerSoldiers.soldierCount++;
			}
		}
	}

	void CreateEngineers() {
		if (isTrainingEngineers) {
			if (gameManager.GetResourceCount() >= ENGINEER_COST) {
				gameManager.AddToResourceCount (-ENGINEER_COST);
				gameManager.AddToEngineerCount (1);
				playerEngineerCount++;
			}
		}
	}

	void LoadSoldiersToShip() {
		selectedShip.StartLoadingSoldiersToShip (this);
	}

	void StopLoadingSoldiersToShip() {
		selectedShip.StopLoadingSoldiersToShip ();
	}

	void LoadEngineersToShip() {
		selectedShip.StartLoadingEngineersToShip (this);
	}

	void StopLoadingEngineersToShip() {
		selectedShip.StopLoadingEngineersToShip ();
	}

	void UnLoadUnitsFromShip() {
		selectedShip.UnloadShip (this);
	}

}
