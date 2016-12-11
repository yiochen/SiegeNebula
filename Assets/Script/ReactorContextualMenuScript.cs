using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public class ReactorContextualMenuScript : AbstractPanel {

    public Toggle reactorToggle;

	private AbstractPlanet.Ownership ownership;
	private ManagerScript gameManager;
	private bool isUpgrading = false;

	protected override void OnActivate ()
	{
		AbstractPlanet planet = targetGameObject.GetComponent<AbstractPlanet> ();
		ownership = planet.planetOwnership;
		gameManager = ManagerScript.Instance;
		CheckForUpdate ();
	}

	protected override void CheckForUpdate ()
	{
		reactorToggle.isOn = gameManager.GetUpgrading ();
		if (ownership == AbstractPlanet.Ownership.Player && gameManager.GetPlayerLevel () < 3)
		{
			reactorToggle.interactable = true;
			reactorToggle.targetGraphic.transform.GetChild(0).gameObject.SetActive(true);
		} else
		{
			reactorToggle.interactable = false;
			reactorToggle.targetGraphic.transform.GetChild(0).gameObject.SetActive(false);
		}
	}

	public void UpgradeToggle() {
		isUpgrading = !isUpgrading;
		Debug.Log("upgrading " + isUpgrading);
		ManagerScript.Instance.audioManager.PlaySound ("ButtonClick");
		gameManager.ActivateUpgrade (isUpgrading, ownership);
	}

}
