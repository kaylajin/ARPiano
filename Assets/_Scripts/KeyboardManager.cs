using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

using static Utility;

public class KeyboardManager : MonoBehaviour
{
    //TODO ** audio distortion for multiple keys

    [SerializeField]
    private Camera arCamera;
    private static ARRaycastManager raycastManager;
    private static List<ARRaycastHit> hits = new List<ARRaycastHit>();

    [SerializeField]
    private GameObject[] keys;
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
                int randomIndex = Random.Range(0, keys.Length);
                Pose pose = hits[0].pose; // first hit
                position = pose.position; // just take the last one in the array

                GameObject randomKey = Instantiate(keys[randomIndex], pose.position, getRotation(pose.position));
                
                string keyName = randomKey.name.Replace("(Clone)", "");
                KeyboardBehavior key = randomKey.GetComponent<KeyboardBehavior>();
                if (!key.IsActive())
                {
                    PressKey(key, keyName);
                }
            }
        }

        return true;
    }

    private Quaternion getRotation(Vector3 position)
    {
        Vector3 cameraDirection = arCamera.transform.position - position;
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
        Debug.Log($"Now releasing key: {name}");
        liveKeys.Remove(name);
        key.Release();
    }
}
