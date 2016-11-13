using UnityEngine;
using System.Collections;

public class PathScript : MonoBehaviour {

    public Transform start;
    public Transform end;
    LineRenderer lineRenderer;

	void Start () {
        lineRenderer = GetComponent<LineRenderer>();
        if (lineRenderer && start && end)
        {
            lineRenderer.SetPosition(0, start.position);
            lineRenderer.SetPosition(1, end.position);
        }
	}

}
