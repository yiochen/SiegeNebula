using UnityEngine;
using System.Collections;
using System;

public class ReactorPlanetScript : AbstractPlanet {

    public ParticleSystem particle;
    void Start()
    {
        base.OnActivate();
        type = PlanetType.Reactor;
    }
    protected override void PlanetUpdates()
    {
        particle.Play();
    }
}
