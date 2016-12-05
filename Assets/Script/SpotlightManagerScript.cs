using UnityEngine;
using System.Collections;

public class SpotlightManagerScript : Singleton<SpotlightManagerScript> {

    public SpotlightScript spotlight;


	// Update is called once per frame
	void Update () {
        AbstractPlanet selectedPlanet = ManagerScript.Instance.GetSelectedPlanet();

	    if (selectedPlanet)
        {
            spotlight.TurnOnFor(selectedPlanet.transform);
        } else
        {
            spotlight.TurnOff();
        }
	}
}
