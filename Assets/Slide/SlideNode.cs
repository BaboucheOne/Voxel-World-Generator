using UnityEngine;

[System.Serializable]
public class SlideNode
{
    public Vector3 Position;
    public Vector3 Direction;
    public Vector3 LastPosition;
    public Quaternion Orientation;
    public GameObject MeshObject;

    public BezierCurve Curve;

    public SlideNode(Vector3 position)
    {
        Position = position;
    }

    public SlideNode(Vector3 position, Vector3 prevPos)
    {
        Position = position;
        LastPosition = prevPos;
        CalculateOrientation();
    }

    private void CalculateOrientation()
    {
        Direction = Vector3.Normalize(Position - LastPosition);
        Orientation = Quaternion.LookRotation(Direction);
    }

    public void SetPrevious(Vector3 v)
    {
        LastPosition = v;
        CalculateOrientation();
    }
}
