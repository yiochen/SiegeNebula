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
    public float pathChosingThreshold = 0.5f;

    private Vector3 direction;
    LineRenderer lineRenderer;

	void Start () {
        lineRenderer = GetComponent<LineRenderer>();
        if (lineRenderer && start && end)
        {
            lineRenderer.SetPosition(0, start.position);
            lineRenderer.SetPosition(1, end.position);
        }
        direction = (end.position - start.position).normalized;
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

    public DirectionalPath getDirectionStartingFrom(Transform start) {
        if (start.position == this.start.position) return getDirectionalPath(false);
        if (start.position == this.end.position) return getDirectionalPath(true);
        if (Vector3.Distance(start.position, this.start.position)<Vector3.Distance(start.position, this.end.position))
        {
            return getDirectionalPath(false);
        } else
        {
            return getDirectionalPath(true);
        }
    }

    public bool IsQualifiedForLaunching(Vector3 launchingPosition)
    {
        bool isCloseToPath = MathHelper.DistanceToLine(launchingPosition, start.position, end.position) < pathChosingThreshold;
        Vector3 startToPos = launchingPosition - start.position;
        Vector3 posToEnd = end.position - launchingPosition;
        bool isBetweenStartAndEnd = Vector3.Dot(startToPos, direction) > 0 && Vector3.Dot(posToEnd, direction) > 0;

        return isCloseToPath && isBetweenStartAndEnd;
    }

}
