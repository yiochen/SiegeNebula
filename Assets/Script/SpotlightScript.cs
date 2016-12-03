using UnityEngine;
using System.Collections;

public class SpotlightScript : MonoBehaviour {

    public bool isFollowing = false;
    public Transform focusedObject = null;

    public float height = 7.38f; //y

    void Start()
    {
        transform.position = new Vector3(0, height, 0);
        transform.rotation = Quaternion.Euler(new Vector3(90.0f, 0.0f, 0.0f));
        gameObject.SetActive(false); //initially turned off
        if (focusedObject != null) {
            TurnOnFor(focusedObject, isFollowing);
        }
    }
    public void TurnOnFor(Transform target)
    {
        TurnOnFor(target, false);
    }

    public void TurnOnFor(Transform target, bool isFollowing)
    {
        gameObject.SetActive(true);
        focusedObject = target;
        this.isFollowing = isFollowing;
        SetPositionTo(target);
    }
    
    public void TurnOff()
    {
        focusedObject = null;
        gameObject.SetActive(false);
    }
	// Update is called once per frame
	void Update () {
	    if (focusedObject != null && isFollowing)
        {
            SetPositionTo(focusedObject);
        }
	}

    private void SetPositionTo(Transform target)
    {
        Vector3 pos = target.position;
        pos.y = height;
        transform.position = pos;
    }
}
