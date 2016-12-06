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
	
	[HideInInspector]
	public AbstractPlanet.Ownership shipOwnership;
	public float movementSpeed = 5.0f;
	public int soldierCapacity;
	public int soldiersOnBoard;
    public float sizeChangingDistance = 1.0f;
    public float filledPercentage
    {
        get
        {
            float totalPercent = 0;
            if (soldierCapacity > 0)
            {
                totalPercent += (float)soldiersOnBoard / (float)soldierCapacity;
            }
            return totalPercent;
        }
    }
	private float timer;
	public float loadTimePerUnit = 0.25f;

	public AbstractPlanet dockedPlanet;
	private AbstractPlanet targetPlanet;

    private DirectionalPath travelPath;
	private Renderer shipRenderer;
	private bool isSoldierLoading;
	private bool isUnloading;
	private bool isShipMoving;

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
		shipRenderer = GetComponentInChildren<Renderer> ();
	}

	// Update is called once per frame
	void Update () {
		if (isShipMoving) {
			MoveShip();
			return;
		}

		if (isSoldierLoading) {
			LoadSoldiersToShip ();
		} else if (isUnloading) {
			UnloadShip ();
		}


	}

	void Awake() {
		timer = 0;
		soldierCapacity = GamePlay.SHIP_CAPACITY;
        shipRenderer = GetComponentInChildren<Renderer>();
    }

	public void StartLoadingSoldiersToShip(AbstractPlanet planet) {
		isSoldierLoading = true;
		this.dockedPlanet = planet;
	}

	public void StopLoadingSoldiersToShip() {
		isSoldierLoading = false;
	}

	public void UnloadShip(AbstractPlanet planet) {
		isUnloading = true;
		this.dockedPlanet = planet;
	}

	public Renderer GetShipRenderer() {
		return shipRenderer;
	}

	public void LaunchShipOnPath(PathScript path, Transform from, AbstractPlanet targetPlanet) {
		this.targetPlanet = targetPlanet;
		switch (shipOwnership) {
		case AbstractPlanet.Ownership.Player:
			dockedPlanet.ships[Indices.SHIP_PLAYER] = null;
			break;
		case AbstractPlanet.Ownership.Enemy:
			dockedPlanet.ships[Indices.SHIP_ENEMY] = null;
			break;
		case AbstractPlanet.Ownership.Neutral:
			break;
		}
		isShipMoving = true;
        this.travelPath = path.getDirectionStartingFrom(from);

        this.transform.position = travelPath.shipStart;
        this.transform.rotation = Quaternion.LookRotation(travelPath.shipEnd - travelPath.shipStart);
        this.transform.localScale = Vector3.zero;
	}

    public bool GetIsLoading()
    {
        return isSoldierLoading;
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
				case AbstractPlanet.Ownership.Player:
					unitsToLoad = Mathf.Min (GamePlay.LOAD_UNITS, dockedPlanet.playerSoldiers);
					unitRemain = soldierCapacity - soldiersOnBoard;
					if (unitsToLoad <= unitRemain) {
						soldiersOnBoard += unitsToLoad;
						dockedPlanet.playerSoldiers -= unitsToLoad;
					} else {
						soldiersOnBoard += unitRemain;
						dockedPlanet.playerSoldiers -= unitRemain;
						isSoldierLoading = false;
					}
					break;
				case AbstractPlanet.Ownership.Enemy:
					unitsToLoad = Mathf.Min (GamePlay.LOAD_UNITS, dockedPlanet.enemySoldiers);
					unitRemain = soldierCapacity - soldiersOnBoard;
					if (unitsToLoad <= unitRemain) {
						soldiersOnBoard += unitsToLoad;
						dockedPlanet.enemySoldiers -= unitsToLoad;
					} else {
						soldiersOnBoard += unitRemain;
						dockedPlanet.enemySoldiers -= unitRemain;
						isSoldierLoading = false;
					}
					break;
				case AbstractPlanet.Ownership.Neutral:
					break;
				}
				timer = 0;
			}
		}
	}

	void UnloadShip() {
		switch (shipOwnership) {
		case AbstractPlanet.Ownership.Player:
			dockedPlanet.playerSoldiers += soldiersOnBoard;
			break;
		case AbstractPlanet.Ownership.Enemy:
			dockedPlanet.enemySoldiers += soldiersOnBoard;
			break;
		case AbstractPlanet.Ownership.Neutral:
			break;
		}

		soldiersOnBoard = 0;
		isUnloading = false;
	}

	void MoveShip() {
		isSoldierLoading = false;
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
