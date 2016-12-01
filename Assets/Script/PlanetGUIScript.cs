using UnityEngine;
using System.Collections;

public class PlanetGUIScript : MonoBehaviour {
    PlanetScript planet;
    public ShipIconScript shipIcon;
	// Use this for initialization
	void Start () {
        planet = transform.parent.gameObject.GetComponent<PlanetScript>();
	}

	// Update is called once per frame
	void Update () {
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
