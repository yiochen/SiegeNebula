using UnityEngine;
using System.Collections;

public class SoldierPlanetScript : AbstractPlanet {

	// Use this for initialization
	void Start () {
		base.OnActivate ();
		type = PlanetType.Soldier;
	}

	override protected void PlanetUpdates () {
		CreateSoldiers ();
	}

}
