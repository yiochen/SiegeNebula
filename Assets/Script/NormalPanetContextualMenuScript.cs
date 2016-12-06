using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;

public class NormalPanetContextualMenuScript : AbstractPanel {

    AbstractPlanet planetScript;
    public StatLabelScript soldierLabel;
    public StatLabelScript engineerLabel;
    public Button shipButton;
    public Slider shipButtonSlider;

    protected override void OnActivate()
    {
        planetScript = targetGameObject.GetComponent<AbstractPlanet>();
        CheckForUpdate();
    }
    protected override void CheckForUpdate()
    {
        if (planetScript)
        {
            soldierLabel.SetValue("" + planetScript.playerSoldiers);
            if (planetScript.ships[Indices.SHIP_PLAYER]  || planetScript.planetOwnership.Equals(AbstractPlanet.Ownership.Player))
            {
                shipButton.interactable = true;
            } else
            {
                shipButton.interactable = false;
            }

            if (planetScript.ships[Indices.SHIP_PLAYER])
            {
                // if there is already a ship
                shipButtonSlider.value = planetScript.ships[Indices.SHIP_PLAYER].filledPercentage;
                
                if (planetScript.ships[Indices.SHIP_PLAYER].GetIsLoading())
                {
                    shipButton.GetComponent<Image>().color = Color.yellow;
                }else
                {
                    shipButton.GetComponent<Image>().color = Color.white;
                }
            }
            else
            {
                shipButtonSlider.value = 0;
                shipButton.GetComponent<Image>().color = Color.white;
            }
        }

    }

    //helper function
    public void PlayerCreateShip()
    {
        Debug.Log("create ship for player");
        ShipScript ship = planetScript.CreateShip(AbstractPlanet.Ownership.Player);
        if (ship != null) {
			ManagerScript.Instance.audioManager.PlaySound ("ButtonClick");
            planetScript.LoadSoldiersToShip(ship);
        }
    }
}
