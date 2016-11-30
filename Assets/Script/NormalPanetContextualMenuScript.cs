using UnityEngine;
using System.Collections;
using System;

public class NormalPanetContextualMenuScript : AbstractPanel {

    PlanetScript planetScript;
    public StatLabelScript figherLabel;
    public StatLabelScript engineerLabel;

    protected override void OnActivate()
    {
        planetScript = targetGameObject.GetComponent<PlanetScript>();
        CheckForUpdate();
    }
    protected override void CheckForUpdate()
    {
        if (planetScript)
        {
            figherLabel.SetValue("" + planetScript.playerSoldiers.soldierCount);
            engineerLabel.SetValue("" + planetScript.playerEngineerCount);
        }

    }
}
