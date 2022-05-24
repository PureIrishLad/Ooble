using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectionUnit : MonoBehaviour
{
  
    public GameObject progress;
    public GameObject progressBarPivot;
    public GameObject model;
    public float maxYOffset;
    public float maxSpeed;
    public Vector3 rotationSpeed;

    private GameManager gameManager;
    private Vector3 initialPos;
    private Vector3 maxPos;
    private float t = 0;
    private bool down = false;
    private float speed;
    private GameObject player;

    private void Start()
    {
        gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        player = GameObject.FindGameObjectWithTag("Player");

        progress.transform.localScale = new Vector3(1, 0, 1);
        progress.SetActive(false);

        initialPos = transform.position;
        maxPos = initialPos + new Vector3(0, maxYOffset, 0);

        speed = maxSpeed;
        t = 0.001f;
    }

    private void Update()
    {
        if (t >= 1)
        {
            t = 1;
            down = true;
        }
        else if (t <= 0)
        {
            t = 0;
            down = false;
        }

        speed = 0.05f + ((t < 0.5f ? t : (1 - t)) / maxSpeed);
        transform.position = Vector3.Lerp(initialPos, maxPos, t);
        t += (down ? -Time.deltaTime : Time.deltaTime) * speed;

        model.transform.rotation = Quaternion.Euler(model.transform.rotation.eulerAngles + (rotationSpeed * Time.deltaTime));
        progressBarPivot.transform.LookAt(player.transform.position);
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
