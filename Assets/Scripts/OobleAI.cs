using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class OobleAI : MonoBehaviour
{
    private GameObject player; // The player

    public float maxSpeed; // Maximum running speed
    public bool running; // Is running

    public float knockoutSpeed = 2f; // How fast the player needs to swing to knockout the Ooble
    private bool knockedOut = false; // True if the Ooble is knocked out
    public Material red; // The material applied to the Ooble after being knocked out

    private Rigidbody rb; // This objects rigidbody
    private Renderer oobleRenderer; // This objects renderer

    public List<Node> path = new List<Node>(); // The path the Ooble will traverse
    public int pathIndex = 0; // The current node in the path the Ooble is at

    private List<Node> painted = new List<Node>();
    private WaypointGraph waypointGraph;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        rb = GetComponent<Rigidbody>();
        oobleRenderer = GetComponent<Renderer>();
        waypointGraph = GameObject.FindGameObjectWithTag("WaypointGraph").GetComponent<WaypointGraph>();

        // Finding the node closest to the ooble
        GameObject closest = null;
        float closestDistance = float.MaxValue;

        foreach (GameObject node in waypointGraph.nodes)
        {
            if (Vector3.Distance(transform.position, node.transform.position) < closestDistance)
            {
                closestDistance = Vector3.Distance(transform.position, node.transform.position);
                closest = node;
            }
        }

        path.Add(closest.GetComponent<Node>());
    }

    private void Update()
    {
        if (!knockedOut && running)
        {
            // If we have reached the final node in the path, generate a new one
            if (pathIndex == path.Count)
            {
                // Generating a new goal node
                Node randomNode = waypointGraph.nodes[Random.Range(0, waypointGraph.nodes.Length)].GetComponent<Node>();
                while (randomNode == path[pathIndex - 1])
                    randomNode = waypointGraph.nodes[Random.Range(0, waypointGraph.nodes.Length)].GetComponent<Node>();

                painted.Clear();
                path = GreedySearch(path[pathIndex - 1], randomNode, new List<Node>());

                pathIndex = 0;

                path.Reverse();
            }

            // Moving towards the next node
            transform.LookAt(path[pathIndex].transform);
            transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);
            rb.AddForce(transform.forward * 6f * Time.deltaTime);

            // If we have reached the current node, move towards the next one
            Vector3 pos = path[pathIndex].transform.position;
            pos.y = transform.position.y;
            if (Vector3.Distance(transform.position, pos) < 0.2f)
            {
                pathIndex++;
            }

            if (rb.velocity.magnitude > 7.0f)
                rb.velocity = rb.velocity.normalized * 7.0f;
        }
        else if (knockedOut)
        {
            running = false;
            oobleRenderer.material = red;
        }
    }

    // Greedy Search pathfinding algorithm
    private List<Node> GreedySearch(Node start, Node end, List<Node> path)
    {
        Node current = start;

        foreach (Edge edge in current.edges)
        {
            if (!painted.Contains(edge.connectedNode))
            {
                painted.Add(edge.connectedNode);

                if (edge.connectedNode == end)
                {
                    path.Add(edge.connectedNode);
                    return path;
                }

                path = GreedySearch(edge.connectedNode, end, path).ToList();

                if (path.Count > 0)
                {
                    path.Add(edge.connectedNode);
                    return path;
                }
            }
        }

        return path;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.rigidbody != null && collision.rigidbody.velocity.magnitude >= knockoutSpeed && (collision.rigidbody.tag == "Bat" || collision.rigidbody.tag == "Broom"))
        {
            GetComponent<OVRGrabbable>().enabled = true;
            knockedOut = true;
        }
    }

    public void OnDiscovered()
    {
        running = true;
    }
}
