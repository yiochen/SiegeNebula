using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Planet", menuName = "Planet Type")]
public class PlanetScriptable : ScriptableObject
{
    public string planetType;
    public GameObject planetMesh;
    public float meshScale = 1;
    public Vector3 meshRotation;

    public Quaternion getRotation()
    {
        return Quaternion.Euler(meshRotation.x, meshRotation.y, meshRotation.z);
    }

    public Vector3 getPosition()
    {
        return Vector3.zero;
    }

    public Vector3 getScale()
    {
        return new Vector3(meshScale, meshScale, meshScale);
    }
}
