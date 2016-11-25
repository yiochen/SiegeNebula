using UnityEngine;
using System.Collections;

public abstract class AbstractPanel : MonoBehaviour {

    private GameObject targetGameObject;

    public void ActivateForGameObject(GameObject target)
    {
        this.targetGameObject = target;
        gameObject.SetActive(true);
    }

    protected abstract void CheckForUpdate();
	// Update is called once per frame
	void Update () {
        CheckForUpdate();
	}
}
