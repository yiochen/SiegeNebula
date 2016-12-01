using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

[Serializable]
public struct ContextualMenuPair
{
    public PlanetScript.PlanetType planetType;
    public GameObject contextualMenu;
}
public class ContextualMenuManagerScript : Singleton<ContextualMenuManagerScript> {
    // use for inspector only
    public ContextualMenuPair[] contextualMenuMapping;

    private Dictionary<PlanetScript.PlanetType, GameObject> contextualMenuMap;

    private GameObject selectedPlanet;
    private GameObject activatedPanel;
    private AbstractPanel[] panelScripts;

	// Use this for initialization
	void Start () {
        Debug.Log("contextual menu manager created");
        contextualMenuMap = new Dictionary<PlanetScript.PlanetType, GameObject>();
        foreach (ContextualMenuPair pair in contextualMenuMapping)
        {
            contextualMenuMap.Add(pair.planetType, pair.contextualMenu);
        }
	}

	// Update is called once per frame
	void Update () {

	}

    public void Deactivate()
    {
        if (panelScripts != null)
        {
            foreach (AbstractPanel panelScript in panelScripts)
            {
                panelScript.Deactivate();
            }
        }
        if (activatedPanel)
        {
            activatedPanel.SetActive(false);
        }
        panelScripts = null;
        selectedPlanet = null;
        activatedPanel = null;
    }

    public void ActivateForPlanet(PlanetScript planet)
    {
        Deactivate();
        selectedPlanet = planet.gameObject;
        activatedPanel = contextualMenuMap[planet.type];
        activatedPanel.SetActive(true);
        panelScripts = activatedPanel.GetComponents<AbstractPanel>();
        foreach (AbstractPanel panelScript in panelScripts)
        {
            panelScript.ActivateForGameObject(selectedPlanet);
        }
    }
}
