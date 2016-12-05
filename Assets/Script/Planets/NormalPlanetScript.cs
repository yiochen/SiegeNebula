using UnityEngine;
using System.Collections;

public class NormalPlanetScript : AbstractPlanet {

	// Use this for initialization
	void Start () {
		base.OnActivate ();
		type = PlanetType.Normal;
	}

	override protected void PlanetUpdates () { }

}
