using UnityEngine;
using System.Collections;

/*
  This class handles keyboard event. Mouse event handling are provided by Unity through OnMouseDown
*/
public class EventManagerScript : Singleton<EventManagerScript> {

	// Update is called once per frame
	void Update () {
	}

    public static Vector3 GetMouseInWorldPosition()
    {
        Vector3 pos=Camera.main.ScreenToWorldPoint(Input.mousePosition);
        pos.y = 0.0f;
        return pos;
    }
}
