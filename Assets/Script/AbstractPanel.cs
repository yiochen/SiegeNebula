using UnityEngine;
using System.Collections;

public abstract class AbstractPanel : MonoBehaviour {

    protected GameObject targetGameObject;
    protected bool isActive;
    public void ActivateForGameObject(GameObject target)
    {
        this.targetGameObject = target;
        isActive = true;
        gameObject.SetActive(true);
        OnActivate();
    }
    public void Deactivate()
    {
        isActive = false;
        OnDeactive();
    }

    // overrideable functions
    protected virtual void OnActivate() { }
    protected virtual void OnDeactive() { }
    // CheckForUpdate is called once per frame if the panel is active.
    protected abstract void CheckForUpdate();



	void Update () {
        if (isActive)
        {
            CheckForUpdate();
        }

	}
}
