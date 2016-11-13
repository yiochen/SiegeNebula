using UnityEngine;
using System.Collections;

public class PlanetControlScript : MonoBehaviour {
    bool isLaunching = false;
    Vector3 mouseDownPosition;
    Vector3 launchingDirection;
    PlanetScript planetScript;

    public float minLaunchingDistance = 5.0f;

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
        ExitLaunching();
    }

    void PrepareLaunching()
    {
        isLaunching = true;
        mouseDownPosition = Input.mousePosition;
    }

    void ExitLaunching()
    {
        if (launchingDirection.magnitude > minLaunchingDistance)
        {
            Debug.Log("launched!");
            Launch();
        }
        isLaunching = false;
    }

    void UpdateLaunching()
    {
        launchingDirection = Input.mousePosition - mouseDownPosition;
    }

    void Launch()
    {
        // TODO find the path based on the launching direction. For proof of concept, just choose the first one
        PathScript path = planetScript.adjacentPaths[0];
        // TODO currently default launching the first ship. Later change it to check if the ship is yours.
        ShipScript ship = planetScript.ships[0];
        Debug.Log("Got ship ", ship);
        ship.gameObject.SetActive(true);
        // TODO currently default launching in positive direction, later check if it is reversed.
        ship.LaunchShipOnPath(path, false);
    }
}
