using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

using static Utility;

public class KeyboardManager : MonoBehaviour
{

    [SerializeField]
    private Camera arCamera;
    private static ARRaycastManager raycastManager;
    private static List<ARRaycastHit> hits = new List<ARRaycastHit>();

    private List<GameObject> keys;
    private static readonly List<string> whiteKeys = new List<string> { "C3", "D3", "E3", "F3", "G3", "A3", "B3", "C4" };
    private static readonly List<string> blackKeys = new List<string> { "Db3", "Eb3", "F#3", "Ab3", "Bb3" };

    private Vector2[] touchPositions;

    [SerializeField]
    private TextMeshPro textUI;

    private readonly float textUIliveSeconds = 1f;
    // seconds this key is live and cannot played again
    private readonly float liveSeconds = 0.8f;
    private readonly List<string> liveKeys = new List<string>();

    void Start()
    {
        raycastManager = GameObject.FindObjectOfType<ARRaycastManager>();
        InitializeKeys(out keys);
    }

    // Update is called once per frame
    void Update()
    {
        if (TrySummonKeys(out Vector3 position) && liveKeys.Count > 0)
        {
            string chord = ChordUtility.GetChord(liveKeys);
            Debug.Log($"Displaying: {chord} for keys pressed {string.Join(", ", liveKeys)} at position {position}");
            SummonText(chord, position);
        }
    }

    private bool TrySummonKeys(out Vector3 position)
    {
        if (!TryGetInputPosition(out touchPositions) || touchPositions.Length == 0)
        {
            position = new Vector3(0,0,0);
            return false;
        }

        // Detect touches and Summon keys
        position = new Vector3(0, 0, 0);
        foreach (Vector2 touchPosition in touchPositions)
        {
            if (raycastManager.Raycast(touchPosition, hits, TrackableType.AllTypes))
            {
                int randomIndex = Random.Range(0, keys.Count);
                Pose pose = hits[0].pose; // first hit
                position = pose.position; // just take the last one in the array

                GameObject randomKey = Instantiate(keys[randomIndex], pose.position, GetQuaternion(pose.position));
                Debug.Log($"random key {randomKey.name} rotation at {randomKey.transform.eulerAngles} vs {randomKey.transform.localEulerAngles}");
                
                string keyName = randomKey.name.Replace("(Clone)", "");
                KeyboardBehavior key = randomKey.GetComponentInChildren<KeyboardBehavior>();
                if (!key.IsActive())
                {
                    PressKey(key, keyName);
                }
            }
        }

        return true;
    }

    // Return angle facing the camera, with slightly randomized rotation around y axis.
    private Quaternion GetQuaternion(Vector3 position)
    {
        Vector3 cameraDirection = Quaternion.AngleAxis(Random.Range(-45,45), Vector3.up) * (arCamera.transform.position - position);
        return Quaternion.LookRotation(cameraDirection, arCamera.transform.up);
    }

    private void SummonText(string message, Vector3 position)
    {
        TextMeshPro textObject = Instantiate(textUI, position, arCamera.transform.rotation);
        textObject.text = message;
        Destroy(textObject, textUIliveSeconds);
    }

    private void PressKey(KeyboardBehavior key, string name)
    {
        liveKeys.Add(name);
        key.OnPress();
        StartCoroutine(ReleaseKey(key, name, liveSeconds));
    }

    private IEnumerator ReleaseKey(KeyboardBehavior key, string name, float delay)
    {
        yield return new WaitForSeconds(delay);
        liveKeys.Remove(name);
        key.Release();
    }

    private static void InitializeKeys(out List<GameObject> keys)
    {
        keys = new List<GameObject>();
        foreach (string key in whiteKeys)
        {
            GameObject newObject = Instantiate(Resources.Load<GameObject>($"Prefabs/WhiteKeyPrefab"), null, false);
            if (newObject == null) Debug.LogError("Failed to initialize");
            newObject.name = key;
            AudioSource audioSource = newObject.GetComponentInChildren<AudioSource>();
            if (audioSource == null) Debug.LogError("Failed to initialize");
            audioSource.clip = Resources.Load($"Audio/{key}") as AudioClip;
            if (audioSource.clip == null) Debug.LogError("Failed to initialize");
            keys.Add(newObject);
        }
        foreach (string key in blackKeys)
        {
            GameObject newObject = Instantiate(Resources.Load<GameObject>($"Prefabs/BlackKeyPrefab"), null, false);
            if (newObject == null) Debug.LogError("Failed to initialize");
            newObject.name = key;
            AudioSource audioSource = newObject.GetComponentInChildren<AudioSource>();
            if (audioSource == null) Debug.LogError("Failed to initialize");
            audioSource.clip = Resources.Load($"Audio/{key}") as AudioClip;
            if (audioSource.clip == null) Debug.LogError("Failed to initialize");
            keys.Add(newObject);
        }
    }

}
