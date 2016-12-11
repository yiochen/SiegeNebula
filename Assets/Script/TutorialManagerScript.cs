using UnityEngine;
using System.Collections;

public class TutorialManagerScript : MonoBehaviour {

	private ManagerScript gameManager;


	// Use this for initialization
	void Start () {
		gameManager = ManagerScript.Instance;
	}
	
	// Update is called once per frame
	void Update () {
		if (gameManager.GetPlayerSoliderCount () == 0) {
			gameManager.playerPlanets.Clear ();
		}
	}
}
