using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class VRButton : MonoBehaviour
{
    public Transform end;
    public int loadScene = 1;
    private bool loading = false;

    private void Update()
    {
        if (transform.position.y <= end.position.y && !loading)
            Event();
    }

    private void Event()
    {
        SceneManager.LoadSceneAsync(loadScene);
        loading = true;
    }
}
