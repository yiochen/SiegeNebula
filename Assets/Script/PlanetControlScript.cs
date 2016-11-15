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

        // TODO currently default launching the first ship. Later change it to check if the ship is yours.
        ShipScript ship = planetScript.ships[0];
        ship.gameObject.SetActive(true);

        ship.LaunchShipOnPath(path, transform);
    }
}
