using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

using static Utility;

public class KeyboardManager : MonoBehaviour
{

    [SerializeField]
    private Camera arCamera;
    [SerializeField]
    private LayerMask keyboardLayerMask;
    private Vector2[] touchPositions;
    private Ray ray;
    private RaycastHit hit;

    private TextMeshProUGUI textComponent;
    [SerializeField]
    private TextMeshPro text2;

    // seconds this key is live and cannot played again
    private readonly float liveSeconds = 0.8f;
    private readonly List<string> liveKeys = new List<string>();

    void Start()
    {
        GameObject canvas = GameObject.Find("Canvas");
        textComponent = canvas.GetComponentInChildren<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        PlayAndDisplayChord();
    }

    private void PlayAndDisplayChord()
    {
        if (!TryGetInputPosition(out touchPositions)) return;

        // Detect Keys Pressed
        foreach (Vector2 touchPosition in touchPositions)
        {

            ray = arCamera.ScreenPointToRay(touchPosition);
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, keyboardLayerMask))
            {
                string keyName = hit.collider.gameObject.name;
                KeyboardBehavior key = hit.collider.gameObject.GetComponent<KeyboardBehavior>();

                if (key != null && !key.IsActive())
                {
                    PressKey(key, keyName);
                }
            }
        }

        // Fetch and Display Chord
        string chord = ChordUtility.GetChord(liveKeys);
        Debug.Log($"Displaying {chord} given {string.Join(", ", liveKeys)}");
        textComponent.text = chord;
        text2.text = chord;
    }

    /* There will be at most 12 coroutines running at a time (for each key) */
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
