using UnityEngine;
using System.Collections;

public class PlanetControlScript : MonoBehaviour {
    bool isLaunching = false;
    Vector3 mouseDownPosition;
    Vector3 launchingDirection;
    AbstractPlanet planetScript;

    public float minLaunchingDistance = 1.0f;

    private PathScript potentialPath = null;
	// Use this for initialization
	void Start () {
        planetScript = GetComponent<AbstractPlanet>();
	}

	// Update is called once per frame
	void Update () {
        PathScript potentialPath = null;
        if (isLaunching)
        {
            UpdateLaunching();

            potentialPath = GetPotentiallyQualifiedPath(EventManagerScript.GetMouseInWorldPosition());
     
            if (potentialPath != null)
            {
                potentialPath.DisplayVisualHint(transform);
            }
        }

        if (this.potentialPath != null && this.potentialPath != potentialPath)
        {
            this.potentialPath.StopVisualHint();
        }

        this.potentialPath = potentialPath;

	}

    void OnMouseDown()
    {
        Debug.Log("mouse down");
        if (planetScript.ships[Indices.SHIP_PLAYER] != null)
        {
            PrepareLaunching();
        }
    }

    void OnMouseUp()
    {
        ExitLaunching(EventManagerScript.GetMouseInWorldPosition());
    }

    void PrepareLaunching()
    {
		isLaunching = true;
		mouseDownPosition = Input.mousePosition;
    }

    PathScript GetPotentiallyQualifiedPath(Vector3 launchingPosition)
    {
        // iterate over all paths, find the one that is most close to mouse point (launchingPosition)
        PathScript[] paths = planetScript.adjacentPaths;
        PathScript chosenPath = null;
        foreach (PathScript path in paths)
        {
            if (path.isMouseCloseEnough(launchingPosition, transform))
            {
                chosenPath = path;
            }
        }
        return chosenPath;
    }

    void ExitLaunching(Vector3 launchingPosition)
    {
        if (launchingDirection.magnitude > minLaunchingDistance)
        {
            if (potentialPath != null && planetScript.ships[Indices.SHIP_PLAYER] != null)
            {
                Launch(potentialPath);
            }

        }
        isLaunching = false;
    }

    void UpdateLaunching()
    {
        launchingDirection = Input.mousePosition - mouseDownPosition;
    }

    void Launch(PathScript path)
    {
		ShipScript ship = planetScript.ships [Indices.SHIP_PLAYER];
		ship.gameObject.SetActive (true);
        AbstractPlanet targetPlanet = path.getDirectionStartingFrom(transform).end.gameObject.GetComponent<AbstractPlanet>();
        if (targetPlanet != null)
        {
            ship.LaunchShipOnPath(path, transform, targetPlanet);
        }

    }
}
