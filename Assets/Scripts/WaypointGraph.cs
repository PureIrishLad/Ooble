using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class WaypointGraph : MonoBehaviour
{
    public GameObject[] nodes;

    public bool regenerate;

    private void Awake()
    {
        nodes = GameObject.FindGameObjectsWithTag("Node");

        foreach(GameObject node in nodes)
            node.GetComponent<Node>().GenerateEdges();
    }

    private void Update()
    {
        if (regenerate)
        {
            Awake();
            regenerate = false;
        }
    }
}
