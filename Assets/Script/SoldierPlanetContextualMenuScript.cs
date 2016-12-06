using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;
public class SoldierPlanetContextualMenuScript : AbstractPanel {
    public Toggle soldierToggle;

    private AbstractPlanet planetScript;
    protected override void OnActivate()
    {
        planetScript = targetGameObject.GetComponent<AbstractPlanet>();
        CheckForUpdate();
    }

    protected override void CheckForUpdate()
    {
        soldierToggle.isOn = planetScript.GetIsTrainingSoldiers(); // note that, this will trigger createSoldier
        if (planetScript.planetOwnership == AbstractPlanet.Ownership.Player)
        {
            soldierToggle.interactable = true;
            soldierToggle.targetGraphic.transform.GetChild(0).gameObject.SetActive(true);
        }else
        {
            soldierToggle.interactable = false;
            soldierToggle.targetGraphic.transform.GetChild(0).gameObject.SetActive(false);
        }
    }

    public void CreateSoldiers(bool value)
    {
        Debug.Log("create soldier " + value);
        planetScript.TrainSoldiers(value);
    }

}
