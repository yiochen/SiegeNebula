using UnityEngine;
using System.Collections;

public class PlanetControlScript : EventHandler {

	// Use this for initialization
	void Start () {

	}

	// Update is called once per frame
	void Update () {

	}

    public override void OnClick(RaycastHit hit)
    {
        Debug.Log(gameObject.name + " was clicked");
    }
}
