using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/**
 * Basic Enemy AI that doesn't look more than 1 step ahead.
 * AI Behaviour:
 * 1. Always create units.
 * 2. Always send soldier units to neighboring planet once ship capacity is met.
 * 3. If Adjacent planet is attacked, then reinforce when ship is at capacity.
 **/
public class BasicEnemyAIScript : MonoBehaviour {

	private List<PlanetScript> planets;
	private ManagerScript gameManager;
	private float thinkTimer;
	private PlanetScript neighboringPlanet;
	private Vector3 launchingPosition;

	// Use this for initialization
	void Start () {
		gameManager = ManagerScript.Instance;
		planets = gameManager.enemyPlanets;
		thinkTimer = 0.0f;
	}

	// Update is called once per frame
	void Update () {

		thinkTimer += Time.deltaTime;
		if (thinkTimer >= AI.THINK_TIME_EASY) {
			UpdatePlanetList ();
			int planetCount = planets.Count;
			for (int i = 0; i < planetCount; i++) {
				PlanetActions (planets [i]);
			}
			thinkTimer = 0;
		}

	}

	void UpdatePlanetList() {
		planets = gameManager.enemyPlanets;
	}

	void PlanetActions(PlanetScript planet) {
		//Always create units
		CreateUnits(planet);
		//Always send units to neighboring planets once ship capacity is met
		//Create ship
		planet.CreateShip(PlanetScript.Ownership.Enemy);
		//Load Units to ship
		planet.LoadSoldiersToShip (planet.ships[Indices.SHIP_ENEMY]);
		//Load Soldiers Units until ship capacity is met
		if (planet.ships[Indices.SHIP_ENEMY].soldiersOnBoard >= planet.ships [Indices.SHIP_ENEMY].soldierCapacity) {
			//Send units to a neighboring planet
			neighboringPlanet = ChoosePlanet(planet);
			//Launch Ship to neighboring planet
			launchingPosition = (neighboringPlanet.transform.position - planet.transform.position) / 2.0f + planet.transform.position;
			LaunchShip (planet, neighboringPlanet, planet.adjacentPaths, planet.ships [Indices.SHIP_ENEMY]);
		}
	}

	void CreateUnits(PlanetScript planet) {
		planet.TrainSoldiers (true);
		planet.TrainEngineers (true);
	}

	void LaunchShip(PlanetScript planet, PlanetScript target, PathScript[] paths, ShipScript ship) {
		PathScript chosenPath = null;
		foreach (PathScript path in paths)
		{
			if (path.IsQualifiedForLaunching(launchingPosition))
			{
				chosenPath = path;
			}
		}
		ship.LaunchShipOnPath (chosenPath, planet.transform, target);
        //TODO remove reference in planetScript to enemyShip
	}

	PlanetScript ChoosePlanet(PlanetScript planet) {
		PlanetScript result = null;
		PlanetScript[] neighPlanets = planet.adjacentPlanet;
		foreach (PlanetScript neighbor in neighPlanets) {
			if (neighbor.planetOwnership != PlanetScript.Ownership.Enemy)
				result = neighbor;
		}
		if(result == null)
			result = planet.adjacentPlanet [0];

		return result;
	}
}
