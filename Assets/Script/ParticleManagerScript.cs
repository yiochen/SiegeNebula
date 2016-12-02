using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public enum ParticleList
{
    FIGHTING
}
[Serializable]
public struct ParticleWithType
{
    public ParticleList type;
    public GameObject particle;
    public ParticleWithType(ParticleList type, GameObject particle)
    {
        this.type = type;
        this.particle = particle;
    }
}
public class ParticleManagerScript : Singleton<ParticleManagerScript> {

    public ParticleWithType[] particleList;
    private Dictionary<ParticleList, GameObject> particleDictionary = new Dictionary<ParticleList, GameObject>();
    private Dictionary<int, ParticleWithType> playingParticles = new Dictionary<int, ParticleWithType>();

    void Awake()
    {
        foreach (ParticleWithType item in particleList)
        {
            particleDictionary.Add(item.type, item.particle);
        }
    }

    // Play the type of particle on the container
    // if called with the same type that is being played. do nothing.
    // if currently played particle is different type, stop and play the new type.
    public GameObject Play(ParticleList type, Transform container)
    {
        GameObject particlePrefab = null;
        GameObject particle = null;
        int instanceId = container.GetInstanceID();

        if (playingParticles.ContainsKey(container.GetInstanceID()))
        {
            ParticleWithType playingParticle = playingParticles[instanceId];
            if (playingParticle.type == type)
            {
                return playingParticle.particle;
            } else
            {
                Stop(container);
            }
        }
        particlePrefab = particleDictionary[type];
        particle = Instantiate(particlePrefab, container, false) as GameObject;
        playingParticles[instanceId] = new ParticleWithType(type, particle);
        return particle;
    }

    public void Stop(Transform container)
    {
        int instanceId = container.GetInstanceID();
        
        if (playingParticles.ContainsKey(instanceId))
        {
            Destroy(playingParticles[instanceId].particle);
        }

        playingParticles.Remove(instanceId);
    }
}
