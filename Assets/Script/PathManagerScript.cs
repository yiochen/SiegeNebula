using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PathManagerScript : Singleton<PathManagerScript> {

    public List<PathScript> pathList = new List<PathScript>();
    public PathScript[] pathsInInspector;
    public GameObject pathArrow;

	// Use this for initialization
	void Awake () {
	    foreach (Transform child in transform) {
            PathScript path = child.GetComponent<PathScript>();
            if (path != null)
            pathList.Add(path);
        }
        pathsInInspector = pathList.ToArray();

	}

	public PathScript[] GetAdjacentPaths(AbstractPlanet planet)
    {

        List<PathScript> paths = new List<PathScript>();

        foreach (PathScript path in this.pathList)
        {
            if (path.start == planet.transform || path.end ==planet.transform)
            {
                paths.Add(path);
            }

        }

        return paths.ToArray();
    }

    public AbstractPlanet[] GetAdjacentPlanets(AbstractPlanet planet)
    {
        List<AbstractPlanet> planets = new List<AbstractPlanet>();
        foreach (PathScript path in this.pathList)
        {
            if (path.start == planet.transform)
            {
                planets.Add(path.end.gameObject.GetComponent<AbstractPlanet>());
            } else if (path.end == planet.transform)
            {
                planets.Add(path.start.gameObject.GetComponent<AbstractPlanet>());
            }
        }
        return planets.ToArray();
    }


}
