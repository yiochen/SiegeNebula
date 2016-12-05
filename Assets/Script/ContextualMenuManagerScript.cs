using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

[Serializable]
public struct ContextualMenuPair
{
    public AbstractPlanet.PlanetType planetType;
    public GameObject contextualMenu;
}
public class ContextualMenuManagerScript : Singleton<ContextualMenuManagerScript> {
    // use for inspector only
    public ContextualMenuPair[] contextualMenuMapping;

    private Dictionary<AbstractPlanet.PlanetType, GameObject> contextualMenuMap;

    private GameObject selectedPlanet;
    private GameObject activatedPanel;
    private AbstractPanel[] panelScripts;

	// Use this for initialization
	void Start () {
        Debug.Log("contextual menu manager created");
        contextualMenuMap = new Dictionary<AbstractPlanet.PlanetType, GameObject>();
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

    public void ActivateForPlanet(AbstractPlanet planet)
    {
        Deactivate();
        selectedPlanet = planet.gameObject;
		activatedPanel = contextualMenuMap[planet.GetPlanetType()];
        activatedPanel.SetActive(true);
        panelScripts = activatedPanel.GetComponents<AbstractPanel>();
        foreach (AbstractPanel panelScript in panelScripts)
        {
            panelScript.ActivateForGameObject(selectedPlanet);
        }
    }
}
