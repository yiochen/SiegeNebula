using UnityEngine;
using System.Collections;

public class PathInspectorScript : MonoBehaviour {
    private PathScript path;

	void OnDrawGizmos()
    {
        Debug.Log("drawing");
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
}
