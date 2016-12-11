using UnityEngine;
using System.Collections;

public struct DirectionalPath
{
    public Transform start;
    public Transform end;
    public Vector3 shipStart;
    public Vector3 shipEnd;
    public PathScript pathScript;

    public DirectionalPath(PathScript pathScript, Transform start, Transform end,  Vector3 shipStart, Vector3 shipEnd)
    {
        this.start = start;
        this.end = end;
        this.pathScript = pathScript;
        this.shipStart = shipStart;
        this.shipEnd = shipEnd;
    }

    public Vector3 GetDirectionVector()
    {
        return this.end.position - this.start.position;
    }
}
public class PathScript : MonoBehaviour {

    public Transform start;
    public Transform end;
    public float pathChosingThreshold = 10.0f;
    public float paddingStart = 1.0f;
    public float paddingEnd = 1.0f;

    private Vector3 direction;
    private Vector3 shipStartPosition;
    private Vector3 shipEndPosition;

    LineRenderer lineRenderer;

    private GameObject hintInstance;

	void Start () {
        lineRenderer = GetComponent<LineRenderer>();
        if (lineRenderer && start && end)
        {
            lineRenderer.SetPosition(0, start.position);
            lineRenderer.SetPosition(1, end.position);
        }

        hintInstance = PathManagerScript.Instance.pathArrow;

        direction = (end.position - start.position).normalized;
        this.shipStartPosition = start.position + direction * paddingStart;
        this.shipEndPosition = end.position - direction * paddingEnd;
	}

    public DirectionalPath getDirectionalPath(bool isReversed)
    {
        if (isReversed)
        {
            return new DirectionalPath(this, end, start, shipEndPosition, shipStartPosition);
        }
        else
        {
            return new DirectionalPath(this, start, end, shipStartPosition, shipEndPosition);
        }
    }

    public DirectionalPath GetDirectionStartingFrom(Transform start) {
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
    public bool IsMouseCloseEnough(Vector3 mousePosition, Transform startPlanet)
    {
        Vector3 startToPos = mousePosition - startPlanet.position;
        Vector3 pathDirection = GetDirectionStartingFrom(startPlanet).GetDirectionVector();
        return Mathf.Abs(Vector3.Angle(startToPos, pathDirection)) < pathChosingThreshold;
    }
    // Display an arrow at the start of the path
    public void DisplayVisualHint(Transform start)
    {
        DirectionalPath path = GetDirectionStartingFrom(start);
        Vector3 direction = path.GetDirectionVector();
        Quaternion rotation= hintInstance.transform.rotation;
        rotation.SetLookRotation(direction);

        hintInstance.transform.rotation = rotation;
        hintInstance.transform.position = path.shipStart;
        hintInstance.SetActive(true);

    }

    public void StopVisualHint()
    {
        hintInstance.SetActive(false);
        
    }
}
