using UnityEngine;
using System.Collections;

public class SoldierPlanetScript : AbstractPlanet {
    public ParticleSystem particle;

	// Use this for initialization
	void Start () {
		base.OnActivate ();
		type = PlanetType.Soldier;
	}

	override protected void PlanetUpdates () {
		CreateSoldiers ();
        if (isTrainingSoldiers)
        {
            particle.Play();
        } else
        {
            particle.Stop();
        }
	}

}
