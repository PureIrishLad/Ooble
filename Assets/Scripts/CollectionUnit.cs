using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectionUnit : MonoBehaviour
{
    private int numOobles;
    private int numDefeated;

    public GameObject progress;

    private void Start()
    {
        foreach(GameObject ooble in GameObject.FindGameObjectsWithTag("Ooble"))
            numOobles++;

        progress.transform.localScale = new Vector3(1, 0, 1);
        progress.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Ooble" && other.GetComponent<OobleAI>().knockedOut)
        {
            progress.SetActive(true);

            Destroy(other.gameObject);
            numDefeated++;

            progress.transform.localScale = new Vector3(1, numDefeated / (float)numOobles, 1);
        }
    }
}
