﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class StrongAI : MonoBehaviour {

	private List<AbstractPlanet> planets;
	private ManagerScript gameManager;
	private AbstractPlanet neighboringPlanet;
	private Vector3 launchingPosition;

	private float thinkTimer;
	private int action;
	private bool actionHappened;
	private const int ACTIONS_LIMIT = 4;
	private ShipScript ship;


	// Use this for initialization
	void Start () {
		gameManager = ManagerScript.Instance;
		planets = gameManager.enemyPlanets;
		thinkTimer = 0.0f;
		action = 0;
		actionHappened = false;
	}
	
	// Update is called once per frame
	void Update () {
		thinkTimer += Time.deltaTime;
		if (thinkTimer >= AI.THINK_TIME_EASY) {
			for(int i = 0; i < planets.Count; i++) {
				AbstractPlanet planet = planets [i];
				actionHappened = false;
				//Do nothing else if you have no more actions
				if (action > ACTIONS_LIMIT)
					continue;
				//Am I being attacked
				BeingAttackedAction(ref planet);

				if (actionHappened)
					continue;
				//Is there a player adajacent to me
				PlayerNearAction (ref planet);

				if (actionHappened)
					continue;

				planet.isRequestingSoldiers = false;
				//Is a planet next to me being attacked
				NeighborAttackedAction (ref planet);

				if (actionHappened)
					continue;

				planet.isFeeding = false;
				//Does my neighbor need soldiers
				NeighborNeedsSoldiersAction (ref planet);

				if (actionHappened)
					continue;

				planet.isFeeding = false;
				//Do I want to expand
				ExpandAction (ref planet);

				if (actionHappened)
					continue;

				//Can I train soldiers with this economy
				if (CanNewUnitsBeCreated ()) {
					planet.TrainSoldiers (true);
					action++;
				} else
					planet.TrainSoldiers (false);
			}
				
			thinkTimer = 0;
			action = 0;
		}
	}

	void ExpandAction(ref AbstractPlanet planet) {
		foreach (AbstractPlanet neigh in planet.adjacentPlanet) {
			if (actionHappened)
				continue;
			if (neigh.planetOwnership == AbstractPlanet.Ownership.Neutral && neigh.isContested == false) {
				planet.TrainSoldiers (true);
				ship = planet.CreateShip (AbstractPlanet.Ownership.Enemy);
				if (ship.soldiersOnBoard >= ship.soldierCapacity * 0.1f ||
					(!CanNewUnitsBeCreated () && ship.soldiersOnBoard > 0)) {
					LaunchShip (planet, neigh, planet.adjacentPaths, ship);
					planet.TrainSoldiers (false);
				}else if (!ship.GetIsLoading ()) {
					ship.StartLoadingSoldiersToShip (planet);
				} 
				action++;
				actionHappened = true;
			}
		}
	}

	void NeighborNeedsSoldiersAction(ref AbstractPlanet planet) {
		foreach (AbstractPlanet neigh in planet.adjacentPlanet) {
			if (actionHappened)
				continue;

			if (neigh.planetOwnership == AbstractPlanet.Ownership.Enemy && IsPlanetRequestingSoldiers(neigh)) {
				planet.TrainSoldiers (true);
				planet.isFeeding = true;

				ship = planet.CreateShip (AbstractPlanet.Ownership.Enemy);
				if (ship.soldiersOnBoard >= ship.soldierCapacity * 0.1f || 
					(!CanNewUnitsBeCreated () && ship.soldiersOnBoard > 0)) {
					LaunchShip (planet, neigh, planet.adjacentPaths, ship);
				} else if (!ship.GetIsLoading ()) {
					ship.StartLoadingSoldiersToShip (planet);
				}
				action++;
				actionHappened = true;
			}
		}
	}

	void NeighborAttackedAction(ref AbstractPlanet planet) {
		foreach (AbstractPlanet neigh in planet.adjacentPlanet) {
			if (actionHappened)
				continue;

			if (neigh.isContested && planet.planetOwnership == AbstractPlanet.Ownership.Enemy) { //Is my neighbor planet being attacked
				planet.TrainSoldiers (true);
				planet.isFeeding = true;
				float totalUnits = neigh.enemySoldiers + neigh.playerSoldiers;
				if (neigh.playerSoldiers / totalUnits > 0.50f) {
					ship = planet.CreateShip (AbstractPlanet.Ownership.Enemy);
					if (ship.soldiersOnBoard == ship.soldierCapacity || 
						(!CanNewUnitsBeCreated () && ship.soldiersOnBoard > 0)) {
						LaunchShip (planet, neigh, planet.adjacentPaths, ship);
					} else if (!ship.GetIsLoading ()) {
						ship.StartLoadingSoldiersToShip (planet);
					} 
				}
				action++;
				actionHappened = true;
			}
		}
	}

	void PlayerNearAction(ref AbstractPlanet planet) {
		foreach (AbstractPlanet neigh in planet.adjacentPlanet) {
			if (actionHappened)
				continue;

			if (neigh.planetOwnership == AbstractPlanet.Ownership.Player) { //Is there an enemy next to me
				planet.TrainSoldiers (true);
				planet.isRequestingSoldiers = true;
				//Am I stronger then the enemy, then attack
				int estimatedUnits = (int)Mathf.Max(neigh.rankingScript.currentRank * 50 - 25, 0);

				ship = planet.CreateShip (AbstractPlanet.Ownership.Enemy);
				if ((ship.soldiersOnBoard + planet.enemySoldiers) > (estimatedUnits)) {
					if (ship.soldiersOnBoard == ship.soldierCapacity || ship.soldiersOnBoard > estimatedUnits) {
						LaunchShip (planet, neigh, planet.adjacentPaths, ship);
					} else if (!ship.GetIsLoading ()) {
						ship.StartLoadingSoldiersToShip (planet);
					}
				} else {
					ship.StopLoadingSoldiersToShip ();
				}
				action++;
				actionHappened = true;
			}
		}
	}

	void BeingAttackedAction(ref AbstractPlanet planet) {
		if (planet.isContested) {
			planet.TrainSoldiers (true);
			planet.isRequestingSoldiers = true;
			//Unload my ship if I have one
			ship = planet.ships [Indices.SHIP_ENEMY];
			if (ship) {
				ship.StopLoadingSoldiersToShip ();
				//Send units to nearest neighbor
				if (ship.soldiersOnBoard > 0) {
					AbstractPlanet target = NearestNeighbor(planet);
					LaunchShip (planet, target, planet.adjacentPaths, ship);
				}
			}
			action++;
			actionHappened = true;
		} else {
			planet.isRequestingSoldiers = false;
		}
	}

	AbstractPlanet NearestNeighbor(AbstractPlanet planet) {
		foreach (AbstractPlanet neigh in planet.adjacentPlanet) {
			if (neigh.planetOwnership == AbstractPlanet.Ownership.Enemy)
				return neigh;
		}
		return planet.adjacentPlanet[0];
	}

	bool CanNewUnitsBeCreated() {
		int resourcePlanets = 0;
		int trainingPlanets = 0;
		for(int i = 0; i < planets.Count; i++) {
			if (planets[i].GetPlanetType () == AbstractPlanet.PlanetType.Resource || planets[i].GetPlanetType () == AbstractPlanet.PlanetType.Hybrid)
				resourcePlanets++;

			if (planets[i].isTrainingSoldiers)
				trainingPlanets++;
		}
		if (2 * trainingPlanets < resourcePlanets || gameManager.enemyResources > 20)
			return true;
		else
			return false;
			
	}

	bool IsPlanetRequestingSoldiers(AbstractPlanet planet) {
		return planet.isFeeding || planet.isRequestingSoldiers;
	}

	void UpdatePlanetList() {
		planets = gameManager.enemyPlanets;
	}

	void LaunchShip(AbstractPlanet planet, AbstractPlanet target, PathScript[] paths, ShipScript ship) {
		launchingPosition = (target.transform.position - planet.transform.position) / 2.0f + planet.transform.position;
		PathScript chosenPath = null;
		foreach (PathScript path in paths)
		{
			if (path.IsQualifiedForLaunching(launchingPosition))
			{
				chosenPath = path;
			}
		}
		ship.LaunchShipOnPath (chosenPath, planet.transform, target);
	}
}