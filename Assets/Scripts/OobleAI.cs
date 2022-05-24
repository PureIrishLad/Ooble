using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class OobleAI : MonoBehaviour
{
    private GameObject player; // The player

    public float knockoutSpeed = 2f; // How fast the player needs to swing to knockout the Ooble
    public bool knockedOut = false; // True if the Ooble is knocked out
    public Material red; // The material applied to the Ooble after being knocked out

    private Renderer oobleRenderer; // This objects renderer

    private GameObject collectionUnit;
    [HideInInspector]
    public Vector3 initialPos;

    [HideInInspector]
    public ParticleSystemHandler ps;

    [HideInInspector]
    public bool destroy;

    private Vector3 initialScale;
    private float lerp;

    public float maxSpeed;
    public bool running;
    public List<Node> path = new List<Node>();
    public int pathIndex = 0;

    private Rigidbody rb;
    private List<Node> painted = new List<Node>();
    private WaypointGraph waypointGraph;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        oobleRenderer = GetComponent<Renderer>();
        collectionUnit = GameObject.FindGameObjectWithTag("CollectionUnit");
        rb = GetComponent<Rigidbody>();
        waypointGraph = GameObject.FindGameObjectWithTag("WaypointGraph").GetComponent<WaypointGraph>();

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

        initialScale = transform.localScale;
    }

    private void Update()
    {
        if (!destroy)
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
        else
            DestroyThis();   
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
        if (collision.rigidbody != null && collision.rigidbody.velocity.magnitude >= knockoutSpeed && collision.rigidbody.tag == "Bat")
        {
            GetComponent<OVRGrabbable>().enabled = true;
            knockedOut = true;
            oobleRenderer.material = red;
        }
    }

    public void OnDiscovered(float f)
    {
        running = true;
    }

    private void DestroyThis()
    {
        transform.localScale = Vector3.Lerp(initialScale, new Vector3(0, 0, 0), lerp);
        transform.position = Vector3.Lerp(initialPos, collectionUnit.transform.position, lerp);
        lerp += Time.deltaTime * 2;

        if (lerp > 1.0f)
        {
            ps.Play();
            Destroy(gameObject);
        }
    }
}
