using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaypointGraph : MonoBehaviour
{
    public GameObject[] nodes;

    private void Awake()
    {
        nodes = GameObject.FindGameObjectsWithTag("Node");
    }
}
