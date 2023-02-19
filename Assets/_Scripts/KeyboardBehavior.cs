using UnityEngine;

public class KeyboardBehavior : MonoBehaviour
{
    private AudioSource audioSource;
    private Animation animation;
    private bool active;

    public float speed = 1.0f;

    void Start()
    {
        audioSource = gameObject.GetComponent<AudioSource>();
        animation = gameObject.GetComponent<Animation>();
    }

    public bool IsActive()
    {
        return active;
    }

    public void OnPress()
    {
        // Play the key
        active = true;
        audioSource.Play();
        PressAnimation();
    }

    public void Release()
    {
        active = false;
        ReleaseAnimation();
    }

    private void PressAnimation()
    {
        //TODO should be rotated from where it's currently at
        animation.Play("KeyPressingAnimation");
    }

    private void ReleaseAnimation()
    {
        animation.Play("KeyReleaseAnimation");
    }
}
