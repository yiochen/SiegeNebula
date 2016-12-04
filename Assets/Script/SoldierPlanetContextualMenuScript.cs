using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;
public class SoldierPlanetContextualMenuScript : AbstractPanel {
    public Toggle soldierToggle;

    private PlanetScript planetScript;
    protected override void OnActivate()
    {
        planetScript = targetGameObject.GetComponent<PlanetScript>();
        CheckForUpdate();
    }

    protected override void CheckForUpdate()
    {
        soldierToggle.isOn = planetScript.GetIsTrainingSoldiers(); // note that, this will trigger createSoldier
    }

    public void CreateSoldiers(bool value)
    {
        Debug.Log("create soldier " + value);
        planetScript.TrainSoldiers(value);
    }

}
