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

	public enum PlanetState {
		Player, Enemy, Contested, Neutral
	};

	public enum PlanetType {
		Hybrid, Resource, Soldier, Normal
	};

	public PlanetState state;
	public PlanetType type;
	public int soldierCount = 0;
	public int engineerCount = 0;
	public int resourceCount = 1200;
	public GameObject[] adjacentPlanet;
    public PathScript[] adjacentPaths;
	public ManagerScript gameManager;

	public ShipScript[] ships = new ShipScript[2]; // Two ship most right now, one for player, one for enermy
	public ShipScript selectedShip;

	private float timer;
	public float planetTick = 1.5f;

	public bool isSelected;
	private bool isTrainingSoldiers;
	private bool isTrainingEngineers;
	private const int SHIP_COST = 300;
	private const int SOLDIER_COST = 10;
	private const int ENGINEER_COST = 3;

	// Use this for initialization
	void Start () {
		timer = 0;
		isSelected = false;
		isTrainingSoldiers = false;
		isTrainingEngineers = false;
	}

	// Update is called once per frame
	void Update () {

		if(isSelected)
			HandleKeyboardInput ();

		timer += Time.deltaTime;
		if (timer >= planetTick) {
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
		isSelected = true;
	}

	void MineResources() {
		if (resourceCount > engineerCount) {
			resourceCount -= engineerCount;
			gameManager.AddToResourceCount (engineerCount);
		} else if(resourceCount > 0 && resourceCount < engineerCount) {
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
				soldierCount++;
			}
		}
	}

	void CreateEngineers() {
		if (isTrainingEngineers) {
			if (gameManager.GetResourceCount() >= ENGINEER_COST) {
				gameManager.AddToResourceCount (-ENGINEER_COST);
				gameManager.AddToEngineerCount (1);
				engineerCount++;
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
