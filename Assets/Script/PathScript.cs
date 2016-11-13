using UnityEngine;
using System.Collections;

public struct DirectionalPath
{
    public Transform start;
    public Transform end;
    public PathScript pathScript;

    public DirectionalPath(Transform start, Transform end, PathScript pathScript)
    {
        this.start = start;
        this.end = end;
        this.pathScript = pathScript;
    }
}
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

    public DirectionalPath getDirectionalPath(bool isReversed)
    {
        if (isReversed)
        {
            return new DirectionalPath(end, start, this);
        }
        else
        {
            return new DirectionalPath(start, end, this);
        }
    }

}
