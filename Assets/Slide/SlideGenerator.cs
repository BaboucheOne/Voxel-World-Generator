using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlideGenerator : MonoBehaviour
{
    [SerializeField]
    private GameObject pref;
    private int pointsOnCurve = 15;

    private Vector3[] slideFlow;
    private int slideSpan = 1000;
    private SlideWalker slideWalker;

    [SerializeField]
    private SlideNode[] slideNodes;
    private Vector3[] verticesB;

    private void Awake()
    {
        GenerateFlow();
        GenerateSlideNodes();

        BuildTransform();

        SlideNode s = new SlideNode(transform.position, transform.position - Vector3.back * 3);
        s.MeshObject = Instantiate(pref, Vector3.up * 16, Quaternion.identity, transform) as GameObject;
        BuildSection(s);
    }

    public SlideNode GetNode(int x) => slideNodes[x];

    private void GenerateFlow()
    {
        slideWalker = new SlideWalker(transform.position, slideSpan, 5.5f, 0.15f, 1f);

        while (!slideWalker.HasFinish())
        {
            slideWalker.Update();
        }

        slideFlow = slideWalker.GetPoints;
    }

    private void GenerateSlideNodes()
    {
        List<SlideNode> nodes = new List<SlideNode>();
        List<SlideNode> curvedNodes = new List<SlideNode>();
        for(int i = 0; i < slideFlow.Length - 2; i+=2)
        {
            SlideNode n = new SlideNode(slideFlow[i]);
            Vector3 lastPos = new Vector3();
            if(i == 0)
            {
                lastPos = transform.position * 1.25f;
            } else
            {
                lastPos = slideFlow[i - 1];
            }

            n.Curve = new BezierCurve(lastPos,
                                        lastPos,
                                        slideFlow[i],
                                        slideFlow[i + 1]);

            nodes.Add(n);
        }

        //Get all points on curve without setting up the previous one.
        Vector3[] points;
        for(int i = 0; i < nodes.Count; i++)
        {
            points = nodes[i].Curve.GetPoints();
            for (int j = 0; j < points.Length; j++)
            {
                curvedNodes.Add(new SlideNode(points[j]));
            }

            curvedNodes.RemoveAt(curvedNodes.Count - 1);
        }

        //Setup the previous one.
        curvedNodes[0].SetPrevious(transform.position);
        for (int i = 1; i < curvedNodes.Count; i++)
        {
            curvedNodes[i].SetPrevious(curvedNodes[i - 1].Position);
        }

        slideNodes = curvedNodes.ToArray();
    }

    private void BuildTransform()
    {
        foreach (SlideNode n in slideNodes)
        {
            n.MeshObject = Instantiate(pref, n.Position, n.Orientation) as GameObject;
        }
    }

    private void BuildSection(SlideNode n)
    {
        Vector3[] verticesShape = new Vector3[]
        {
            new Vector3(1, 0, 0),
            new Vector3(2, 1, 0),
            new Vector3(2, 1, 0),

            new Vector3(1, 0, 0),
            new Vector3(2, 1, 0),
            new Vector3(1, 0, 0),

            new Vector3(-1, 0, 0),
            new Vector3(1, 0, 0),
            new Vector3(1, 0, 0),

            new Vector3(-1, 0, 0),
            new Vector3(1, 0, 0),
            new Vector3(-1, 0, 0),

            new Vector3(-2, 1, 0),
            new Vector3(-1, 0, 0),
            new Vector3(-1, 0, 0),

            new Vector3(-2, 1, 0),
            new Vector3(-1, 0, 0),
            new Vector3(-2, 1, 0),
        };

        int[] trianglesShape = new int[]
        {
            0, 1, 2,
            3, 4, 5,
            6, 7, 8,
            9, 10, 11,
            12, 13, 14,
            15, 16, 17
        };

        //print(n.LastPosition);
        //print(n.Position);

        verticesShape[0] += n.LastPosition;
        verticesShape[1] += n.LastPosition;
        verticesShape[2] += n.Position + n.MeshObject.transform.right + n.MeshObject.transform.up + n.MeshObject.transform.forward;

        verticesShape[3] += n.LastPosition;
        verticesShape[4] += n.Position;
        verticesShape[5] += n.Position;

        verticesShape[6] += n.LastPosition;
        verticesShape[7] += n.LastPosition;
        verticesShape[8] += n.Position;

        verticesShape[9] += n.LastPosition;
        verticesShape[10] += n.Position;
        verticesShape[11] += n.Position;

        verticesShape[12] += n.LastPosition;
        verticesShape[13] += n.LastPosition;
        verticesShape[14] += n.Position;

        verticesShape[15] += n.LastPosition;
        verticesShape[16] += n.Position;
        verticesShape[17] += n.Position;

        //for(int i = 0; i < verticesShape.Length; i++)
        //{
        //    print(verticesShape[i].ToString());
        //}

        verticesB = verticesShape;

        Mesh mesh = new Mesh()
        {
            vertices = verticesShape,
            triangles = trianglesShape,
            name = "Slide section"
        };

        mesh.Optimize();
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        n.MeshObject.GetComponent<MeshFilter>().mesh = mesh;
        n.MeshObject.GetComponent<MeshCollider>().sharedMesh = mesh;
    }

    private void OnDrawGizmos()
    {
        if (slideFlow == null) return;

        Gizmos.color = Color.red;
        for (int i = 1; i < slideNodes.Length; i++)
        {
            Gizmos.DrawLine(slideNodes[i].LastPosition, slideNodes[i].Position); 
        }

        if (verticesB == null) return;

        for (int i = 0; i < verticesB.Length; i++)
        {
            Gizmos.DrawSphere(verticesB[i], 0.15f);
        }
    }
}
