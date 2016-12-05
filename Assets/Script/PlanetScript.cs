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
	private GameObject shipsContainer;

	public SoldierUnit playerSoldiers;
	public SoldierUnit enemySoldiers;

	public PlanetScript[] adjacentPlanet;

    public PathScript[] adjacentPaths;

	public ShipScript[] ships; // Two ship most right now, one for player, one for enermy

	public RankingBarScript rankingScript;

    private ManagerScript gameManager;
    private float timer;
	private float changeTimer;


	public bool isSelected;

    // TODO: set to true for testing only, change to private later
	public bool isTrainingSoldiers;


	// Use this for initialization
	void Start () {
        gameManager = ManagerScript.Instance;
		timer = 0;

		isSelected = false;
		isTrainingSoldiers = false;

        shipsContainer = gameManager.shipContainer;

        adjacentPaths = gameManager.pathManager.GetAdjacentPaths(this);
        adjacentPlanet = gameManager.pathManager.GetAdjacentPlanets(this);
	}

	// Update is called once per frame
	void Update () {

		isSelected = this.Equals(gameManager.GetSelectedPlanet());

		PlanetStateChanges ();

		timer += Time.deltaTime;
		if (timer >= GamePlay.PLANET_TICK) {
			switch (type) {
			case PlanetType.Hybrid:
				MineResources ();
				CreateSoldiers ();
				break;
			case PlanetType.Normal:
				break;
			case PlanetType.Resource:
				MineResources ();
				break;
			case PlanetType.Soldier:
				CreateSoldiers ();
				break;
			}
			timer = 0;
		}
    }

	/**
	 * Selections need to be managed by the GameManager.
	 * We should see visual indication of a selection.
	**/
	void OnMouseDown() {
		gameManager.ChangeSelection (this);
	}

    void SetContested(bool value)
    {
        isContested = value;
        if (isContested)
        {
            ParticleManagerScript.Instance.Play(ParticleList.FIGHTING, transform);
        } else
        {
            ParticleManagerScript.Instance.Stop(transform);
        }
    }
	void PlanetStateChanges() {
		switch (planetOwnership) {
		case Ownership.Enemy:
			if (playerSoldiers.soldierCount > 0 && enemySoldiers.soldierCount > 0) {
				changeTimer = 0;
				SetContested(true);
			} else if (enemySoldiers.soldierCount == 0 && playerSoldiers.soldierCount > 0) {
				SetContested(false);
				changeTimer += Time.deltaTime;
				if (changeTimer >= GamePlay.PLANET_CHANGE) {
					planetOwnership = Ownership.Neutral;
					gameManager.CapturePlanet (Ownership.Enemy, this);
					isTrainingSoldiers = false;
					changeTimer = 0;
				}
			} else {
				SetContested(false);
				changeTimer = 0;
			}
			break;
		case Ownership.Neutral:
			if (playerSoldiers.soldierCount > 0 && enemySoldiers.soldierCount > 0) {
				changeTimer = 0;
				SetContested(true);
			} else if (playerSoldiers.soldierCount == 0 && enemySoldiers.soldierCount > 0) {
				SetContested(false);
				changeTimer += Time.deltaTime;
				if (changeTimer >= GamePlay.PLANET_CHANGE) {
					planetOwnership = Ownership.Enemy;
					gameManager.CapturePlanet (Ownership.Neutral, this);
					isTrainingSoldiers = false;
					changeTimer = 0;
				}
			} else if (enemySoldiers.soldierCount == 0 && playerSoldiers.soldierCount > 0) {
				SetContested(false);
				changeTimer += Time.deltaTime;
				if (changeTimer >= GamePlay.PLANET_CHANGE) {
					planetOwnership = Ownership.Player;
					gameManager.CapturePlanet (Ownership.Neutral, this);
					isTrainingSoldiers = false;
					changeTimer = 0;
				}
			} else {
				SetContested(false);
				changeTimer = 0;
			}
			break;
		case Ownership.Player:
			if (playerSoldiers.soldierCount > 0 && enemySoldiers.soldierCount > 0) {
				changeTimer = 0;
				SetContested(true);
			} else if (playerSoldiers.soldierCount == 0 && enemySoldiers.soldierCount > 0) {
				SetContested(false);
				changeTimer += Time.deltaTime;
				if (changeTimer >= GamePlay.PLANET_CHANGE) {
					planetOwnership = Ownership.Neutral;
					gameManager.CapturePlanet (Ownership.Player, this);
					isTrainingSoldiers = false;
					changeTimer = 0;
				}
			} else {
				SetContested(false);
				changeTimer = 0;
			}
			break;
		}
	}

	void MineResources() {
		gameManager.MineResources (planetOwnership);	
	}

	public ShipScript CreateShip(Ownership ownership) {
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
			ships [index].shipOwnership = Ownership.Enemy;
			break;
		default:
			break;
		}
		ships [index].transform.SetParent (shipsContainer.transform);
		ships [index].GetShipRenderer().enabled = false;
        return ships[index];
	}

    public bool GetIsTrainingSoldiers ()
    {
        return isTrainingSoldiers;
    }

	public void TrainSoldiers(bool isTrue) {
		isTrainingSoldiers = isTrue;
	}

	void CreateSoldiers() {
		if (isTrainingSoldiers) {
			gameManager.TrainSoldier (this);
		}
	}

	public void LoadSoldiersToShip(ShipScript ship) {
		ship.StartLoadingSoldiersToShip (this);
	}

	public void StopLoadingSoldiersToShip(ShipScript ship) {
		ship.StopLoadingSoldiersToShip ();
	}

	public void UnLoadUnitsFromShip(ShipScript ship) {
		ship.UnloadShip (this);
	}

}
