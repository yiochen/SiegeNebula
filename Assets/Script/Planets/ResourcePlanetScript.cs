using UnityEngine;
using System.Collections;

public class ResourcePlanetScript : AbstractPlanet {

	// Use this for initialization
	void Start () {
		base.OnActivate ();
		type = PlanetType.Resource;
	}

	override protected void PlanetUpdates () {
		MineResources ();
	}
}
