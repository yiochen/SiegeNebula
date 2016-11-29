using UnityEngine;
using System.Collections;

/**
 * Class contains all actions regarding ships.
 * This includes:
 * 1. Ship statistics (movement speed, capacity)
 * 2. Ship loading and unloading
 * 3. Ship movement
**/

public class ShipScript : MonoBehaviour {

	public float movementSpeed;
	public int soldierCapacity;
	public int engineerCapacity;
	public int soldiersOnBoard;
    public float sizeChangingDistance = 1.0f;
	private int engineersOnBoard;
	private bool isEngineerLoading;
	private bool isSoldierLoading;
	private bool isUnloading;
	private bool isShipMoving;

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

	public void LaunchShipOnPath(PathScript path, Transform from) {
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
				switch (dockedPlanet.planetOwnership) {
				case PlanetScript.Ownership.Player:
					if (dockedPlanet.playerSoldiers.soldierCount > 0) {
						soldiersOnBoard++;
						dockedPlanet.playerSoldiers.soldierCount--;
					} else {
						isSoldierLoading = false;
					}
					break;
				case PlanetScript.Ownership.Enemy:
					if (dockedPlanet.enemySoldiers.soldierCount > 0) {
						soldiersOnBoard++;
						dockedPlanet.enemySoldiers.soldierCount--;
					} else {
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
				switch (dockedPlanet.planetOwnership) {
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
		switch (dockedPlanet.planetOwnership) {
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
        //TODO: Add implementation for moving to another planet
        if (traveledDistance <= sizeChangingDistance || remainingDistance <= sizeChangingDistance)
        {
            this.transform.localScale = Mathf.Min(traveledDistance, remainingDistance) / sizeChangingDistance * Vector3.one;
        }

        if (remainingDistance < 0.05f)
        {
            isShipMoving = false;
        }
        else
        {
            transform.position = Vector3.MoveTowards(transform.position, travelPath.end.position, Mathf.Min(movementSpeed * Time.deltaTime, remainingDistance));
        }
	}
}
