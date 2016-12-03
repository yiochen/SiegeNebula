using UnityEngine;
using System.Collections;

public class SpotlightScript : MonoBehaviour {

    public Transform focusedObject = null;

    public float height = 7.38f; //y

    void Start()
    {
        transform.position = new Vector3(0, height, 0);
        transform.rotation = Quaternion.Euler(new Vector3(90.0f, 0.0f, 0.0f));
        gameObject.SetActive(false); //initially turned off
        if (focusedObject != null) {
            TurnOnFor(focusedObject);
        }
    }
    public void TurnOnFor(Transform target)
    {
        gameObject.SetActive(true);
        focusedObject = target;
        SetPositionTo(target);
    }
    
    public void TurnOff()
    {
        focusedObject = null;
        gameObject.SetActive(false);
    }

    private void SetPositionTo(Transform target)
    {
        Vector3 pos = target.position;
        pos.y = height;
        transform.position = pos;
    }
}
