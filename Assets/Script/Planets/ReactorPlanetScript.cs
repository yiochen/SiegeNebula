using UnityEngine;
using System.Collections;
using System;

public class ReactorPlanetScript : AbstractPlanet {

    void Start()
    {
        base.OnActivate();
        type = PlanetType.Reactor;
    }
    protected override void PlanetUpdates()
    {
        
    }
}
