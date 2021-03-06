﻿using UnityEngine;
using System.Collections;

public class ResourcePlanetScript : AbstractPlanet
{
    public ParticleSystem particle;
    // Use this for initialization
    void Start()
    {
        base.OnActivate();
        type = PlanetType.Resource;
    }

    override protected void PlanetTickUpdates()
    {
        MineResources();
        if (isMiningResources())
        {
            particle.Play();
        }
        else
        {
            particle.Stop();
        }
    }

    protected override void PlanetFrameUpdates() { }
}
