using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[ExecuteInEditMode]
// Represents a node in the waypoint graph
public class Node : MonoBehaviour
{
    public Edge[] edges; // Each edge going from this node
    public WaypointGraph waypointGraph;

    public bool pregenerate; // Does this node pre-generate its connections

    // Generates the edges to and from this node from other nodes in the graph
    public void GenerateEdges()
    {
        waypointGraph = transform.parent.GetComponent<WaypointGraph>();

        if (pregenerate)
        {
            edges = new Edge[waypointGraph.nodes.Length];
            List<Edge> edgelist = new List<Edge>();
            for (int i = 0; i < waypointGraph.nodes.Length; i++)
            {
                Vector3 displacement = waypointGraph.nodes[i].transform.position - transform.position;
                float magnitude = displacement.magnitude;
                Vector3 dir = displacement / magnitude;
                // Testing to make sure that this edge does not collide with any world objects
                if (!Physics.Raycast(transform.position, dir, magnitude))
                    edgelist.Add(new Edge(waypointGraph.nodes[i].GetComponent<Node>(), Vector3.Distance(transform.position, waypointGraph.nodes[i].transform.position)));
            }
            
            edges = MergeSort(edgelist.ToArray());

            Edge[] temp = new Edge[edges.Length - 1];
            for (int i = 1; i < edges.Length; i++)
                temp[i - 1] = edges[i];
            edges = temp;
        }
        else
        {
            for (int i = 0; i < edges.Length; i++)
                edges[i].ConnectedDistance = Vector3.Distance(transform.position, edges[i].connectedNode.transform.position);

            edges = MergeSort(edges);
        }
    }

    private void Update()
    {
        for (int i = 0; i < edges.Length; i++)
            Debug.DrawLine(transform.position, edges[i].connectedNode.transform.position, Color.blue);
    }

    // Merge Sort implementation for sorting each of this nodes edges by distance
    private Edge[] MergeSort(Edge[] array)
    {
        if (array.Length <= 1)
            return array;

        Edge[] left = new Edge[array.Length / 2];
        Edge[] right = new Edge[Mathf.CeilToInt(array.Length / 2f)];

        for (int i = 0; i < array.Length; i++)
        {
            if (i < array.Length / 2)
                left[i] = array[i];
            else
                right[i - array.Length / 2] = array[i];
        }

        left = MergeSort(left);
        right = MergeSort(right);

        return Merge(left, right);
    }
    private Edge[] Merge(Edge[] left, Edge[] right)
    {
        List<Edge> result = new List<Edge>();

        Queue<Edge> leftQueue = new Queue<Edge>(left);
        Queue<Edge> rightQueue = new Queue<Edge>(right);

        while (leftQueue.Count > 0 && rightQueue.Count > 0)
            if (leftQueue.First().ConnectedDistance <= rightQueue.First().ConnectedDistance)
                result.Add(leftQueue.Dequeue());
            else
                result.Add(rightQueue.Dequeue());

        while (leftQueue.Count > 0)
            result.Add(leftQueue.Dequeue());
        while (rightQueue.Count > 0)
            result.Add(rightQueue.Dequeue());

        return result.ToArray();
    }
}

[System.Serializable]
public class Edge
{
    public Node connectedNode;
    public float connectedDistance;

    public float ConnectedDistance { get { return connectedDistance; } set { connectedDistance = value; } }

    public Edge(Node connectedNode, float connectedDistance)
    {
        this.connectedNode = connectedNode;
        this.connectedDistance = connectedDistance;
    }
}