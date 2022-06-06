using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Controls Ooble behaviour
public class OobleAI : MonoBehaviour
{
    // Compass pointer
    public GameObject pointerPrefab;
    [HideInInspector]
    public GameObject pointer;

    public float knockoutSpeed = 2f; // How fast the player needs to swing to knockout the Ooble
    public bool knockedOut = false; // True if the Ooble is knocked out
    public Material red; // The material applied to the Ooble after being knocked out

    public float health = 100;

    public Renderer oobleRenderer; // This objects renderer
    public Renderer eyeRenderer;

    public Material knockedOutEyesMaterial;

    public float rotateSpeed = 5.0f;

    private GameObject collectionUnit;
    [HideInInspector]
    public Vector3 initialPos; // The initial position of the ooble when it entered the collection units tractor beam
    private Vector3 initialScale;
    private float lerp;

    [HideInInspector]
    public ParticleSystemHandler ps;
    [HideInInspector]
    public AudioSystemHandler a;

    [HideInInspector]
    public bool destroy; // True if the ooble is in the process of being destroyed

    // Movement and pathfinding
    public float maxSpeed;
    public bool running;
    public List<Node> path = new List<Node>();
    private List<Node> painted = new List<Node>();
    public int pathIndex = 0;
    private WaypointGraph waypointGraph;

    private Rigidbody rb;

    private void Start()
    {
        collectionUnit = GameObject.FindGameObjectWithTag("CollectionUnit");
        rb = GetComponent<Rigidbody>();
        waypointGraph = GameObject.FindGameObjectWithTag("WaypointGraph").GetComponent<WaypointGraph>();

        // Finding the closest node so when ooble is discovered it runs towards it
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
        pointer = Instantiate(pointerPrefab);
        pointer.GetComponent<TargetPointer>().positionTarget = transform;
        pointer.SetActive(false);
    }

    private void Update()
    {
        if (health <= 0)
            knockedOut = true;

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

                Vector3 pos = path[pathIndex].transform.position;
                pos.y = transform.position.y;

                Quaternion lookOnLook = Quaternion.LookRotation(pos - transform.position);

                transform.rotation = Quaternion.Slerp(transform.rotation, lookOnLook, Time.deltaTime * rotateSpeed);

                // Moving towards the next node
                transform.rotation = Quaternion.Euler(0, transform.eulerAngles.y, 0);
                rb.AddForce(transform.forward * maxSpeed * Time.deltaTime);

                // If we have reached the current node, move towards the next one
                if (Vector3.Distance(transform.position, pos) < 0.5f)
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
                eyeRenderer.material = knockedOutEyesMaterial;

                pointer.SetActive(true);
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

    // Function gets run when the ooble is discovered
    public void OnDiscovered(float f)
    {
        running = true;
    }

    // Function runs when the ooble is in the process of being destroyed
    private void DestroyThis()
    {
        transform.localScale = Vector3.Lerp(initialScale, new Vector3(0, 0, 0), lerp);
        transform.position = Vector3.Lerp(initialPos, collectionUnit.transform.position, lerp);
        lerp += Time.deltaTime * 2;

        if (lerp > 1.0f)
        {
            ps.Play();
            a.Play();
            Destroy(gameObject);
        }
    }
}
