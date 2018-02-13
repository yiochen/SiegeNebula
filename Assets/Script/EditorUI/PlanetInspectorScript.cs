using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetInspectorScript : MonoBehaviour
{
    public Color color = Color.blue;
    private void OnDrawGizmos()
    {
        Gizmos.color = color;
        Gizmos.DrawSphere(transform.position, 1.0f);
    }
}
