using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;
public class SoldierPlanetContextualMenuScript : AbstractPanel {
    public Toggle soldierToggle;
    public Toggle engineerToggle;

    private PlanetScript planetScript;
    protected override void OnActivate()
    {
        planetScript = targetGameObject.GetComponent<PlanetScript>();
        CheckForUpdate();
    }

    protected override void CheckForUpdate()
    {
        soldierToggle.isOn = planetScript.GetIsTrainingSoldiers(); // note that, this will trigger createSoldier
        engineerToggle.isOn = planetScript.GetIsTransingEngineers(); // this too
    }

    public void CreateSoldiers(bool value)
    {
        Debug.Log("create soldier " + value);
        planetScript.TrainSoldiers(value);
    }

    public void CreateEngineers(bool value)
    {
        planetScript.TrainEngineers(value);
    }

}
