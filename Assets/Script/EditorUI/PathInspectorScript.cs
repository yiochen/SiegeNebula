using UnityEngine;
using System.Collections;

public class PathInspectorScript : MonoBehaviour {
    private PathScript path;

    void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        if (!path)
        {
            path = GetComponent<PathScript>();
        }
        if (path)
        {
            Gizmos.DrawLine(path.start.position, path.end.position);
        }
    }
	void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        if (!path)
        {
            path = GetComponent<PathScript>();
        }
        if (path)
        {
            Gizmos.DrawLine(path.start.position, path.end.position);
        }
    }
}
