using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectionUnit : MonoBehaviour
{
    private GameManager gameManager;    
    public GameObject progress;

    private void Start()
    {
        gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();

        progress.transform.localScale = new Vector3(1, 0, 1);
        progress.SetActive(false);
    }

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
