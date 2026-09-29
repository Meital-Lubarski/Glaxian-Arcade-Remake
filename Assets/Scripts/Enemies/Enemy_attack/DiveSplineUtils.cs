using UnityEngine;
using UnityEngine.Splines;

public static class DiveSplineUtils
{
    private const float ZeroZ = 0f;
    private const int DefultLengthSamples = 80;
    private const float MinSplineLength = 0.0001f;
    private const float StartTime = 0f;
    
    
    public static Vector3 EvaluateOffsetLocal(SplineContainer template, float t)
    {
        Vector3 worldP = template.EvaluatePosition(t);
        Vector3 localP = template.transform.InverseTransformPoint(worldP);
        return new Vector3(localP.x, localP.y, ZeroZ);
    }

    public static Vector3 EvaluateTangentLocal(SplineContainer template, float t)
    {
        Vector3 worldT = template.EvaluateTangent(t);
        Vector3 localT = template.transform.InverseTransformDirection(worldT);
        return new Vector3(localT.x, localT.y, ZeroZ);
        
    }
    
    public static float ApproxLength(SplineContainer container, int samples = 80)
    {
        float length = 0f;
        Vector3 prev = container.EvaluatePosition(StartTime);

        for (int i = 1; i <= samples; i++)
        {
            float tt = i / (float)samples;
            Vector3 p = container.EvaluatePosition(tt);
            length += Vector3.Distance(prev, p);
            prev = p;
        }

        return Mathf.Max(MinSplineLength, length);
    }
}
