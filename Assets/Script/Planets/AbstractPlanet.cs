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


public abstract class AbstractPlanet : MonoBehaviour {

	public enum PlanetType {
		Hybrid, Resource, Soldier, Normal, Reactor
	};

	public enum Ownership {
		Player, Enemy, Neutral
	};

	public bool isContested;
	protected PlanetType type;
	public Ownership planetOwnership;

	private GameObject shipsContainer;

	public int playerSoldiers;
	public int enemySoldiers;

	public AbstractPlanet[] adjacentPlanet;

    public PathScript[] adjacentPaths;

	public ShipScript[] ships; // Two ship most right now, one for player, one for enermy

	public RankingBarScript rankingScript;

    private ManagerScript gameManager;
    private float timer;
	private float changeTimer;


	public bool isSelected;

    // TODO: set to true for testing only, change to private later
	public bool isTrainingSoldiers;

	protected void OnActivate() {
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
		//Debug.Log ("Calling Update");
		isSelected = this.Equals(gameManager.GetSelectedPlanet());

		PlanetStateChanges ();

		timer += Time.deltaTime;
		if (timer >= GamePlay.PLANET_TICK) {
			PlanetUpdates ();
			timer = 0;
		}
    }

	abstract protected void PlanetUpdates();

	public PlanetType GetPlanetType() {
		return type;
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
			if (playerSoldiers > 0 && enemySoldiers > 0) {
				changeTimer = 0;
				SetContested(true);
			} else if (enemySoldiers == 0 && playerSoldiers > 0) {
				SetContested(false);
				changeTimer += Time.deltaTime;
				if (changeTimer >= GamePlay.PLANET_CHANGE) {
					ChangePlanetOwnership (Ownership.Enemy, Ownership.Neutral);
				}
			} else {
				SetContested(false);
				changeTimer = 0;
			}
			break;
		case Ownership.Neutral:
			if (playerSoldiers > 0 && enemySoldiers > 0) {
				changeTimer = 0;
				SetContested(true);
			} else if (playerSoldiers == 0 && enemySoldiers > 0) {
				SetContested(false);
				changeTimer += Time.deltaTime;
				if (changeTimer >= GamePlay.PLANET_CHANGE) {
					ChangePlanetOwnership (Ownership.Neutral, Ownership.Enemy);
				}
			} else if (enemySoldiers == 0 && playerSoldiers > 0) {
				SetContested(false);
				changeTimer += Time.deltaTime;
				if (changeTimer >= GamePlay.PLANET_CHANGE) {
					ChangePlanetOwnership (Ownership.Neutral, Ownership.Player);
				}
			} else {
				SetContested(false);
				changeTimer = 0;
			}
			break;
		case Ownership.Player:
			if (playerSoldiers > 0 && enemySoldiers > 0) {
				changeTimer = 0;
				SetContested(true);
			} else if (playerSoldiers == 0 && enemySoldiers > 0) {
				SetContested(false);
				changeTimer += Time.deltaTime;
				if (changeTimer >= GamePlay.PLANET_CHANGE) {
					ChangePlanetOwnership (Ownership.Player, Ownership.Neutral);
				}
			} else {
				SetContested(false);
				changeTimer = 0;
			}
			break;
		}
	}

	void ChangePlanetOwnership(Ownership oldOwn, Ownership newOwn) {
		planetOwnership = newOwn;
		gameManager.CapturePlanet (oldOwn, this);
		isTrainingSoldiers = false;
		changeTimer = 0;
	}

	protected void MineResources() {
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
					if (playerSoldiers > 0) {
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
					if (enemySoldiers > 0) {
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
		switch (index) {
		case Indices.SHIP_PLAYER:
			ships [index] = ShipManagerScript.Instance.CreatePlayerShip();
			break;
		case Indices.SHIP_ENEMY:
			ships [index] = ShipManagerScript.Instance.CreateEnemyShip();
			break;
		default:
			break;
		}
        return ships[index];
	}

    public bool GetIsTrainingSoldiers ()
    {
        return isTrainingSoldiers;
    }

	public void TrainSoldiers(bool isTrue) {
		isTrainingSoldiers = isTrue;
	}

	protected void CreateSoldiers() {
		if (isTrainingSoldiers) {
			gameManager.TrainSoldier (this);
		}
	}

	public void PlayerTakeDamage(int damage) {
		int oldSolders = playerSoldiers;
		playerSoldiers -= damage;
		if (playerSoldiers < 0)
			playerSoldiers = 0;
		int damageTaken = oldSolders - playerSoldiers;
		gameManager.PlayerTakeDamage (damageTaken);
	}

	public void EnemyTakeDamage(int damage) {
		int oldSolders = enemySoldiers;
		enemySoldiers -= damage;
		if (enemySoldiers < 0)
			enemySoldiers = 0;
		int damageTaken = oldSolders - enemySoldiers;
		gameManager.EnemyTakeDamage (damageTaken);
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
