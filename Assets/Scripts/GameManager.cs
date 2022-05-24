using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class GameManager : MonoBehaviour
{
    public GameObject ooblePrefab;
    public int numOobles;

    private GameObject[] spawnPositions;

    [HideInInspector]
    public GameObject[] oobles;
    [HideInInspector]
    public InputDevice leftController;
    [HideInInspector]
    public InputDevice rightController;
    [HideInInspector]
    public int numDefeated;

    public GameObject oobleCapturedParticles;

    private InputDeviceCharacteristics leftControllerCharacteristics = InputDeviceCharacteristics.HeldInHand | InputDeviceCharacteristics.Left | InputDeviceCharacteristics.Controller;
    private InputDeviceCharacteristics rightControllerCharacteristics = InputDeviceCharacteristics.HeldInHand | InputDeviceCharacteristics.Right | InputDeviceCharacteristics.Controller;

    private void Awake()
    {
        SpawnOobles();
        StartCoroutine(GetControllers());
    }

    private IEnumerator GetControllers()
    {
        List<InputDevice> leftHandedControllers = new List<InputDevice>();
        List<InputDevice> rightHandedControllers = new List<InputDevice>();

        while (leftHandedControllers.Count == 0 && rightHandedControllers.Count == 0)
        {
            InputDevices.GetDevicesWithCharacteristics(leftControllerCharacteristics, leftHandedControllers);
            InputDevices.GetDevicesWithCharacteristics(rightControllerCharacteristics, rightHandedControllers);

            if (leftHandedControllers.Count > 0)
                leftController = leftHandedControllers[0];
            else
                Debug.Log("Left controller not detected");

            if (rightHandedControllers.Count > 0)
                rightController = rightHandedControllers[0];
            else
                Debug.Log("Right controller not detected");

            yield return null;
        }
    }

    private void SpawnOobles()
    {
        spawnPositions = GameObject.FindGameObjectsWithTag("Spawn Point");
        spawnPositions = Shuffle(spawnPositions);

        numOobles = numOobles >= spawnPositions.Length ? spawnPositions.Length : numOobles;

        List<GameObject> oobles = new List<GameObject>();

        for (int i = 0; i < numOobles; i++)
        {
            GameObject spawnPosition = spawnPositions[i];
            oobles.Add(Instantiate(ooblePrefab, spawnPosition.transform.position, Quaternion.identity));
        }

        this.oobles = oobles.ToArray();
    }

    // Fisher-Yates Shuffle Algorithm
    private static GameObject[] Shuffle(GameObject[] array)
    {
        int n = array.Length;

        while (n > 1)
        {
            int k = Random.Range(0, n--);
            GameObject temp = array[n];
            array[n] = array[k];
            array[k] = temp;
        }

        return array;
    }

    public void RemoveOoble(GameObject ooble)
    {
        numDefeated++;
        numOobles--;

        GameObject[] temp = new GameObject[numOobles];
        byte found = 0;

        for (int i = 0; i <= numOobles; i++)
        {
            if (oobles[i] == ooble)
                found = 1;
            else
                temp[i - found] = oobles[i];
        }

        GameObject ps = Instantiate(oobleCapturedParticles, ooble.transform.position, Quaternion.identity);

        Destroy(ooble.GetComponent<Rigidbody>());
        Destroy(ooble.GetComponent<Collider>());
        OobleAI ai = ooble.GetComponent<OobleAI>();
        ai.initialPos = ai.transform.position;
        ai.destroy = true;
        ai.ps = ps.GetComponent<ParticleSystemHandler>();

        oobles = temp;
    }
}
