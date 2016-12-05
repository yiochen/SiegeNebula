using UnityEngine;
using System.Collections;

public class HybridPlanetScript : AbstractPlanet {

	void Start() {
		base.OnActivate ();
		type = PlanetType.Hybrid;
	}
		

	override protected void PlanetUpdates () {
		CreateSoldiers ();
		MineResources ();
	}
}
