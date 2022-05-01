using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class World : MonoBehaviour
{
    public GameObject ooblePrefab;
    public int numOoble;

    private void Start()
    {
        for (int i = 0; i < numOoble; i++)
        {
            GameObject ooble = Instantiate(ooblePrefab);
            ooble.transform.position = new Vector3(Random.Range(-9f, 9f), 0.15f, Random.Range(-9f, 9f));
        }
    }
}
