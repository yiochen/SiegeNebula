using UnityEngine;
using System.Collections;

public class ContextPanelBindingScript : MonoBehaviour {

    public AbstractPanel panel;

	void OnMouseDown()
    {
        panel.ActivateForGameObject(gameObject);
    }
}
