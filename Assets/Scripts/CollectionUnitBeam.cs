using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectionUnitBeam : MonoBehaviour
{
    public GameManager gameManager;
    public GameObject progress;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Ooble" && other.GetComponent<OobleAI>().knockedOut)
        {
            progress.SetActive(true);

            gameManager.RemoveOoble(other.gameObject);

            progress.transform.localScale = new Vector3(1, gameManager.numDefeated / (float)(gameManager.numOobles + gameManager.numDefeated), 1);
        }
    }
}
