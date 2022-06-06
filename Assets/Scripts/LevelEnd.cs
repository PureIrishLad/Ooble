using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelEnd : MonoBehaviour
{
    private GameManager gm;
    [HideInInspector]
    public bool gameWon = false;
    [HideInInspector]
    public bool gameLost = false;

    public float waitTime;

    public OVRScreenFade fader;

    private void Start()
    {
        gm = GetComponent<GameManager>();
    }

    private void Update()
    {
        if (!gameWon && !gameLost)
        {
            gameWon = gm.numOobles == 0;
            gameLost = gm.timeLimit == 0 && !gameWon;

            if (gameWon || gameLost)
            {
                fader.FadeOut();
            }
        }
        else
            waitTime -= Time.deltaTime;


        if (waitTime <= 0)
        {
            if (gameWon)
                SceneManager.LoadSceneAsync(2);
            else
                SceneManager.LoadSceneAsync(3);
        }
    }
}
