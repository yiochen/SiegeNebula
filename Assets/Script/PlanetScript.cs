using UnityEngine;
using System.Collections;
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
	public int resourceCount = 0;
	public GameObject[] adjacentPlanet;
	public ManagerScript gameManager;

	/**
	 * These will be used for keeping track of the ship.
	 * Currently, there is no ship implementation so this is commented to avoid errors.
	**/
	//private ShipScript[] ships;
	//private ShipScript selectedShip;

	// Use this for initialization
	void Start () {

	}

	// Update is called once per frame
	void Update () {
        transform.Rotate(0, 0, 50 * Time.deltaTime);
    }

	void MineResources() {

	}

	void CreateShip() {

	}

	void CreateSoldiers() {

	}

	void CreateEngineers() {

	}
		
	void LoadSoldiersToShip() {

	}

	void UnLoadSoldiersFromShip() {

	}

}
