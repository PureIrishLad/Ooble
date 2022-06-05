using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class GameManager : MonoBehaviour
{
    public GameObject ooblePrefab;
    public int numOobles; // The number of oobles left
    public float timeLimit = 60;

    // Array of all possible ooble spawn positions
    private GameObject[] spawnPositions;

    [HideInInspector]
    public GameObject[] oobles;
    [HideInInspector]
    public InputDevice leftController;
    [HideInInspector]
    public InputDevice rightController;
    [HideInInspector]
    public int numDefeated;

    // Particle object prefabs
    public GameObject oobleCapturedParticles;
    public GameObject plateSmashParticles;
    public GameObject attackTrailParticles;
    public GameObject attackExplosionParticles;
    public GameObject telekinesisParticles;

    // Audio object prefabs
    public GameObject oobleCapturedAudio;
    public GameObject plateSmashAudio;
    public GameObject attackSpellFireAudio;
    public GameObject attackSpellHitAudio;
    public GameObject wandTelekinesisAudio;
    public GameObject fireworkAudio;

    private InputDeviceCharacteristics leftControllerCharacteristics = InputDeviceCharacteristics.HeldInHand | InputDeviceCharacteristics.Left | InputDeviceCharacteristics.Controller;
    private InputDeviceCharacteristics rightControllerCharacteristics = InputDeviceCharacteristics.HeldInHand | InputDeviceCharacteristics.Right | InputDeviceCharacteristics.Controller;

    private void Awake()
    {
        SpawnOobles();
        StartCoroutine(GetControllers());
    }

    private void Update()
    {
        timeLimit -= Time.deltaTime;

        if (timeLimit < 0)
            timeLimit = 0;
    }

    // Gets the controller using characteristics
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

    // Randomly spawns oobles
    private void SpawnOobles()
    {
        spawnPositions = GameObject.FindGameObjectsWithTag("Spawn Point");
        spawnPositions = Shuffle(spawnPositions);

        // Making sure we don't try to spawn more oobles than is possible
        numOobles = Mathf.Min(spawnPositions.Length, numOobles);

        List<GameObject> oobles = new List<GameObject>();

        for (int i = 0; i < numOobles; i++)
        {
            GameObject spawnPosition = spawnPositions[i];
            GameObject ooble = Instantiate(ooblePrefab, spawnPosition.transform.position, Quaternion.identity);
            OobleAI ai = ooble.GetComponent<OobleAI>();

            DiegeticRotator dr = spawnPosition.transform.parent.GetComponent<DiegeticRotator>();

            dr.onValueChanged = new UnityEngine.Events.UnityEvent<float>();
            dr.onValueChanged.AddListener(ai.OnDiscovered);
            dr.rotatablePart.GetComponent<CupboardTrigger>().ooble = ai;

            oobles.Add(ooble);
        }

        this.oobles = oobles.ToArray();
    }

    // Fisher-Yates Shuffle Algorithm for randomly shuffling an array
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

    // Removes an ooble from the game
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
        GameObject ao = Instantiate(fireworkAudio, ooble.transform.position, Quaternion.identity);
        AudioSystemHandler a = Instantiate(oobleCapturedAudio, ooble.transform.position, Quaternion.identity).GetComponent<AudioSystemHandler>();
        a.Play();

        OobleAI ai = ooble.GetComponent<OobleAI>();

        Destroy(ooble.GetComponent<OVRGrabbable>());
        Destroy(ooble.GetComponent<Rigidbody>());
        Destroy(ooble.GetComponent<Collider>());
        Destroy(ai.pointer);

        ai.initialPos = ai.transform.position;
        ai.destroy = true;
        ai.ps = ps.GetComponent<ParticleSystemHandler>();
        ai.a = ao.GetComponent<AudioSystemHandler>();

        oobles = temp;
    }
}
