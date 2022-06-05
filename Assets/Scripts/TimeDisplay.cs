using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeDisplay : MonoBehaviour
{
    private GameManager gm;
    private TextMesh tm;

    private void Start()
    {
        gm = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        tm = GetComponent<TextMesh>();
    }

    private void Update()
    {
        int m = (int)gm.timeLimit / 60;
        int s = (int)gm.timeLimit % 60;

        string sec = s.ToString();
        if (s < 10)
            sec = "0" + sec;

        tm.text = m + ":" + sec;
    }
}
