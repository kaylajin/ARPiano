using UnityEngine;

public class KeyboardBehavior : MonoBehaviour
{

    [SerializeField]
    private Camera arCamera;
    private AudioSource audioSource;
    private bool active;

    private static readonly float destroyTimer = 0.8f;

    void Awake()
    {
        audioSource = gameObject.GetComponent<AudioSource>();
        audioSource.spatialize = true;
    }

    public bool IsActive()
    {
        return active;
    }

    public void OnPress()
    {
        // Play the key
        active = true;
        Debug.Log($"distance - {Vector3.Distance(arCamera.transform.position, transform.position)}");
        SetAudioParameters();
        audioSource.Play();
        PressAnimation();
    }

    public void Release()
    {
        active = false;
        ReleaseAnimation();
        Destroy(gameObject.transform.parent.gameObject, destroyTimer);
    }

    private void PressAnimation()
    {
        GetComponent<Animation>().Play("KeyPressingAnimation");
    }

    private void ReleaseAnimation()
    {
        GetComponent<Animation>().Play("KeyReleaseAnimation");
    }

    /**
       TODO:
        1. volume scaled to inverse distance (compressor in the mixer may be hindering)
        2. emulate spatial audio, including panning along the y axis of the screen ->> how to achieve this???
     */
    // Sets the volume and panning based on the key's position and the camera (listener's position)
    private void SetAudioParameters()
    {
        audioSource.panStereo = GetPanning();
        // done on the component
        //audioSource.volume = GetVolume();
    }

    // Return between -1 and 1 (left and right)
    private float GetPanning()
    {
        Vector3 screenPos = arCamera.WorldToScreenPoint(transform.position);
        float screenWidth = Screen.width;

        // Return X position relative to the mid screen = 0 
        return (screenPos.x / screenWidth) - 0.5f ;
    }

}
