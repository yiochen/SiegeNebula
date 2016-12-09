using UnityEngine;
using System.Collections;

public class HybridPlanetScript : AbstractPlanet {

    public ParticleSystem resourceParticle;
    public ParticleSystem soldierParticle;

	void Start() {
		base.OnActivate ();
		type = PlanetType.Hybrid;
	}
		

	override protected void PlanetUpdates () {
		CreateSoldiers ();
		MineResources ();
        if (isTrainingSoldiers)
        {
            soldierParticle.Play();
        } else
        {
            soldierParticle.Stop();
        }
        if (isMiningResources())
        {
            resourceParticle.Play();
        } else
        {
            resourceParticle.Stop();
        }
	}
}
