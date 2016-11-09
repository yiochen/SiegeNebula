using UnityEngine;
using System.Collections;

public class EventManagerScript : Singleton<EventManagerScript> {

	// Update is called once per frame
	void Update () {
        handleMouseDown();
	}

    void handleMouseDown()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hitInfo = new RaycastHit();
            // TODO: use layerMask when we decide what layer to receive raycast
            // var layerMask = 1 << Layers.DEFAULT
            bool hit = Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo);
            if (hit)
            {
                Debug.Log("hit " + hitInfo.transform.gameObject.name);
            }
        }
    }
}
