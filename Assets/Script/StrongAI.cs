using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class StrongAI : MonoBehaviour {

	private List<AbstractPlanet> enemyPlanets;
	private ManagerScript gameManager;
	private AbstractPlanet neighboringPlanet;
	private List<AbstractPlanet> searched = new List<AbstractPlanet> ();

	private float thinkTimer;
	private int action;
	private bool actionHappened;
	private ShipScript ship;
	private ShipScript[] ships;

	[Range(1, 6)]
	public int actionsLimit = 4;
	[Range(3.0f, 15.0f)]
	public float thinkTime = AI.THINK_TIME_EASY;

	// Use this for initialization
	void Start () {
		gameManager = ManagerScript.Instance;
		enemyPlanets = gameManager.enemyPlanets;
		thinkTimer = 0.0f;
		action = 0;
		actionHappened = false;
	}
	
	// Update is called once per frame
	void Update () {
		thinkTimer += Time.deltaTime;
		if (thinkTimer >= thinkTime) {
			//Check if I have a ship with units doing nothing
			ships = gameManager.shipContainer.gameObject.GetComponentsInChildren<ShipScript>();
			foreach (ShipScript s in ships) {
				if (s.shipOwnership == AbstractPlanet.Ownership.Enemy) {
					if (s.dockedPlanet != null && s.dockedPlanet.planetOwnership != AbstractPlanet.Ownership.Enemy) {
						//Send to neighboring planet with this order: Enemy => Neutral => Player
						AbstractPlanet docked = s.dockedPlanet;
						AbstractPlanet bestPlanet = null;
						foreach (AbstractPlanet planet in docked.adjacentPlanet) {
							if (planet.planetOwnership == AbstractPlanet.Ownership.Enemy) {
								bestPlanet = planet;
								break;
							} else if (planet.planetOwnership == AbstractPlanet.Ownership.Neutral) {
								bestPlanet = planet;
							}
						}
						if (bestPlanet == null)
							bestPlanet = docked.adjacentPlanet [0];

						LaunchShip (docked, bestPlanet, docked.adjacentPaths, s);
					}
				}
			}
			//Enemy Planet Actions
			for(int i = 0; i < enemyPlanets.Count; i++) {
				AbstractPlanet planet = enemyPlanets [i];
				actionHappened = false;
				//Do nothing else if you have no more actions
				if (action > actionsLimit)
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
				if (ship.soldiersOnBoard >= 1 ||
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
			AbstractPlanet target = null;
			if (neigh.planetOwnership == AbstractPlanet.Ownership.Enemy && neigh.isRequestingSoldiers) {
				planet.TrainSoldiers (true);
				planet.isFeeding = true;
				planet.planetRequesting = neigh;
				target = neigh;
			} else if (neigh.planetOwnership == AbstractPlanet.Ownership.Enemy && neigh.isFeeding) {
				if (PlanetInPath (neigh, neigh.planetRequesting)) {
					planet.TrainSoldiers (true);
					planet.isFeeding = true;
					planet.planetRequesting = neigh.planetRequesting;
					target = neigh;				
				}
			}
			if (target) {
				ship = planet.CreateShip (AbstractPlanet.Ownership.Enemy);
				if (ship.soldiersOnBoard >= ship.soldierCapacity * 0.1f || 
					(!CanNewUnitsBeCreated () && ship.soldiersOnBoard > 0)) {
					LaunchShip (planet, target, planet.adjacentPaths, ship);
				} else if (!ship.GetIsLoading ()) {
					ship.StartLoadingSoldiersToShip (planet);
				}
				action++;
				actionHappened = true;
			}
		}
	}

	bool PlanetInPath(AbstractPlanet planet, AbstractPlanet target) {
		searched.Clear ();
		int val = RecursivePlanetFind (null, planet, target);
		if (val > 0)
			return true;
		return false;
	}

	int RecursivePlanetFind(AbstractPlanet prevPlanet, AbstractPlanet currentPlanet, AbstractPlanet target) {
		int sum = 0;
		if (searched.Contains (currentPlanet)) {
			return 0;
		} else {
			searched.Add (currentPlanet);
		}
		
		if (currentPlanet == target) {
			return 1;
		} else if (!currentPlanet.isFeeding) {
			return 0;
		}
		else {
			for (int i = 0; i < currentPlanet.adjacentPlanet.Length; i++) {
				int val = RecursivePlanetFind (currentPlanet, currentPlanet.adjacentPlanet [i], target);
				sum += val;
				if (sum > 0)
					return sum;
			}
			return sum;
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
		for(int i = 0; i < enemyPlanets.Count; i++) {
			if (enemyPlanets[i].GetPlanetType () == AbstractPlanet.PlanetType.Resource || enemyPlanets[i].GetPlanetType () == AbstractPlanet.PlanetType.Hybrid)
				resourcePlanets++;

			if (enemyPlanets[i].isTrainingSoldiers)
				trainingPlanets++;
		}
		if (2 * trainingPlanets < resourcePlanets || gameManager.enemyResources > 20)
			return true;
		else
			return false;
			
	}

	void UpdatePlanetList() {
		enemyPlanets = gameManager.enemyPlanets;
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
	}
}
