using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public class ReactorContextualMenuScript : AbstractPanel {

    public Toggle reactorToggle;
    public Slider reactorProgress;

	private AbstractPlanet.Ownership ownership;
	private ManagerScript gameManager;
	private bool isPlayerUpgrading = false;
    private ReactorPlanetScript reactor;

	protected override void OnActivate ()
	{
		AbstractPlanet planet = targetGameObject.GetComponent<AbstractPlanet> ();
        reactor = planet.GetComponent<ReactorPlanetScript>();
		ownership = planet.planetOwnership;
		gameManager = ManagerScript.Instance;
		CheckForUpdate ();
	}

	protected override void CheckForUpdate ()
	{
        if (ownership == AbstractPlanet.Ownership.Player)
        {
            isPlayerUpgrading = reactorToggle.isOn = gameManager.GetUpgrading();
            reactorToggle.interactable = true;
            if (isPlayerUpgrading)
            {
                reactorProgress.value = reactor.GetUpgradePercentage();
            } else
            {
                reactorProgress.value = 0;
            } 


        } else
        {
            reactorToggle.interactable = false;
            reactorToggle.isOn = false;
            reactorProgress.value = 0;
        }
		
		//if (ownership == AbstractPlanet.Ownership.Player && gameManager.GetPlayerLevel () < 2)
		//{
		//	reactorToggle.interactable = true;
		//	reactorToggle.targetGraphic.transform.GetChild(0).gameObject.SetActive(true);
		//} else
		//{
		//	reactorToggle.interactable = false;
		//	reactorToggle.targetGraphic.transform.GetChild(0).gameObject.SetActive(false);
		//}
	}

	public void TogglePlayerUpgrade() {
		isPlayerUpgrading = !isPlayerUpgrading;
		if (ownership == AbstractPlanet.Ownership.Player)
        {
            ManagerScript.Instance.audioManager.PlaySound("ButtonClick");
            if (!gameManager.ActivateUpgrade(isPlayerUpgrading, ownership))
            {
                Debug.Log("Not enough Resources to Upgrade");
                isPlayerUpgrading = !isPlayerUpgrading;
            } else
            {
                Debug.Log("start upgrading");
            }
        }
	}

}
