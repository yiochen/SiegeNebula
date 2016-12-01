using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public class PlanetGUIScript : MonoBehaviour {
    PlanetScript planet;
    public ShipIconScript shipIcon;
    public Image OwnershipImage;
	// Use this for initialization
	void Start () {
        planet = transform.parent.gameObject.GetComponent<PlanetScript>();
	}

	// Update is called once per frame
	void Update () {
        switch (planet.planetOwnership)
        {
            case PlanetScript.Ownership.Enemy:
                OwnershipImage.color = Color.blue;
                break;
            case PlanetScript.Ownership.Player:
                OwnershipImage.color = Color.yellow;
                break;
            default:
                OwnershipImage.color = Color.gray;
                break;
        }
        if (planet.ships[Indices.SHIP_PLAYER] != null)
        {
            shipIcon.gameObject.SetActive(true);
        }
        else
        {
            shipIcon.gameObject.SetActive(false);
        }
    }
}
