using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;

public class NormalPanetContextualMenuScript : AbstractPanel {

    PlanetScript planetScript;
    public StatLabelScript figherLabel;
    public StatLabelScript engineerLabel;
    public Button shipButton;
    public Slider shipButtonSlider;

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
            if (planetScript.ships[Indices.SHIP_PLAYER]  || planetScript.planetOwnership.Equals(PlanetScript.Ownership.Player))
            {
                shipButton.gameObject.SetActive(true);
            }

            if (planetScript.ships[Indices.SHIP_PLAYER])
            {
                // if there is already a ship
                shipButtonSlider.value = planetScript.ships[Indices.SHIP_PLAYER].filledPercentage;
                if (shipButtonSlider.value > 0)
                {
                    shipButton.GetComponent<Image>().color = Color.yellow;
                } else
                {
                    shipButton.GetComponent<Image>().color = Color.white;
                }
            }
            else
            {
                shipButtonSlider.value = 0;
            }
        }

    }

    //helper function
    public void PlayerCreateShip()
    {
        if (planetScript)
        {
            ShipScript ship = planetScript.CreateShip(PlanetScript.Ownership.Player);
            if (ship != null) {
                planetScript.LoadSoldiersToShip(ship);
            }
        }
    }
}
