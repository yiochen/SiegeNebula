using UnityEngine;
using System.Collections;

public class MathHelper {

    public static float DistanceToLine(Vector3 point, Vector3 pointAOnLine, Vector3 pointBOnLine)
    {
        Vector3 line = pointBOnLine - pointAOnLine;
        Vector3 lineDir = line.normalized;

        float dis = Vector3.Dot(point - pointAOnLine, lineDir);
        Vector3 projectionOnLine = pointAOnLine + lineDir * dis;

        return Vector3.Distance(point, projectionOnLine);
    }
}
