using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;

public class NormalPanetContextualMenuScript : AbstractPanel {

    AbstractPlanet planetScript;
    public StatLabelScript soldierLabel;
	public Text planetLabel;
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
			planetLabel.text = GetPlanetTypeName (planetScript);
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

	string GetPlanetTypeName(AbstractPlanet planet) {
		switch (planet.GetPlanetType ()) {
		case AbstractPlanet.PlanetType.Hybrid:
			return PlanetNames.HYBRID_PLANET;
		case AbstractPlanet.PlanetType.Normal:
			return PlanetNames.NORMAL_PLANET;
		case AbstractPlanet.PlanetType.Reactor:
			return PlanetNames.REACTOR_PLANET;
		case AbstractPlanet.PlanetType.Resource:
			return PlanetNames.RESOURCE_PLANET;
		case AbstractPlanet.PlanetType.Soldier:
			return PlanetNames.SOLDIER_PLANET;
		}
		Debug.LogError ("NormalContextMenu: PlanetType was not set on planet!");
		return null;
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
