using UnityEngine;
using System.Collections;

public abstract class EventHandler : MonoBehaviour {

    public abstract void OnClick(RaycastHit hit);

    //public abstract void OnMouseUp(RaycastHit hit);
}
