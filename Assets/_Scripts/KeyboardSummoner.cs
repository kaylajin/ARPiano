using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using System.Collections.Generic;

using static Utility;

public class KeyboardSummoner: MonoBehaviour
{
    [SerializeField]
    private Camera arCamera;
    private static ARRaycastManager raycastManager;
    private static List<ARRaycastHit> hits = new List<ARRaycastHit>();

    [SerializeField]
    private GameObject keyboard;
    private Vector2[] touchPositions;

    // Use this for initialization
    void Start()
    {
        raycastManager = GameObject.FindObjectOfType<ARRaycastManager>();
    }

    // Update is called once per frame
    void Update()
    {
        // TODO allow summon only once
        if (!TryGetInputPosition(out touchPositions)) return;

        // TODO positions can be empty
        if (raycastManager.Raycast(touchPositions[0], hits, TrackableType.AllTypes))
        {
            Pose pose = hits[0].pose;
            GameObject ob = Instantiate(keyboard, pose.position, arCamera.transform.rotation);
            gameObject.GetComponent<KeyboardSummoner>().enabled = false;
        }
    }
}
