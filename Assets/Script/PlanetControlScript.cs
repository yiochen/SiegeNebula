using UnityEngine;
using System.Collections;

public class PlanetControlScript : MonoBehaviour {

	// Use this for initialization
	void Start () {

	}

	// Update is called once per frame
	void Update () {

	}

    void OnMouseDown() //unity built-in function
    {
        Debug.Log(gameObject.name + " was clicked");
    }
}
