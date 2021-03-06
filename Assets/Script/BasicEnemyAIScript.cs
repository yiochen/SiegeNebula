﻿using UnityEngine;
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

	private List<AbstractPlanet> planets;
	private ManagerScript gameManager;
	private float thinkTimer;
	private AbstractPlanet neighboringPlanet;

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

	void PlanetActions(AbstractPlanet planet) {
		//Always create units
		CreateUnits(planet);
		//Always send units to neighboring planets once ship capacity is met
		//Create ship
		planet.CreateShip(AbstractPlanet.Ownership.Enemy);
		//Load Units to ship
		planet.LoadSoldiersToShip (planet.ships[Indices.SHIP_ENEMY]);
		//Load Soldiers Units until ship capacity is met
		if ((planet.enemySoldiers == 0 && planet.ships[Indices.SHIP_ENEMY].soldiersOnBoard > 0) || 
			planet.ships[Indices.SHIP_ENEMY].soldiersOnBoard >= planet.ships [Indices.SHIP_ENEMY].soldierCapacity) {
			//Send units to a neighboring planet
			neighboringPlanet = ChoosePlanet(planet);
			//Launch Ship to neighboring planet
			LaunchShip (planet, neighboringPlanet, planet.adjacentPaths, planet.ships [Indices.SHIP_ENEMY]);
		}
	}

	void CreateUnits(AbstractPlanet planet) {
		planet.TrainSoldiers (true);
	}

	void LaunchShip(AbstractPlanet planet, AbstractPlanet target, PathScript[] paths, ShipScript ship) {
		PathScript chosenPath = null;
		foreach (PathScript path in paths)
		{
			if ((path.start == planet.transform && path.end == target.transform) ||
				(path.end == planet.transform && path.start == target.transform)) {
				chosenPath = path;
				break;
			}

		}
		ship.LaunchShipOnPath (chosenPath, planet.transform, target);
        //TODO remove reference in planetScript to enemyShip
	}

	AbstractPlanet ChoosePlanet(AbstractPlanet planet) {
		AbstractPlanet result = null;
		AbstractPlanet[] neighPlanets = planet.adjacentPlanet;
		foreach (AbstractPlanet neighbor in neighPlanets) {
			if (neighbor.planetOwnership != AbstractPlanet.Ownership.Enemy)
				result = neighbor;
		}
		if(result == null)
			result = planet.adjacentPlanet [0];

		return result;
	}
}
