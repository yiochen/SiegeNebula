using UnityEngine;
using System.Collections;
using System;

public class ShipIconScript : MonoBehaviour {

    private bool isDragging = false;
	// Use this for initialization
	void Start () {

	}

	// Update is called once per frame
	void Update () {
        if (isDragging)
        {
            Debug.Log(Input.mousePosition);
        }
	}

    void OnMouseDown()
    {
        isDragging = true;
    }

    void OnMouseUp()
    {
        isDragging = false;
    }
}
