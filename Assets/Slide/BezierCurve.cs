using UnityEngine;

public class BezierCurve
{
    public enum PointType { Initial, Ctrl1, Ctrl2, Final }

    private Vector3 initialPoint;
    private Vector3 controlPoint1;
    private Vector3 controlPoint2;
    private Vector3 finalPoint;
    private Vector3[] points;
    private int maxPoints = 15;

    public BezierCurve(Vector3 initialPoint, Vector3 controlPoint1, Vector3 controlPoint2, Vector3 finalPoint)
    {
        this.initialPoint = initialPoint;
        this.controlPoint1 = controlPoint1;
        this.controlPoint2 = controlPoint2;
        this.finalPoint = finalPoint;

        CalculateCurve();
    }

    public Vector3 GetInitial() => initialPoint;
    public Vector3 GetControl1() => controlPoint1;
    public Vector3 GetControl2() => controlPoint2;
    public Vector3 GetFinal() => finalPoint;
    public Vector3[] GetPoints() => points;
    public int PointsCount() => maxPoints;

    public void SetPoint(Vector3 pos, PointType type)
    {
        switch (type)
        {
            case PointType.Initial:
                initialPoint = pos;
                break;

            case PointType.Ctrl1:
                controlPoint1 = pos;
                break;

            case PointType.Ctrl2:
                controlPoint2 = pos;
                break;

            case PointType.Final:
                finalPoint = pos;
                break;
        }

        CalculateCurve();
    }

    public void SetMaxPoints(int x)
    {
        if (x < 0) return;

        maxPoints = x;

        CalculateCurve();
    }

    public void CalculateCurve()
    {
        points = new Vector3[maxPoints];

        float t = 0f;
        int tpoints = maxPoints - 1;
        for (int i = 0; i < maxPoints; i++)
        {
            t = (float)i / tpoints;
            float x = (initialPoint.x * Mathf.Pow(1 - t, 3)) + (3 * controlPoint1.x * t * Mathf.Pow(1 - t, 2)) + (3 * controlPoint2.x * Mathf.Pow(t, 2) * (1 - t)) + (finalPoint.x * Mathf.Pow(t, 3));
            float y = (initialPoint.y * Mathf.Pow(1 - t, 3)) + (3 * controlPoint1.y * t * Mathf.Pow(1 - t, 2)) + (3 * controlPoint2.y * Mathf.Pow(t, 2) * (1 - t)) + (finalPoint.y * Mathf.Pow(t, 3));
            float z = (initialPoint.z * Mathf.Pow(1 - t, 3)) + (3 * controlPoint1.z * t * Mathf.Pow(1 - t, 2)) + (3 * controlPoint2.z * Mathf.Pow(t, 2) * (1 - t)) + (finalPoint.z * Mathf.Pow(t, 3));

            points[i] = new Vector3(x, y, z);
        }
    }
}
