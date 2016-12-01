using UnityEngine;
using System.Collections;

public class PlanetControlScript : MonoBehaviour {
    bool isLaunching = false;
    Vector3 mouseDownPosition;
    Vector3 launchingDirection;
    PlanetScript planetScript;

    public float minLaunchingDistance = 5.0f;
    public float pathChosingThreshold = 0.5f; // how far can player drop the ship away from the path

	// Use this for initialization
	void Start () {
        planetScript = GetComponent<PlanetScript>();
	}

	// Update is called once per frame
	void Update () {
        if (isLaunching)
        {
            UpdateLaunching();
        }
	}

    void OnMouseDown()
    {
        PrepareLaunching();
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

    void ExitLaunching(Vector3 launchingPosition)
    {
        if (launchingDirection.magnitude > minLaunchingDistance)
        {
            // iterate over all paths, find the one that is most close to mouse point (launchingPosition)
            PathScript[] paths = planetScript.adjacentPaths;
            PathScript chosenPath = null;
            foreach (PathScript path in paths)
            {
                if (path.IsQualifiedForLaunching(launchingPosition))
                {
                    chosenPath = path;
                }
            }
            if (chosenPath)
            {
                Launch(chosenPath);
            }

        } else
        {
            Debug.Log("Not far enough");
        }
        isLaunching = false;
    }

    void UpdateLaunching()
    {
        launchingDirection = Input.mousePosition - mouseDownPosition;
    }

    void Launch(PathScript path)
    {
		if (planetScript.ships [Indices.SHIP_PLAYER]) {
			ShipScript ship = planetScript.ships [Indices.SHIP_PLAYER];
			ship.gameObject.SetActive (true);
            PlanetScript targetPlanet = path.getDirectionStartingFrom(transform).end.gameObject.GetComponent<PlanetScript>();
            if (targetPlanet != null)
            {
                ship.LaunchShipOnPath(path, transform, targetPlanet);
                planetScript.ships[Indices.SHIP_PLAYER] = null;
            }

		}
    }
}
