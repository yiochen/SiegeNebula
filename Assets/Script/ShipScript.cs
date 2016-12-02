﻿using UnityEngine;
using System.Collections;

/**
 * Class contains all actions regarding ships.
 * This includes:
 * 1. Ship statistics (movement speed, capacity)
 * 2. Ship loading and unloading
 * 3. Ship movement
**/

public class ShipScript : MonoBehaviour {
    [HideInInspector]
	public Renderer shipRenderer; // get this using GetComponentInChildren
	[HideInInspector]
	public PlanetScript.Ownership shipOwnership;
	public float movementSpeed = 5.0f;
	public int soldierCapacity;
	public int engineerCapacity;
	public int soldiersOnBoard;
    public float sizeChangingDistance = 1.0f;
	private int engineersOnBoard;
	private bool isEngineerLoading;
	private bool isSoldierLoading;
	private bool isUnloading;
	private bool isShipMoving;
    public float filledPercentage
    {
        get
        {
            float totalPercent = 0;
            if (soldierCapacity > 0)
            {
                totalPercent += (float)soldiersOnBoard / (float)soldierCapacity;
            }
            if (engineerCapacity > 0)
            {
                totalPercent += (float)engineersOnBoard / (float)engineerCapacity;
            }
            return totalPercent;
        }
    }
	private float timer;
	public float loadTimePerUnit = 0.25f;

	private PlanetScript dockedPlanet;
	private PlanetScript targetPlanet;

    private DirectionalPath travelPath;

    private float remainingDistance
    {
        get
        {
            return Vector3.Distance(transform.position, travelPath.shipEnd);
        }
    }

    private float traveledDistance
    {
        get
        {
            return Vector3.Distance(transform.position, travelPath.shipStart);
        }
    }
	// Use this for initialization
	void Start () {
		timer = 0;
		soldierCapacity = GamePlay.SHIP_CAPACITY;

	}

	// Update is called once per frame
	void Update () {
		if (isShipMoving) {
			MoveShip();
			return;
		}

		if (isSoldierLoading) {
			LoadSoldiersToShip ();
		} else if (isEngineerLoading) {
			LoadEngineersToShip ();
		} else if (isUnloading) {
			UnloadShip ();
		}


	}

	void Awake() {
		timer = 0;
		soldierCapacity = GamePlay.SHIP_CAPACITY;
        shipRenderer = GetComponentInChildren<Renderer>();
    }

	public void StartLoadingSoldiersToShip(PlanetScript planet) {
		isSoldierLoading = true;
		this.dockedPlanet = planet;
	}

	public void StopLoadingSoldiersToShip() {
		isSoldierLoading = false;
	}

	public void StartLoadingEngineersToShip(PlanetScript planet) {
		isEngineerLoading = true;
		this.dockedPlanet = planet;
	}

	public void StopLoadingEngineersToShip() {
		isEngineerLoading = false;
	}

	public void UnloadShip(PlanetScript planet) {
		isUnloading = true;
		this.dockedPlanet = planet;
	}

	public void LaunchShipOnPath(PathScript path, Transform from, PlanetScript targetPlanet) {
		this.targetPlanet = targetPlanet;
		switch (shipOwnership) {
		case PlanetScript.Ownership.Player:
			dockedPlanet.ships[Indices.SHIP_PLAYER] = null;
			break;
		case PlanetScript.Ownership.Enemy:
			dockedPlanet.ships[Indices.SHIP_ENEMY] = null;
			break;
		case PlanetScript.Ownership.Neutral:
			break;
		}
		isShipMoving = true;
        this.travelPath = path.getDirectionStartingFrom(from);

        this.transform.position = travelPath.shipStart;
        this.transform.rotation = Quaternion.LookRotation(travelPath.shipEnd - travelPath.shipStart);
        this.transform.localScale = Vector3.zero;
	}

	void LoadSoldiersToShip() {
		if (soldiersOnBoard >= soldierCapacity) {
			soldiersOnBoard = soldierCapacity;
			isSoldierLoading = false;
		} else {
			timer += Time.deltaTime;
			if (timer >= loadTimePerUnit) {
				int unitsToLoad = 0;
				int unitRemain = 0;
				switch (shipOwnership) {
				case PlanetScript.Ownership.Player:
					unitsToLoad = Mathf.Min (GamePlay.LOAD_UNITS, dockedPlanet.playerSoldiers.soldierCount);
					unitRemain = soldierCapacity - soldiersOnBoard;
					if (unitsToLoad <= unitRemain) {
						soldiersOnBoard += unitsToLoad;
						dockedPlanet.playerSoldiers.soldierCount -= unitsToLoad;
					} else {
						soldiersOnBoard += unitRemain;
						dockedPlanet.playerSoldiers.soldierCount -= unitRemain;
						isSoldierLoading = false;
					}
					break;
				case PlanetScript.Ownership.Enemy:
					unitsToLoad = Mathf.Min (GamePlay.LOAD_UNITS, dockedPlanet.enemySoldiers.soldierCount);
					unitRemain = soldierCapacity - soldiersOnBoard;
					if (unitsToLoad <= unitRemain) {
						soldiersOnBoard += unitsToLoad;
						dockedPlanet.enemySoldiers.soldierCount -= unitsToLoad;
					} else {
						soldiersOnBoard += unitRemain;
						dockedPlanet.enemySoldiers.soldierCount -= unitRemain;
						isSoldierLoading = false;
					}
					break;
				case PlanetScript.Ownership.Neutral:
					break;
				}
				timer = 0;
			}
		}
	}

	void LoadEngineersToShip() {
		if (engineersOnBoard >= engineerCapacity) {
			engineersOnBoard = engineerCapacity;
			isEngineerLoading = false;
		} else {
			timer += Time.deltaTime;
			if (timer >= loadTimePerUnit) {
				switch (shipOwnership) {
				case PlanetScript.Ownership.Player:
					if (dockedPlanet.playerEngineerCount > 0) {
						engineersOnBoard++;
						dockedPlanet.playerEngineerCount--;
					} else {
						isEngineerLoading = false;
					}
					break;
				case PlanetScript.Ownership.Enemy:
					if (dockedPlanet.enemyEngineerCount > 0) {
						engineersOnBoard++;
						dockedPlanet.enemyEngineerCount--;
					} else {
						isEngineerLoading = false;
					}
					break;
				case PlanetScript.Ownership.Neutral:
					break;
				}
				timer = 0;
			}
		}
	}

	void UnloadShip() {
		switch (shipOwnership) {
		case PlanetScript.Ownership.Player:
			dockedPlanet.playerSoldiers.soldierCount += soldiersOnBoard;
			dockedPlanet.playerEngineerCount += engineersOnBoard;
			break;
		case PlanetScript.Ownership.Enemy:
			dockedPlanet.enemySoldiers.soldierCount += soldiersOnBoard;
			dockedPlanet.enemyEngineerCount += engineersOnBoard;
			break;
		case PlanetScript.Ownership.Neutral:
			break;
		}

		soldiersOnBoard = 0;
		engineersOnBoard = 0;
		isUnloading = false;
	}

	void MoveShip() {
		isSoldierLoading = false;
		isEngineerLoading = false;
		isUnloading = false;
		shipRenderer.enabled = true;
        if (traveledDistance <= sizeChangingDistance || remainingDistance <= sizeChangingDistance)
        {
            this.transform.localScale = Mathf.Min(traveledDistance, remainingDistance) / sizeChangingDistance * Vector3.one;
        }

        if (remainingDistance < 0.05f)
        {

			dockedPlanet = targetPlanet;
			UnloadShip ();
			Destroy (this.gameObject);
        }
        else
        {
            transform.position = Vector3.MoveTowards(transform.position, travelPath.end.position, Mathf.Min(movementSpeed * Time.deltaTime, remainingDistance));
        }
	}
}
