using UnityEngine;
using System.Collections;

public class EventManagerScript : Singleton<EventManagerScript> {

	// Update is called once per frame
	void Update () {
        HandleMouseDown();
	}

    void HandleMouseDown()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hitInfo = new RaycastHit();
            // TODO: use layerMask when we decide what layer to receive raycast
            // var layerMask = 1 << Layers.DEFAULT
            bool hit = Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo);
            if (hit)
            {
                GameObject gameObject = hitInfo.transform.gameObject;
                EventHandler[] handlers = gameObject.GetComponents<EventHandler>();
                if (handlers.Length > 0)
                {
                    foreach (EventHandler handler in handlers)
                    {
                        handler.OnClick(hitInfo);
                    }
                }
            }
        }
    }
}
