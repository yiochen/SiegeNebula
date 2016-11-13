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

	public void LaunchShipOnPath(PathScript path, bool isReverse) {
		isShipMoving = true;
        // Note from Yiou: The target planet might not be used since I plan on using the PathScript to handle movement
        if (isReverse)
        {
            this.targetPlanet = path.start.GetComponent<PlanetScript>();
        } else
        {
            this.targetPlanet = path.end.GetComponent<PlanetScript>();
        }

        this.travelPath = path.getDirectionalPath(isReverse);

        this.transform.position = path.start.position;
        this.transform.rotation = Quaternion.LookRotation(path.end.position - path.start.position);

	}

	void LoadSoldiersToShip() {
		if (soldiersOnBoard >= soldierCapacity) {
			soldiersOnBoard = soldierCapacity;
			isSoldierLoading = false;
		} else {
			timer += Time.deltaTime;
			if (timer >= loadTimePerUnit) {
				if (dockedPlanet.soldierCount > 0) {
					soldiersOnBoard++;
					dockedPlanet.soldierCount--;
				} else {
					isSoldierLoading = false;
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
				if (dockedPlanet.engineerCount > 0) {
					engineersOnBoard++;
					dockedPlanet.engineerCount--;
				} else {
					isEngineerLoading = false;
				}
				timer = 0;
			}
		}
	}

	void UnloadShip() {
		dockedPlanet.soldierCount += soldiersOnBoard;
		dockedPlanet.engineerCount += engineersOnBoard;

		soldiersOnBoard = 0;
		engineersOnBoard = 0;
		isUnloading = false;
	}

	void MoveShip() {
        //TODO: Add implementation for moving to another planet
        float remainingDistance = Vector3.Distance(transform.position, travelPath.end.position);
        if (remainingDistance < 0.05f)
        {
            Debug.Log("Arrived at " + transform.position);
            isShipMoving = false;
        }
        else
        {
            transform.position = Vector3.MoveTowards(transform.position, travelPath.end.position, Mathf.Min(movementSpeed * Time.deltaTime, remainingDistance));
        }
	}
}
