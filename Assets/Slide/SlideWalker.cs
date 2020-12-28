using System.Collections.Generic;
using UnityEngine;

public class SlideWalker
{
    private Vector3 position = new Vector3();
    private Vector3 velocity = new Vector3();
    private Vector3 acceleration = new Vector3();
    private Vector3 wind = new Vector3();

    private List<Vector3> points = new List<Vector3>();

    private int updateTime = 0;
    private float maxSpan = 0;
    private float length = 0;
    private Vector3 lastPos = new Vector3();

    private float maxTurn;
    private float maxUp;
    private float minUp;

    private bool finish = false;

    public SlideWalker(Vector3 position, float maxSpan, float maxTurn, float maxUp, float minUp)
    {
        this.position = position;
        this.maxSpan = maxSpan;
        this.maxTurn = maxTurn;
        this.maxUp = maxUp;
        this.minUp = minUp;
    }

    public bool HasFinish() => finish;
    public Vector3[] GetPoints => points.ToArray();

    public void Update()
    {
        if (length >= maxSpan)
        {
            finish = true;
        }

        if (updateTime % 2 == 0)
        {
            wind.Set(Random.Range(-maxTurn, maxTurn), Random.Range(-minUp, maxUp), Random.Range(-maxTurn, maxTurn));
        }

        acceleration += wind;
        velocity += acceleration;
        position += velocity;

        points.Add(position);

        velocity *= 0.9f;
        acceleration = Vector3.zero;

        length += Vector3.Distance(position, lastPos);
        lastPos = position;

        updateTime++;
    }
}
