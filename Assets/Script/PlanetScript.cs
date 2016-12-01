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

	public enum PlanetType {
		Hybrid, Resource, Soldier, Normal
	};

	public enum Ownership {
		Player, Enemy, Neutral
	};

	public bool isContested;
	public PlanetType type;
	public Ownership planetOwnership;

	public ShipScript shipPrefab;
	public GameObject shipsContainer;

	public SoldierUnit playerSoldiers;
	public SoldierUnit enemySoldiers;
	public int playerEngineerCount;
	public int enemyEngineerCount;

	public int resourceCount;
	public PlanetScript[] adjacentPlanet;
    public PathScript[] adjacentPaths;

	public ShipScript[] ships; // Two ship most right now, one for player, one for enermy

	public RankingBarScript rankingScript;

    private ManagerScript gameManager;
    private float timer;
	private float changeTimer;


	public bool isSelected;
	private bool isTrainingSoldiers;
	private bool isTrainingEngineers;


	// Use this for initialization
	void Start () {
        gameManager = ManagerScript.Instance;
		timer = 0;

		playerSoldiers.soldierCount = 0;
		playerSoldiers.defense = 0;
		playerSoldiers.defenseMod = 0;
		playerSoldiers.attackMod = 0;

		enemySoldiers.soldierCount = 0;
		enemySoldiers.defense = 0;
		enemySoldiers.defenseMod = 0;
		enemySoldiers.attackMod = 0;

		resourceCount = GamePlay.PLANET_RESOURCE_STD;


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
		if (timer >= GamePlay.PLANET_TICK) {
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
				if (changeTimer >= GamePlay.PLANET_CHANGE) {
					planetOwnership = Ownership.Neutral;
					gameManager.CapturePlanet (Ownership.Enemy, this);
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
				if (changeTimer >= GamePlay.PLANET_CHANGE) {
					planetOwnership = Ownership.Enemy;
					gameManager.CapturePlanet (Ownership.Neutral, this);
					changeTimer = 0;
				}
			} else if (enemySoldiers.soldierCount == 0 && playerSoldiers.soldierCount > 0) {
				isContested = false;
				changeTimer += Time.deltaTime;
				if (changeTimer >= GamePlay.PLANET_CHANGE) {
					planetOwnership = Ownership.Player;
					gameManager.CapturePlanet (Ownership.Neutral, this);
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
				if (changeTimer >= GamePlay.PLANET_CHANGE) {
					planetOwnership = Ownership.Neutral;
					gameManager.CapturePlanet (Ownership.Player, this);
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
		switch (planetOwnership) {
		case Ownership.Player:
			if (resourceCount > playerEngineerCount) {
				resourceCount -= playerEngineerCount;
				gameManager.AddToResourceCount (playerEngineerCount, planetOwnership);
			} else if(resourceCount > 0 && resourceCount < playerEngineerCount) {
				gameManager.AddToResourceCount (resourceCount, planetOwnership);
				resourceCount = 0;
			}
			break;
		case Ownership.Enemy:
			if (resourceCount > enemyEngineerCount) {
				resourceCount -= enemyEngineerCount;
				gameManager.AddToResourceCount (enemyEngineerCount, planetOwnership);
			} else if(resourceCount > 0 && resourceCount < enemyEngineerCount) {
				gameManager.AddToResourceCount (resourceCount, planetOwnership);
				resourceCount = 0;
			}
			break;
		case Ownership.Neutral:
			break;
		}
	}

	public ShipScript CreateShip(Ownership ownership) {
        Debug.Log("creating ship for " + ownership);
        ShipScript ship = null;
		switch (ownership) {
		case Ownership.Player:
			if (ships [Indices.SHIP_PLAYER] == null) {
				switch (planetOwnership) {
				case Ownership.Player:
					ship = ShipInstantiation (Indices.SHIP_PLAYER);
					break;
				default:
					if (playerSoldiers.soldierCount > 0) {
						ship = ShipInstantiation (Indices.SHIP_PLAYER);
					}
					break;
				}
			}
			break;
		case Ownership.Enemy:
			if (ships [Indices.SHIP_ENEMY] == null) {
				switch (planetOwnership) {
				case Ownership.Enemy:
					ship = ShipInstantiation (Indices.SHIP_ENEMY);
					break;
				default:
					if (enemySoldiers.soldierCount > 0) {
						ship = ShipInstantiation (Indices.SHIP_ENEMY);
					}
					break;
				}
			}
			break;
		case Ownership.Neutral: //This shouldn't happen
			break;
		}
        return ship;
	}

	//Helper function
	ShipScript ShipInstantiation(int index) {
		ships [index] = Instantiate (shipPrefab) as ShipScript;
		ships [index].gameObject.SetActive (true);
		switch (index) {
		case Indices.SHIP_PLAYER:
			ships [index].shipOwnership = Ownership.Player;
			break;
		case Indices.SHIP_ENEMY:
			ships [index].shipOwnership = Ownership.Player;
			break;
		default:
			break;
		}
		ships [index].transform.SetParent (shipsContainer.transform);
		ships [index].shipRenderer.enabled = false;
        return ships[index];
	}

	public void TrainSoldiers(bool isTrue) {
		isTrainingSoldiers = isTrue;
	}

	public void TrainEngineers(bool isTrue) {
		isTrainingEngineers = isTrue;
	}

	void CreateSoldiers() {
		if (isTrainingSoldiers) {
			gameManager.TrainSoldier (this);
		}
	}

	void CreateEngineers() {
		if (isTrainingEngineers) {
			gameManager.TrainEngineer (this);
		}
	}

	public void LoadSoldiersToShip(ShipScript ship) {
		ship.StartLoadingSoldiersToShip (this);
	}

	public void StopLoadingSoldiersToShip(ShipScript ship) {
		ship.StopLoadingSoldiersToShip ();
	}

	public void LoadEngineersToShip(ShipScript ship) {
		ship.StartLoadingEngineersToShip (this);
	}

	public void StopLoadingEngineersToShip(ShipScript ship) {
		ship.StopLoadingEngineersToShip ();
	}

	public void UnLoadUnitsFromShip(ShipScript ship) {
		ship.UnloadShip (this);
	}

}
