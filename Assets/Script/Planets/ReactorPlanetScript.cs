using UnityEngine;
using System.Collections;
using System;

public class ReactorPlanetScript : AbstractPlanet {

    public ParticleSystem particle;

	private float upgradeTimer;
	[Range(5f, 60f)]
	public float upgrade = 25.0f;

    void Start()
    {
        base.OnActivate();
        type = PlanetType.Reactor;
		upgradeTimer = 0;
    }

	protected override void PlanetTickUpdates (){
		if (gameManager.GetUpgrading ())
			particle.Play();
		else 
			particle.Stop ();

	}
	protected override void PlanetFrameUpdates ()
	{
		if (gameManager.GetUpgrading ()) {
			if (planetOwnership != gameManager.GetUpgrader ()) {
				gameManager.StopUpgrading ();
				upgradeTimer = 0;
			} else {
				upgradeTimer += Time.deltaTime;
				if (upgradeTimer >= upgrade) {
					switch (planetOwnership) {
					case Ownership.Enemy:
						gameManager.LevelEnemy ();
						gameManager.StopUpgrading ();
						break;
					case Ownership.Player:
						gameManager.LevelPlayer ();
						gameManager.StopUpgrading ();
						break;
					case Ownership.Neutral:
						break;
					}
					gameManager.audioManager.PlaySound ("upgradeComplete");
					upgradeTimer = 0;
					Debug.Log ("ReactorPlanetScript: Finished upgrading");
					Debug.Log ("ReactorPlanetScript: Player Level: "+ gameManager.GetPlayerLevel());
				}
			}
		} else
			upgradeTimer = 0;
	}
}
